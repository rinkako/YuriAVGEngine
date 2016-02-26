using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuriHalation.ScriptPackage
{
    /// <summary>
    /// 场景包装类
    /// </summary>
    [Serializable]
    class ScenePackage
    {
        /// <summary>
        /// 场景的名称
        /// </summary>
        public string sceneName = "";

        /// <summary>
        /// 函数向量
        /// </summary>
        public List<FunctionPackage> funcList = new List<FunctionPackage>();

        /// <summary>
        /// 场景动作向量
        /// </summary>
        public List<ActionPackage> mainAPList = new List<ActionPackage>();

        /// <summary>
        /// 场景内变量向量
        /// </summary>
        public List<VariablePackage> localVariableList = new List<VariablePackage>();

        /// <summary>
        /// 增加一个动作
        /// </summary>
        /// <param name="ap">动作包装</param>
        /// <param name="insertLine">插入的行</param>
        /// <returns>操作成功与否</returns>
        public bool AddAction(ActionPackage ap, int insertLine = -1)
        {
            if (insertLine < 0)
            {
                this.mainAPList.Add(ap);
            }
            else if (insertLine >= 0 && insertLine < this.mainAPList.Count)
            {
                this.mainAPList.Insert(insertLine, ap);
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 增加一个函数
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <returns>操作成功与否</returns>
        public bool AddFunction(string funcName)
        {
            if (this.funcList.Find((x) => x.functionName == funcName) != null)
            {
                return false;
            }
            this.funcList.Add(new FunctionPackage() { functionName = funcName });
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
            if ((varItem = this.localVariableList.Find((x) => x.varName == varName)) != null)
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
            this.localVariableList.Add(varItem);
            return true;
        }

        /// <summary>
        /// 字符串化方法
        /// </summary>
        /// <returns>场景的名字和行数</returns>
        public override string ToString()
        {
            return String.Format("Scene: {0} ({1} lines, {2} funcs)",
                this.sceneName, this.mainAPList.Count, funcList.Count);
        }
    }
}
