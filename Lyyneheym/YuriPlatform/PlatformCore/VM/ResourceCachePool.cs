using System;
using System.Collections.Generic;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 缓冲池类
    /// </summary>
    internal static class ResourceCachePool
    {
        /// <summary>
        /// 通过资源标识符获取对应资源缓冲
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <param name="cType">资源缓存类型</param>
        /// <returns>缓冲区中的字节数组</returns>
        public static byte[] Refer(string resourceId, ResourceCacheType cType)
        {
            bool inPermanentFlag = false;
            if (cType == ResourceCacheType.Unknown)
            {
                inPermanentFlag = ResourceCachePool.PermanentDictionary.ContainsKey(resourceId);
            }
            CacheBlock cBlock;
            if (cType == ResourceCacheType.Eden || (inPermanentFlag == false && cType == ResourceCacheType.Unknown))
            {
                var edenIdx = ResourceCachePool.EdenQueue.FindIndex(x => x.CacheId == resourceId);
                if (edenIdx == -1 || edenIdx > GlobalConfigContext.EdenResourceCacheSize)
                {
                    return null;
                }
                cBlock = ResourceCachePool.EdenQueue.ElementAt(edenIdx);
                cBlock.Referred();
                ResourceCachePool.EdenQueue.Exchange(edenIdx, ResourceCachePool.EdenQueue.Count() - 1);
                ResourceCachePool.EdenQueue.Adjust();
                ResourceCachePool.EdenQueue.ForEachFrom(GlobalConfigContext.EdenResourceCacheSize, t => t.Abandon());
            }
            else
            {
                cBlock = ResourceCachePool.PermanentDictionary.ContainsKey(resourceId)
                    ? ResourceCachePool.PermanentDictionary[resourceId]
                    : null;
            }
            return cBlock?.AllocReference;
        }

        /// <summary>
        /// 将一个资源注册到缓冲区中
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <param name="allocPtr">资源的字节数组</param>
        /// <param name="cType">资源缓存类型</param>
        /// <param name="priority">初始引用计数</param>
        public static void Register(string resourceId, byte[] allocPtr, ResourceCacheType cType, long priority = 1)
        {
            switch (cType)
            {
                case ResourceCacheType.Eden:
                    var edenIdx = ResourceCachePool.EdenQueue.FindIndex(x => x.CacheId == resourceId);
                    if (edenIdx == -1)
                    {
                        ResourceCachePool.EdenQueue.Push(new CacheBlock(resourceId, allocPtr, priority));
                    }
                    else
                    {
                        var cBlock = ResourceCachePool.EdenQueue.ElementAt(edenIdx);
                        cBlock.Referred();
                        cBlock.AllocReference = allocPtr;
                        ResourceCachePool.EdenQueue.Exchange(edenIdx, ResourceCachePool.EdenQueue.Count() - 1);
                        ResourceCachePool.EdenQueue.Adjust();
                        ResourceCachePool.EdenQueue.ForEachFrom(GlobalConfigContext.EdenResourceCacheSize, t => t.Abandon());
                    }
                    break;
                case ResourceCacheType.Permanent:
                    ResourceCachePool.PermanentDictionary.Add(resourceId, new CacheBlock(resourceId, allocPtr, priority));
                    break;
            }
        }

        /// <summary>
        /// 清空缓冲池
        /// </summary>
        public static void Clear()
        {
            ResourceCachePool.EdenQueue.Clear();
            ResourceCachePool.PermanentDictionary.Clear();
        }

        /// <summary>
        /// 临时资源缓冲优先队列
        /// </summary>
        private static readonly PriorityQueue<CacheBlock> EdenQueue = new PriorityQueue<CacheBlock>();

        /// <summary>
        /// 常驻资源字典
        /// </summary>
        private static readonly Dictionary<string, CacheBlock> PermanentDictionary = new Dictionary<string, CacheBlock>();
        
        /// <summary>
        /// 缓存块类：维持文件读入的字节流的引用和访问次数统计的块
        /// </summary>
        private sealed class CacheBlock : IComparable<CacheBlock>
        {
            /// <summary>
            /// 构造器
            /// </summary>
            /// <param name="cacheId">缓冲的唯一标识符</param>
            /// <param name="allocPtr">对象在内存中的引用</param>
            /// <param name="beginCount">起始引用计数</param>
            public CacheBlock(string cacheId, byte[] allocPtr, long beginCount = 0)
            {
                this.CacheId = cacheId;
                this.AllocReference = allocPtr;
                this.referenceCount = beginCount;
            }
            
            /// <summary>
            /// 引用这个对象一次
            /// </summary>
            /// <returns>对象本身</returns>
            public CacheBlock Referred()
            {
                this.referenceCount++;
                return this;
            }

            /// <summary>
            /// 将该对象的引用计数清空，这可能使缓冲池放弃对其引用的维持从而触发CLR垃圾回收
            /// </summary>
            public void Abandon()
            {
                this.AllocReference = null;
                this.referenceCount = 0;
            }

            /// <summary>
            /// 获取或设置缓冲块的唯一标识符
            /// </summary>
            public string CacheId
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取或设置对象字节流在内存中的引用
            /// </summary>
            public byte[] AllocReference
            {
                get;
                set;
            }

            /// <summary>
            /// 获取或设置引用次数
            /// </summary>
            private long referenceCount
            {
                get; set;
            }

            /// <summary>
            /// 比较两个缓冲块的优先度
            /// </summary>
            /// <param name="other">要对比的另一个块</param>
            /// <returns>两个缓冲块的被引用次数的比较结果</returns>
            public int CompareTo(CacheBlock other)
            {
                return this.referenceCount.CompareTo(other.referenceCount);
            }

            /// <summary>
            /// 字符串化方法
            /// </summary>
            /// <returns>该缓冲块的描述文本</returns>
            public override string ToString()
            {
                return String.Format("CacheBlock: {0} (RefCount:{1}, Length:{2})", this.CacheId, this.referenceCount, this.AllocReference.Length);
            }
        }

        /// <summary>
        /// 大者优先的优先队列
        /// </summary>
        /// <typeparam name="T">T是队列元素的可比较类型</typeparam>
        private sealed class PriorityQueue<T> where T : IComparable<T>
        {
            /// <summary>
            /// 构造器
            /// </summary>
            public PriorityQueue()
            {
                this.buffer = new T[defaultCapacity];
                this.heapLength = 0;
            }

            /// <summary>
            /// 查询队列是否为空
            /// </summary>
            /// <returns>队列是否为空的布尔值</returns>
            public bool Empty()
            {
                return this.heapLength == 0;
            }

            /// <summary>
            /// 获取队列中的项目数量
            /// </summary>
            /// <returns>队列长度</returns>
            public int Count()
            {
                return this.heapLength;
            }

            /// <summary>
            /// 获取队列的容量
            /// </summary>
            /// <returns>队列能接受的最大项目数</returns>
            public int Capacity()
            {
                return this.buffer.Length;
            }

            /// <summary>
            /// 清空队列
            /// </summary>
            public void Clear()
            {
                this.heapLength = 0;
            }

            /// <summary>
            /// 查看队头元素
            /// </summary>
            /// <returns>队头元素</returns>
            public T Top()
            {
                if (this.heapLength != 0)
                {
                    return this.buffer[0];
                }
                throw new OverflowException("队列中没有元素");
            }

            /// <summary>
            /// 将元素排队
            /// </summary>
            /// <param name="obj">要放入的元素</param>
            public void Push(T obj)
            {
                // 注意：这里不拓展，直接替换掉队尾
                if (this.heapLength == this.buffer.Length)
                {
                    this.expand();
                }
                // 维护堆的属性
                this.buffer[heapLength] = obj;
                MaxHeap<T>.heapAdjustFromBottom(this.buffer, this.heapLength);
                this.heapLength++;
            }

            /// <summary>
            /// 弹出队头
            /// </summary>
            public void Pop()
            {
                if (this.heapLength == 0)
                {
                    throw new OverflowException("队列为空时不能出队");
                }
                // 维护堆的属性
                heapLength--;
                this.swap(0, heapLength);
                MaxHeap<T>.heapAdjustFromTop(this.buffer, 0, this.heapLength);
            }

            /// <summary>
            /// 立即维护队列的特性
            /// </summary>
            public void Adjust()
            {
                if (this.heapLength > 0)
                {
                    MaxHeap<T>.heapAdjustFromBottom(this.buffer, this.heapLength - 1);
                }
            }
            
            /// <summary>
            /// 在优先队列中寻找第一个符合条件的对象
            /// </summary>
            /// <param name="match">条件的lambda表达式</param>
            /// <returns>对象的引用</returns>
            public T Find(Predicate<T> match)
            {
                for (int i = 0; i < this.heapLength; i++)
                {
                    if (match(buffer[i]))
                    {
                        return buffer[i];
                    }
                }
                return default(T);
            }

            /// <summary>
            /// 在优先队列中寻找第一个符合条件的对象
            /// </summary>
            /// <param name="match">条件的lambda表达式</param>
            /// <returns>对象下标</returns>
            public int FindIndex(Predicate<T> match)
            {
                for (int i = 0; i < this.heapLength; i++)
                {
                    if (match(buffer[i]))
                    {
                        return i;
                    }
                }
                return -1;
            }

            /// <summary>
            /// 从某个下标开始对队列中的元素执行动作
            /// </summary>
            /// <param name="beginIdx">开始下标（含）</param>
            /// <param name="act">动作委托</param>
            public void ForEachFrom(int beginIdx, Action<T> act)
            {
                int border = Math.Max(0, Math.Min(beginIdx, this.Capacity()));
                int capa = this.Capacity();
                for (int i = border; i < capa; i++)
                {
                    act(this.buffer[i]);
                }
            }

            /// <summary>
            /// 以某个下标为结束对队列中的元素执行动作
            /// </summary>
            /// <param name="endIdx">结束下标（含）</param>
            /// <param name="act">动作委托</param>
            public void ForEachTo(int endIdx, Action<T> act)
            {
                int border = Math.Max(0, Math.Min(endIdx + 1, this.Capacity()));
                for (int i = 0; i < border; i++)
                {
                    act(this.buffer[i]);
                }
            }

            /// <summary>
            /// 以某个下标为开始对队列中的后续N个元素执行动作
            /// </summary>
            /// <param name="beginIdx">开始下标（含）</param>
            /// <param name="n">最大处理个数</param>
            /// <param name="act">动作委托</param>
            public void ForEachTo(int beginIdx, int n, Action<T> act)
            {
                int beginT = Math.Max(0, Math.Min(beginIdx, this.Capacity()));
                int endT = Math.Max(0, Math.Min(beginIdx + n, this.Capacity()));
                for (int i = beginT; i < endT; i++)
                {
                    act(this.buffer[i]);
                }
            }

            /// <summary>
            /// 获取指定位置的对象
            /// </summary>
            /// <param name="index">下标</param>
            /// <returns>对象的引用</returns>
            public T ElementAt(int index)
            {
                if (index < this.heapLength && index >= 0)
                {
                    return this.buffer[index];
                }
                return default(T);
            }

            /// <summary>
            /// 交换两个对象
            /// </summary>
            /// <param name="aIdx">第一个对象的下标</param>
            /// <param name="bIdx">第二个对象的下标</param>
            public void Exchange(int aIdx, int bIdx)
            {
                this.swap(aIdx, bIdx);
            }

            /// <summary>
            /// 拓展队列长度
            /// </summary>
            private void expand()
            {
                Array.Resize<T>(ref buffer, buffer.Length * 2);
            }

            /// <summary>
            /// 辅助函数：交换元素
            /// </summary>
            private void swap(int a, int b)
            {
                T tmp = this.buffer[a];
                this.buffer[a] = this.buffer[b];
                this.buffer[b] = tmp;
            }

            /// <summary>
            /// 堆的长度
            /// </summary>
            private int heapLength;

            /// <summary>
            /// 堆向量
            /// </summary>
            private T[] buffer;

            /// <summary>
            /// 堆的原始尺寸
            /// </summary>
            private const int defaultCapacity = 128;
        }

        /// <summary>
        /// 大顶堆操作类
        /// </summary>
        /// <typeparam name="T">T是堆元素的类型</typeparam>
        private static class MaxHeap<T> where T : IComparable<T>
        {
            /// <summary>
            /// 将输入进行堆排序
            /// </summary>
            public static void HeapSort(T[] objects)
            {
                for (int i = objects.Length / 2 - 1; i >= 0; --i)
                {
                    heapAdjustFromTop(objects, i, objects.Length);
                }
                for (int i = objects.Length - 1; i > 0; --i)
                {
                    swap(objects, i, 0);
                    heapAdjustFromTop(objects, 0, i);
                }
            }

            /// <summary>
            /// 自下维护堆的属性
            /// </summary>
            public static void heapAdjustFromBottom(T[] objects, int n)
            {
                while (n > 0 && objects[(n - 1) >> 1].CompareTo(objects[n]) < 0)
                {
                    swap(objects, n, (n - 1) >> 1);
                    n = (n - 1) >> 1;
                }
            }

            /// <summary>
            /// 自上维护堆的属性
            /// </summary>
            public static void heapAdjustFromTop(T[] objects, int n, int len)
            {
                while ((n << 1) + 1 < len)
                {
                    int m = (n << 1) + 1;
                    if (m + 1 < len && objects[m].CompareTo(objects[m + 1]) < 0)
                    {
                        ++m;
                    }
                    if (objects[n].CompareTo(objects[m]) > 0)
                    {
                        return;
                    }
                    swap(objects, n, m);
                    n = m;
                }
            }

            /// <summary>
            /// 辅助函数：交换元素
            /// </summary>
            private static void swap(T[] objects, int a, int b)
            {
                T tmp = objects[a];
                objects[a] = objects[b];
                objects[b] = tmp;
            }
        }
    }

    /// <summary>
    /// 枚举：缓存的类型
    /// </summary>
    internal enum ResourceCacheType
    {
        /// <summary>
        /// 无法确定资源类型（只能用在获取方法，否则资源不被缓存）
        /// </summary>
        Unknown,
        /// <summary>
        /// 最近最常使用原则缓存
        /// </summary>
        Eden,
        /// <summary>
        /// 常驻内存
        /// </summary>
        Permanent
    }
}
