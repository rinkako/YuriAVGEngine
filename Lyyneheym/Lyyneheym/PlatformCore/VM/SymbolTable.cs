using System;
using System.Text.RegularExpressions;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// <para>符号表类：维护运行时环境的用户符号</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    [Serializable]
    internal sealed class SymbolTable
    {
        /// <summary>
        /// 为符号表设置新的DAO
        /// </summary>
        /// <param name="sceneDao">场景上下文DAO</param>
        /// <param name="globalDao">全局变量上下文DAO</param>
        public void SetDAO(SceneContextDAO sceneDao, GlobalContextDAO globalDao)
        {
            this.SceneCtxDao = sceneDao;
            this.GlobalCtxDao = globalDao;
        }

        /// <summary>
        /// 为符号表设置临时上下文
        /// </summary>
        /// <param name="tempCtx">临时上下文对象</param>
        public void SetTemporaryDAO(SimpleContext tempCtx)
        {
            this.GlobalTempCtxDao = tempCtx;
        }

        /// <summary>
        /// 为场景添加符号表
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <returns>操作成功与否</returns>
        public bool AddSceneSymbolContext(Scene scene)
        {
            if (this.SceneCtxDao.ExistScene(scene.Scenario))
            {
                return false;
            }
            this.SceneCtxDao.CreateSceneContext(scene.Scenario);
            return true;
        }

        /// <summary>
        /// 测试一个符号是否为开关表达式
        /// </summary>
        /// <param name="parStr">测试字符串</param>
        /// <returns>是否是系统开关操作</returns>
        public static bool IsSwitchExpression(string parStr)
        {
            return Regex.IsMatch(parStr, @"^switches{\d+}$");
        }

        /// <summary>
        /// 为符号表重新设置唯一实例的引用
        /// </summary>
        /// <param name="st">新符号表</param>
        public static void ResetSynObject(SymbolTable st)
        {
            SymbolTable.synObject = st;
        }
        
        /// <summary>
        /// 工厂方法：获得类的唯一实例
        /// </summary>
        /// <returns>符号表管理器</returns>
        public static SymbolTable GetInstance()
        {
            return SymbolTable.synObject ?? (SymbolTable.synObject = new SymbolTable());
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        private SymbolTable()
        {
            this.SceneCtxDao = new SceneContextDAO();
            this.GlobalCtxDao = new GlobalContextDAO();
            this.GlobalTempCtxDao = new SimpleContext("&__YuriTemporary");
        }
        
        /// <summary>
        /// 获取场景变量DAO
        /// </summary>
        public SceneContextDAO SceneCtxDao { get; private set; }

        /// <summary>
        /// 获取全局变量DAO
        /// </summary>
        public GlobalContextDAO GlobalCtxDao { get; private set; }

        /// <summary>
        /// 获取全局临时变量DAO
        /// </summary>
        public SimpleContext GlobalTempCtxDao { get; private set; }

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static SymbolTable synObject = null;
    }
}