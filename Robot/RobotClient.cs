using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Server.Protocol;
using Server;
using Server.Logic;
using Server.Tools;

namespace Robot
{
    public class RobotClient
    {

        private TCPClient _tcpClient = null;
        public Point CurrentGrid;
        public Dircetions CurrentDir;
        public int UserID = 0;
        public int RoleID = 0;
        public int Token = 0;
        public string UserName;
        public int MapCode = 0;

        public int MapSize
        {
            set
            {
                totalGridXNum = value;
                totalGridYNum = value;
            }
        }

        static Random Rand = new Random();
        int totalGridXNum ;
        int totalGridYNum ;
        private int gridX;
        private int gridY;
        bool LoadRoleOK = false;


        /// 是否是主动断开的连接
        private bool ActiveDisconnect = false;

        //定义通讯连接通知事件
        private void SocketConnect(object sender, SocketConnectEventArgs e)
        {
            switch ((NetSocketTypes)e.NetSocketType)
            {
                case NetSocketTypes.SOCKET_CONN:
                    {
                        //判断是否连接成功
                        if (e.Error == "Success")
                        {

                            //获取用户ID和用户Token
                            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", UserID, UserName, Token, 12344, (int)TCPCmdProtocolVer.VerSign, 1);

                            //连接成功, 立即发送请求登陆的指令
                            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)TCPGameServerCmds.CMD_LOGIN_ON));
                        }
                        else
                        {
                            //设置主动断开连接
                            ActiveDisconnect = true;
                            string errMsg = string.Format("连接游戏务器失败");
                            e.ErrorMsg = errMsg;
                            e.ReturnStartPage = false;
                            e.ShowMsgBox = false;
                        }
                        break;
                    }
                case NetSocketTypes.SOCKET_SEND:
                    {
                        //设置主动断开连接
                        ActiveDisconnect = true;
                        string errMsg = string.Format("与游戏服务器通讯失败");
                        e.ErrorMsg = errMsg;
                        e.ReturnStartPage = false;
                        e.ShowMsgBox = true;

                        break;
                    }
                case NetSocketTypes.SOCKET_RECV:
                    {
                        break;
                    }
                case NetSocketTypes.SOCKET_CLOSE:
                    {
                        //如果不是主动断开
                        if (!ActiveDisconnect)
                        {
                            string errMsg = string.Format("亲爱的玩家，你暂时与服务器断开了连接。请放心，我们已经保存了您的数据，请重新登录！");
                            e.ErrorMsg = errMsg;
                            e.ReturnStartPage = true;
                            e.ShowMsgBox = true;
                        }
                        break;
                    }
                case NetSocketTypes.SOCKT_CMD:
                    {
                        SocketCommand(this, e);
                        break;
                    }
                default:
                    {
                        //设置主动断开连接
                        ActiveDisconnect = true;
                        throw new Exception("错误的Socket操作类型");
                    }
            }
        }

        RoleData roleData = null;
        private void SocketCommand(object sender, SocketConnectEventArgs e)
        {
            //MainWindow.GetInstance().ShowText(string.Format("{0}: recv cmd {1}", UserName, e.CmdID));
            lastRecvTicks = DateTime.Now.Ticks/10000;

            if (e.CmdID == (int)(TCPGameServerCmds.CMD_ROLE_LIST))
            {
                if (e.fields.Length > 1 && Convert.ToInt32(e.fields[0]) > 0)
                {
                    string[] roles = e.fields[1].Split('|');

                    string[] temp = roles[0].Split('$');

                    RoleID = (Convert.ToInt32(temp[0]));
                    InitPlayGame();

                }
                else
                {
                    CreateRole();
                }
            }
            else if (e.CmdID == (int)TCPGameServerCmds.CMD_CREATE_ROLE)
            {
                string[] temp = e.fields[1].Split('$');
                RoleID = (Convert.ToInt32(temp[0]));
                InitPlayGame();
            }
            else if (e.CmdID == (int)TCPGameServerCmds.CMD_INIT_GAME)
            {
                roleData = DataHelper.BytesToObject<RoleData>(e.bytesData, 0, e.bytesData.Length);
                StartPlayGame();
            }
            else if (e.CmdID == (int)TCPGameServerCmds.CMD_PLAY_GAME)
            {

                if (roleData.MapCode != MapCode)
                {
                    SpriteMapConversion();
                }

                LoadRoleOK = true;
                GMF5();

            }
            else if (e.CmdID == (int)TCPGameServerCmds.CMD_SPR_MAPCHANGE)
            {
                LoadRoleOK = true;
                GMF5();
            }
        }

        void GMF5()
        {
            string text = string.Format("-addattack {0} {1}", UserName, 1000);
            SpriteSendChat(0, "", "", text);
            text = string.Format("-adddefense {0} {1}", UserName, 500000);
            SpriteSendChat(0, "", "", text);
        }

        bool InitOK = false;
        public void Connect(int id, int token, string ip, int port)
        {
            UserID = id;
            Token = token;
            UserName = "Robot" + id;
            LoadRoleOK = false;
            //进行移动的操作
            gridX = Rand.Next(1, totalGridXNum);
            gridY = Rand.Next(1, totalGridYNum);
            CurrentGrid = new Point(gridX, gridY);
            CurrentDir = (Dircetions)Rand.Next(0, 8);

            _tcpClient = new TCPClient();

            //添加事件处理
            _tcpClient.SocketConnect += SocketConnect;
            try
            {
                //先建立连接
                _tcpClient.Connect(ip, port);
            }
            catch (Exception e)
            {
                MainWindow.GetInstance().ShowText(e.ToString());
            }
            //先关闭连接
            ActiveDisconnect = false;

            GetRoleList();
        }


        public void Disconnect()
        {
            ActiveDisconnect = true;
            _tcpClient.Disconnect();
        }

        long lastTicks = DateTime.Now.Ticks / 10000;
        long lastMoveTicks = DateTime.Now.Ticks / 10000;
        long lastHeartTicks = DateTime.Now.Ticks / 10000;
        long lastSkillTicks = DateTime.Now.Ticks / 10000;
        private long lastRecvTicks = DateTime.Now.Ticks/10000;
        private int GridSize = 50;
        private int dir = 0;
        string pathStr = "";

        /// 进行测试
        public void RunTest(long ticks)
        {
            if (!LoadRoleOK)
            {
                return;
            }
            if (ticks - lastMoveTicks >= 500)
            {
                lastMoveTicks = ticks;
                if (CurrentGrid.X >= totalGridXNum || CurrentGrid.X <= 0)
                {
                    gridX = Rand.Next(0, totalGridXNum);
                    gridY = Rand.Next(0, totalGridYNum);
                    CurrentGrid = new Point(gridX, gridY);
                }

                if (CurrentGrid.Y >= totalGridYNum || CurrentGrid.Y <= 0)
                {
                    gridX = Rand.Next(0, totalGridXNum);
                    gridY = Rand.Next(0, totalGridYNum);
                    CurrentGrid = new Point(gridX, gridY);
                }

                Point oldGrid = this.CurrentGrid;
                ChuanQiUtils.RunTo(this, (Dircetions)dir, out pathStr);
                

                Point from = new Point(oldGrid.X * GridSize + GridSize / 2, oldGrid.Y * GridSize + GridSize / 2);
                Point to = new Point(CurrentGrid.X * GridSize + GridSize / 2, CurrentGrid.Y * GridSize + GridSize / 2);
                SpriteMoveTo(from, to, (int)GActions.ReadyWalk, 0, pathStr);
                MainWindow.GetInstance().ShowText($"{UserName}: move to {CurrentGrid.X}_{CurrentGrid.Y}");
            }

            if (ticks - lastTicks >= 5000)
            {
                lastTicks = ticks;
                SpriteHeart();
                dir = Rand.Next(0, 8);

            }

            if (ticks - lastHeartTicks >= 2000)
            {
                lastHeartTicks = ticks;

                Point to = new Point(CurrentGrid.X * GridSize + GridSize / 2, CurrentGrid.Y * GridSize + GridSize / 2);
                SpritePosition(to);
            }

            if (ticks - lastSkillTicks >= 3000)
            {
                lastSkillTicks = ticks;
                Point to = new Point(CurrentGrid.X * GridSize + GridSize / 2, CurrentGrid.Y * GridSize + GridSize / 2);

                SpriteAction(dir, 9, to, to, 90, new Point(0, 0));
                SpriteAttack(to, -1, to, new Point(-1, -1), 202001, dir, 1);

            }

            if (ticks - lastRecvTicks > 5000)
            {
                MainWindow.GetInstance().ShowText($"{UserName}: 超过5秒没有收到消息！");
            }

        }


        /// 将原来的字符串=>字节=>压缩=>base64
        public static string ZipStringToBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            byte[] bytes = new UTF8Encoding().GetBytes(text);
            return Convert.ToBase64String(bytes);

        }

        public enum ChatType
        {
            TextOrSymbol = 0, //文字或表情
            Voice = 1, //语音
        }
        #region 网络命令
        public void SpriteSendChat(int index, string fromRoleName, string toRoleName, string text, ChatType chatType = ChatType.TextOrSymbol)
        {
            if (ActiveDisconnect)
            {
                return;
            }
            string strcmd = "";
            strcmd = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}:{6}", RoleID, fromRoleName, 0, toRoleName, index, text, (int)chatType);
            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)(TCPGameServerCmds.CMD_SPR_CHAT)));
        }

        public void SpriteMoveTo(Point from, Point to, int action, int extAction, string zipPathString)
        {
            zipPathString = ZipStringToBase64(zipPathString);
            _tcpClient.SendData(DataHelper.ObjectToTCPOutPacket(
                new SpriteMoveData(RoleID, MapCode, action, (int)(to.X), (int)(to.Y), extAction, (int)(from.X), (int)(from.Y), DateTime.Now.Ticks / 10000, zipPathString),
                (int)(TCPGameServerCmds.CMD_SPR_MOVE)));
        }

        public void SpriteHeart()
        {
            String strcmd = "";
            strcmd = string.Format("{0}:{1}:{2}", RoleID, Token, DateTime.Now.Ticks / 10000);
            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)(TCPGameServerCmds.CMD_SPR_CLIENTHEART)));
        }

        public void SpritePosition(Point to)
        {
            _tcpClient.SendData(DataHelper.ObjectToTCPOutPacket<SpritePositionData>(
                new SpritePositionData(RoleID, MapCode, (int)(to.X), (int)(to.Y), DateTime.Now.Ticks / 10000),
                (int)(TCPGameServerCmds.CMD_SPR_POSITION)));
        }

        public void GetRoleList()
        {
            if (ActiveDisconnect)
            {
                return;
            }
            string strcmd = "";
            strcmd = string.Format("{0}:{1}", UserID, 1);
            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)(TCPGameServerCmds.CMD_ROLE_LIST)));
        }

        public void CreateRole()
        {
            if (ActiveDisconnect)
            {
                return;
            }
            string strcmd = "";
            strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", UserID, UserName, 1, 1, UserName, 1);
            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)(TCPGameServerCmds.CMD_CREATE_ROLE)));
        }


        public void InitPlayGame()
        {
            if (ActiveDisconnect)
            {
                return;
            }
            string strcmd = "";
            strcmd = string.Format("{0}:{1}", UserID, RoleID);
            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)(TCPGameServerCmds.CMD_INIT_GAME)));
        }


        public void StartPlayGame()
        {
            if (ActiveDisconnect)
            {
                return;
            }
            string strcmd = "";
            strcmd = string.Format("{0}", RoleID);
            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)(TCPGameServerCmds.CMD_PLAY_GAME)));
        }

        public void SpriteMagicCode(int magicCode)
        {
            if (ActiveDisconnect)
            {
                return;
            }
            string strcmd = "";
            SpriteMagicCodeData magicCodeData = new SpriteMagicCodeData();

            magicCodeData.roleID = RoleID;
            magicCodeData.mapCode = 10100;
            magicCodeData.magicCode = magicCode;
            _tcpClient.SendData(DataHelper.ObjectToTCPOutPacket<SpriteMagicCodeData>(magicCodeData, (int)(TCPGameServerCmds.CMD_SPR_MAGICCODE)));
        }

        public void SpriteAction(double direction, int action, Point to, Point targetPos, int yAngle, Point moveTo)
        {
            if (ActiveDisconnect)
            {
                return;
            }

            string strcmd = "";
            _tcpClient.SendData(DataHelper.ObjectToTCPOutPacket<SpriteActionData>(new SpriteActionData(RoleID, MapCode, (int)(direction), action, (int)(to.X), (int)(to.Y), (int)(targetPos.X), (int)(targetPos.Y), yAngle, (int)moveTo.X, (int)moveTo.Y), (int)(TCPGameServerCmds.CMD_SPR_ACTTION)));
        }

        public void SpriteAttackAction(double direction, int action, Point to, Point targetPos, int yAngle, Point moveTo)
        {
            if (ActiveDisconnect)
            {
                return;
            }

            string strcmd = "";
            _tcpClient.SendData(DataHelper.ObjectToTCPOutPacket<SpriteActionData>(new SpriteActionData(RoleID, MapCode, (int)(direction), action, (int)(to.X), (int)(to.Y), (int)(targetPos.X), (int)(targetPos.Y), yAngle, (int)moveTo.X, (int)moveTo.Y), (int)(TCPGameServerCmds.CMD_SPR_ATTACK_ACTION)));
        }
        public void SpriteAttack(Point selfPos, int enemy, Point enemyPos, Point realEnemyPos, int magicCode, int director, int magicLevel)
        {
            if (ActiveDisconnect)
            {
                return;
            }
            SpriteAttackData attackData = new SpriteAttackData();
            attackData.roleID = RoleID;
            attackData.roleX = (int)(selfPos.X);
            attackData.roleY = (int)(selfPos.Y);
            attackData.enemy = enemy;
            attackData.enemyX = (int)(enemyPos.X);
            attackData.enemyY = (int)(enemyPos.Y);
            attackData.realEnemyX = (int)(realEnemyPos.X);
            attackData.realEnemyY = (int)(realEnemyPos.Y);
            attackData.magicCode = magicCode;
            attackData.director = director;
            attackData.magicLevel = magicLevel;

            _tcpClient.SendData(DataHelper.ObjectToTCPOutPacket<SpriteAttackData>(attackData, (int)(TCPGameServerCmds.CMD_SPR_ATTACK)));

        }
        public void SpriteMapConversion()
        {
            if (ActiveDisconnect)
            {
                return;
            }

            string strcmd = "";
            strcmd = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", RoleID, -1, MapCode, -1, -1, 0);
            _tcpClient.SendData(TCPOutPacket.MakeTCPOutPacket(strcmd, (int)(TCPGameServerCmds.CMD_SPR_MAPCHANGE)));
        }

        #endregion 

    }
}
