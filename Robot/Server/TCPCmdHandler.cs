using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Server.Logic;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace Server
{
    public enum TCPProcessCmdResults { RESULT_OK = 0, RESULT_FAILED = 1, RESULT_DATA = 2 };

    /// 处理收到的TCP协议命令
    class TCPCmdHandler
    {
        /// SHA1密码
        public static string KeySHA1 = "abcde";
        /// 数据加密密码
        public static string KeyData = "12345";
        /// Web加密密码
        public static string WebKey = "12345";        

        #region 与登陆服务器端的通讯处理

        /// 处理与登陆服务服务器的网络协议命令
        public static bool ProcessServerCmd(TCPClient client, int nID, byte[] data, int count)
        {

            bool ret = false;
            switch (nID)
            {
                case (int)(TCPGameServerCmds.CMD_LOGIN_ON):
                case (int)(TCPGameServerCmds.CMD_ROLE_LIST):
                case (int)(TCPGameServerCmds.CMD_CREATE_ROLE):
                case (int)(TCPGameServerCmds.CMD_SYNC_TIME):
                case (int)(TCPGameServerCmds.CMD_PLAY_GAME):
                case (int)(TCPGameServerCmds.CMD_SPR_MOVEEND):
                case (int)(TCPGameServerCmds.CMD_SPR_POSITION):
                case (int)(TCPGameServerCmds.CMD_LOG_OUT):
                    {
                        ret = ProcessGameCmd(client, nID, data, count);
                        break;
                    }

                case (int)(TCPGameServerCmds.CMD_INIT_GAME):
                case (int)(TCPGameServerCmds.CMD_SPR_MOVE):
                case (int)(TCPGameServerCmds.CMD_SPR_ACTTION):
                case (int)(TCPGameServerCmds.CMD_SPR_INJURE):
                case (int)(TCPGameServerCmds.CMD_SPR_ATTACK):
                case (int)(TCPGameServerCmds.CMD_SPR_MAGICCODE):
                    {
                        ret = ProcessGameStreamCmd(client, nID, data, count);
                        break;
                    }
                default:
                    {
                        ret = true;
                        break;
                    }
            }

            return ret;
        }


        /// 处理游戏服务器命令
        private static bool ProcessGameCmd(TCPClient client, int nID, byte[] data, int count)
        {
            string strData = new UTF8Encoding().GetString(data, 0, count);

            //解析客户端的指令
            string[] fields = strData.Split(':');

            //通知外部
            client.NotifyRecvData(new SocketConnectEventArgs()
            {
                Error = "Success",
                NetSocketType = (int)NetSocketTypes.SOCKT_CMD,
                CmdID = (int)nID,
                fields = fields
            });
            return true;
        }

        /// 处理游戏服务器命令(返回二进制格式)
        private static bool ProcessGameStreamCmd(TCPClient client, int nID, byte[] data, int count)
        {
            byte[] bytesData = new byte[count];
            DataHelper.CopyBytes(bytesData, 0, data, 0, count);

            //通知外部
            client.NotifyRecvData(new SocketConnectEventArgs()
            {
                Error = "Success",
                NetSocketType = (int)NetSocketTypes.SOCKT_CMD,
                CmdID = (int)nID,
                fields = null,
                bytesData = bytesData,
            });
            return true;
        }

        #endregion 

    }
}
