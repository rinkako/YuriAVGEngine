using System;
using System.Collections.Generic;

namespace Yuri.YuriHalation.ScriptPackage
{
    /// <summary>
    /// 可执行包装类
    /// </summary>
    [Serializable]
    internal class RunnablePackage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RunnablePackage()
        {
            ActionPackage pad = new ActionPackage()
            {
                nodeName = "pad",
                nodeType = ActionPackageType.NOP
            };
            this.APList.Add(pad);
        }

        /// <summary>
        /// 增加一个动作
        /// </summary>
        /// <param name="ap">动作包装</param>
        /// <param name="insertLine">插入的行</param>
        /// <returns>操作成功与否</returns>
        public bool AddAction(ActionPackage ap, int insertLine)
        {
            if (insertLine >= 0 && insertLine < this.APList.Count)
            {
                this.APList.Insert(insertLine, ap);
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 替换指定位置的一个动作
        /// </summary>
        /// <param name="ap">新的动作包装</param>
        /// <param name="editLine">要被替换的行</param>
        /// <returns>操作成功与否</returns>
        public bool ReplaceAction(ActionPackage ap, int editLine)
        {
            if (editLine >= 0 && editLine < this.APList.Count)
            {
                this.APList[editLine] = ap;
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查找一个局部变量，如果不存在就增加它；并且增添它的被引用次数
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="isLeftValue">是否作为左值</param>
        /// <param name="line">出现的行</param>
        /// <returns>在调用方法前该变量是否不存在</returns>
        public bool SignalVar(string varName, bool isLeftValue, int line)
        {
            VariablePackage varItem = null;
            if ((varItem = this.localVarList.Find((x) => x.varName == varName)) != null)
            {
                if (isLeftValue)
                {
                    varItem.firstLeftValueLine = Math.Min(line, varItem.firstLeftValueLine);
                }
                varItem.referenceCount++;
                return false;
            }
            varItem = new VariablePackage()
            {
                varName = varName,
                firstLeftValueLine = isLeftValue ? line : Int32.MaxValue,
                isGlobal = false,
                referenceCount = 1
            };
            this.localVarList.Add(varItem);
            return true;
        }

        /// <summary>
        /// 删除一个动作
        /// </summary>
        /// <param name="line">所在行</param>
        /// <return>操作成功与否</return>
        public bool DeleteAction(int line)
        {
            if (line >= 0 && line < this.APList.Count)
            {
                this.APList.RemoveAt(line);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除对局部变量的引用
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <return>操作成功与否</return>
        public bool UnsignalVar(string varName)
        {
            VariablePackage varItem = null;
            if ((varItem = this.localVarList.Find((x) => x.varName == varName)) != null)
            {
                varItem.referenceCount--;
                if (varItem.referenceCount == 0)
                {
                    this.localVarList.Remove(varItem);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取一个动作包装
        /// </summary>
        /// <param name="line">所在行数</param>
        /// <returns>动作包装实例</returns>
        public ActionPackage GetAction(int line)
        {
            if (line >= 0 && line < this.APList.Count)
            {
                return this.APList[line];
            }
            return null;
        }

        /// <summary>
        /// 获取该场景的所有动作包装
        /// </summary>
        /// <returns>动作包装序列</returns>
        public List<ActionPackage> GetAction()
        {
            return this.APList;
        }

        /// <summary>
        /// 获取一个变量包装
        /// </summary>
        /// <param name="name">变量名</param>
        /// <returns>变量包装实例</returns>
        public VariablePackage GetVar(string name)
        {
            return this.localVarList.Find((x) => x.varName == name);
        }

        /// <summary>
        /// 获取代码的行数
        /// </summary>
        /// <returns>代码的行数</returns>
        public int APListCount()
        {
            return this.APList.Count;
        }

        /// <summary>
        /// 场景动作向量
        /// </summary>
        protected List<ActionPackage> APList = new List<ActionPackage>();

        /// <summary>
        /// 场景内变量向量
        /// </summary>
        protected List<VariablePackage> localVarList = new List<VariablePackage>();
    }
}
