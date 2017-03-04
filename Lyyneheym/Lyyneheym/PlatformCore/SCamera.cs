using System;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// <para>场景镜头类：为游戏提供有3D景深效果的镜头移动动画</para>
    /// <para>注意所有效果在施加到调用堆栈上后该函数即刻结束，不等待动画完成</para>
    /// <para>她是一个静态类，被画音渲染器UpdateRender引用</para>
    /// </summary>
    public static class SCamera
    {
        /// <summary>
        /// 将镜头中心平移到指定的区块
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        public static void Translate(int r, int c)
        {

        }

        /// <summary>
        /// 在镜头即将对准的区块上调整焦距
        /// 即将对准的意思是：当前全部带平移的动作都完成之后的镜头中心所对准的位置
        /// </summary>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸是1.0</param>
        public static void Focus(double ratio)
        {

        }

        /// <summary>
        /// 将镜头对准某个立绘的指定区块并调整焦距
        /// </summary>
        /// <param name="id">立绘id</param>
        /// <param name="blockId">立绘纵向划分区块id，值域[0, 11]，通常眼1，胸3，腹4，膝7，足10</param>
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0，原始尺寸指设置中所定义的立绘原始缩放比</param>
        public static void FocusCharacter(int id, int blockId, double ratio)
        {

        }

        /// <summary>
        /// 重置镜头对准屏幕中心并采用1.0的对焦比例
        /// </summary>
        public static void ResetFocus()
        {

        }

        /// <summary>
        /// 进入场景时的默认拉长焦距效果
        /// </summary>
        public static void EnterSceneZoomOut()
        {

        }
    }
}
