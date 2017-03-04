using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
        /// <param name="ratio">缩放的倍率，值域[0.0, +∞]，原始尺寸对应于1.0</param>
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

        /// <summary>
        /// 获取屏幕分区的中心坐标
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 4]，其中2是屏幕纵向正中</param>
        /// <param name="c">区块的纵向编号，值域[0, 16]，其中0是屏幕横向正中</param>
        /// <returns>块的中心坐标</returns>
        public static Point GetScreenCoordination(int r, int c)
        {
            return SCamera.ScreenPointMap[r, c];
        }

        /// <summary>
        /// 获取角色分区的中心坐标
        /// </summary>
        /// <param name="r">区块的横向编号，值域[0, 11]</param>
        /// <returns>块的中心坐标</returns>
        public static Point GetCharacterCoordination(int r)
        {
            return SCamera.CharacterPointMap[r];
        }

        /// <summary>
        /// 初始化镜头系统，必须在使用场景镜头系统前调用它
        /// </summary>
        public static void Init()
        {
            // 提前计算所有中心点坐标，避免每次调用的重复计算
            SCamera.ScreenPointMap = new Point[GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT, GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT];
            var ScrBlockWidth = (double)GlobalDataContainer.GAME_WINDOW_WIDTH / GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT;
            var ScrBlockHeight = (double)GlobalDataContainer.GAME_WINDOW_HEIGHT / GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT;
            var beginX = 0.0 - GlobalDataContainer.GAME_SCAMERA_SCR_SINGLEBLOODCOLCOUNT * ScrBlockWidth + ScrBlockWidth / 2.0;
            var beginY = ScrBlockHeight / 2.0;
            for (int i = 0; i < GlobalDataContainer.GAME_SCAMERA_SCR_ROWCOUNT; i++)
            {
                for (int j = 0; j < GlobalDataContainer.GAME_SCAMERA_SCR_COLCOUNT; j++)
                {
                    SCamera.ScreenPointMap[i, j] = new Point(beginX + j * ScrBlockWidth, beginY + i * ScrBlockHeight);
                }
            }
            SCamera.CharacterPointMap = new Point[GlobalDataContainer.GAME_SCAMERA_CSTAND_ROWCOUNT];
            var CstBlockHeight = (double)GlobalDataContainer.GAME_SCAMERA_CSTAND_HEIGHT / GlobalDataContainer.GAME_SCAMERA_CSTAND_ROWCOUNT;
            var beginCstY = 0.0 - ((double)GlobalDataContainer.GAME_SCAMERA_CSTAND_HEIGHT / 2.0) + CstBlockHeight / 2.0;
            for (int i = 0; i < GlobalDataContainer.GAME_SCAMERA_CSTAND_ROWCOUNT; i++)
            {
                SCamera.CharacterPointMap[i] = new Point(0, beginCstY + i * CstBlockHeight);
            }
        }
        
        /// <summary>
        /// 屏幕分块中心绝对坐标字典
        /// </summary>
        private static Point[,] ScreenPointMap;

        /// <summary>
        /// 立绘分块中心相对坐标字典
        /// </summary>
        private static Point[] CharacterPointMap;
    }
}
