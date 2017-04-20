using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Yuri.Utils;
using Yuri.Yuriri;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// <para>符号表类：维护运行时环境的用户符号</para>
    /// <para>她是一个单例类，只有唯一实例</para>
    /// </summary>
    internal sealed class SymbolTable : ForkableState
    {
        /// <summary>
        /// 为场景添加符号表
        /// </summary>
        /// <param name="scene">场景实例</param>
        /// <returns>操作成功与否</returns>
        public bool AddSceneSymbolContext(Scene scene)
        {
            if (this.sceneCtxDao.ExistScene(scene.Scenario))
            {
                return false;
            }
            this.sceneCtxDao.CreateSceneContext(scene.Scenario);
            return true;
        }

        /// <summary>
        /// 测试一个符号是否为开关表达式
        /// </summary>
        /// <param name="parStr">测试字符串</param>
        /// <returns>是否是系统开关操作</returns>
        public static bool IsSwitchExpression(string parStr)
        {
            return SymbolTable.switchRegex.IsMatch(parStr);
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

        }
        
        /// <summary>
        /// 场景变量DAO
        /// </summary>
        private SceneContextDAO sceneCtxDao = null;

        /// <summary>
        /// 全局变量DAO
        /// </summary>
        private GlobalContextDAO globalCtxDao = null;

        /// <summary>
        /// 全局临时变量DAO
        /// </summary>
        private SimpleContext globalTempCtxDao = null;

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static SymbolTable synObject = null;
        
        /// <summary>
        /// 开关正则式子
        /// </summary>
        private static readonly Regex switchRegex = new Regex(@"^switches{\d+}$");
    }
}