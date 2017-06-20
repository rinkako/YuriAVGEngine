using Yuri.Utils;

namespace Yuri.PlatformCore.VM
{
    /// <summary>
    /// 持久类DAO：为游戏提供持久性上下文包装，它不会被回滚和存读档影响
    /// </summary>
    internal static class PersistContextDAO
    {
        /// <summary>
        /// 保存持久上下文到稳定储存器
        /// </summary>
        public static void SaveToSteadyMemory() => PersistContextDAO.PersistenceContext.SaveToSteadyMemory(IOUtils.ParseURItoURL(GlobalConfigContext.PersistenceFileName));

        /// <summary>
        /// 从稳定储存器将持久上下文读入内存
        /// </summary>
        public static void LoadFromSteadyMemory() => PersistContextDAO.PersistenceContext.LoadFromSteadyMemory(IOUtils.ParseURItoURL(GlobalConfigContext.PersistenceFileName));

        /// <summary>
        /// 将一个变量放入持久上下文中，如果指定变量名已存在，就覆盖原来的对象
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="varObj">要存入的对象</param>
        public static void Assign(string varName, object varObj) => PersistContextDAO.PersistenceContext.Assign(varName, varObj);

        /// <summary>
        /// 从持久上下文中移除一个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>是否移除成功（变量原本是否存在）</returns>
        public static bool Remove(string varName) => PersistContextDAO.PersistenceContext.Remove(varName);

        /// <summary>
        /// 查找持久上下文中是否存在某个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量是否存在</returns>
        public static bool Exist(string varName) => PersistContextDAO.PersistenceContext.Exist(varName);

        /// <summary>
        /// 从持久上下文中取一个变量
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <returns>变量的引用</returns>
        public static object Fetch(string varName) => PersistContextDAO.PersistenceContext.Fetch(varName);

        /// <summary>
        /// 持久性上下文对象
        /// </summary>
        public static readonly PersistContext PersistenceContext = new PersistContext();
    }
}
