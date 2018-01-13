using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Server.Tools;

namespace Robot
{
    /// 方向定义
    public enum Dircetions
    {
        DR_UP = 0,
        DR_UPRIGHT = 1,
        DR_RIGHT = 2,
        DR_DOWNRIGHT = 3,
        DR_DOWN = 4,
        DR_DOWNLEFT = 5,
        DR_LEFT = 6,
        DR_UPLEFT = 7,
    }

    /// 魔方游戏基本单元，GameClient 和 Monster 都会使用它
    /// 由它统一实现模仿传奇功能的函数
    public class ChuanQiUtils
    {
        /// 提取向某个方向移动一个位置得到的新xy坐标值
        protected static void WalkNextPos(RobotClient obj, Dircetions nDir, out int nX, out int nY)
        {
            Point grid = obj.CurrentGrid;
            int nCurrX = (int)grid.X;
            int nCurrY = (int)grid.Y;

            nX = nCurrX;
            nY = nCurrY;

	        switch (nDir)
	        {
                case Dircetions.DR_UP:
			        nX = nCurrX;
			        nY = nCurrY - 1;
			        break;
                case Dircetions.DR_UPRIGHT:
			        nX = nCurrX + 1;
			        nY = nCurrY - 1;
			        break;
                case Dircetions.DR_RIGHT:
			        nX = nCurrX + 1;
			        nY = nCurrY;
			        break;
                case Dircetions.DR_DOWNRIGHT:
			        nX = nCurrX + 1;
			        nY = nCurrY + 1;
			        break;
                case Dircetions.DR_DOWN:
			        nX = nCurrX;
			        nY = nCurrY + 1;
			        break;
                case Dircetions.DR_DOWNLEFT:
			        nX = nCurrX - 1;
			        nY = nCurrY + 1;
			        break;
                case Dircetions.DR_LEFT:
			        nX = nCurrX - 1;
			        nY = nCurrY;
			        break;
                case Dircetions.DR_UPLEFT:
			        nX = nCurrX - 1;
			        nY = nCurrY - 1;
			        break;
	        }
        }

        /// 向某个方向移动一个格子位置
        /// 移动可能失败,失败原因 1.相关位置不可走 2.相关位置已经有其他角色或者怪物
        public static Boolean WalkTo(RobotClient obj, Dircetions nDir, out string pathStr)
        {
            pathStr = "";
	        int nX, nY;
	        WalkNextPos(obj, nDir, out nX, out nY);

            Point grid = obj.CurrentGrid;
            int nCurrX = (int)grid.X;
            int nCurrY = (int)grid.Y;

            pathStr = String.Format("{0}_{1}|{2}_{3}", nCurrX, nCurrY, nX, nY);
            Boolean fResult = WalkXY(obj, nX, nY, nDir, pathStr);

	        if (fResult)
	        {
                //旧传奇代码这儿是隐藏设置，可能用于隐藏魔法
	        }

	        return fResult;
        }

        /// 向某个方向移动2个格子位置
        /// 移动可能失败,失败原因 1.相关位置不可走 2.相关位置已经有其他角色或者怪物
        public static Boolean RunTo(RobotClient obj, Dircetions nDir, out string pathStr)
        {
            pathStr = "";
            Point grid = obj.CurrentGrid;
            int nCurrX = (int)grid.X;
            int nCurrY = (int)grid.Y;

	        int nX = nCurrX, nY = nCurrY;
	        int nWalk = 2;

            pathStr = String.Format("{0}_{1}", nCurrX, nCurrY); ;

            //不考虑坐骑速度
	        for (int i = 0; i < nWalk; i++)
	        {
		        switch (nDir)
		        {
                    case Dircetions.DR_UP:
				        nY--;
				        break;
                    case Dircetions.DR_UPRIGHT:
				        nX++;
				        nY--;
				        break;
                    case Dircetions.DR_RIGHT:
				        nX++;
				        break;
                    case Dircetions.DR_DOWNRIGHT:
				        nX++;
				        nY++;
				        break;
                    case Dircetions.DR_DOWN:
				        nY++;
				        break;
                    case Dircetions.DR_DOWNLEFT:
				        nX--;
				        nY++;
				        break;
                    case Dircetions.DR_LEFT:
				        nX--;
				        break;
                    case Dircetions.DR_UPLEFT:
				        nX--;
				        nY--;
				        break;
		        }

                pathStr += String.Format("|{0}_{1}", nX, nY);
	        }

            return RunXY(obj, nX, nY, nDir, pathStr); 
        }

        /// 走动到新的位置
        protected static Boolean WalkXY(RobotClient obj, int nX, int nY, Dircetions nDir, String pathStr)
        {
            Point grid = obj.CurrentGrid;
            int nCurrX = (int)grid.X;
            int nCurrY = (int)grid.Y;
	        
            obj.CurrentGrid = new Point(nX, nY);
            obj.CurrentDir = nDir;

	        return true;
        }

        /// 跑到XY坐标，计算时已经验证可移动，这儿不需要再次进行可否移动的判断
        protected static Boolean RunXY(RobotClient obj, int nX, int nY, Dircetions nDir, String pathStr)
        {
            Point grid = obj.CurrentGrid;
            int nCurrX = (int)grid.X;
            int nCurrY = (int)grid.Y;

            obj.CurrentGrid = new Point(nX, nY);
            obj.CurrentDir = nDir;

            //进行九宫格通知
            return true;
        }
    }
}
