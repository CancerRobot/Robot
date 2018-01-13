using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using System.Windows.Threading;
using Robot;
using Server.Logic;

namespace Server
{
    public enum NetSocketTypes { SOCKET_CONN = 0, SOCKET_SEND, SOCKET_RECV, SOCKET_CLOSE, SOCKT_CMD }

    /// 与服务器端通信连接参数
    public class SocketConnectEventArgs : EventArgs
    {
        /// 远程地址
        public string RemoteEndPoint;

        /// Socket连接错误码
        public string Error;

        /// 内部错误描述信息
        public string ErrorStr;

        /// 错误描述信息
        public string ErrorMsg;

        /// 是否回到起始页面
        public bool ReturnStartPage = false;

        /// 是否显示错误对话框
        public bool ShowMsgBox = false;

        /// 网络通讯的操作类型
        public int NetSocketType;

        /// 网络命令
        public int CmdID = -1;

        /// 网络通讯字段
        public string[] fields = null;

        /// 二进制格式返回数据
        public byte[] bytesData = null;
    }

    public delegate void SocketConnectEventHandler(object sender, SocketConnectEventArgs e);

    /// TCP连接DB服务器端的客户端类
    public class TCPClient
    {
        private TCPInPacket _MyTCPInPacket = null;
        private TCPInPacket MyTCPInPacket
        {
            get { return _MyTCPInPacket; }
        }

        private long _LastSendDataTicks = 0;
        public long LastSendDataTicks
        {
            get { return _LastSendDataTicks; }
        }

        private bool _Connected = false;
        public bool Connected
        {
            get { return _Connected; }
            set { _Connected = value; }
        }

        /// 互斥锁对象
        private object MutexSocket = new object();

        /// 连接成功的Socket
        private Socket _Socket = null;


        /// 在ListBox中的索引值
        public int ListIndex
        {
            get;
            set;
        }

        public TCPClient()
        {
            _MyTCPInPacket = new TCPInPacket((int)TCPCmdPacketSize.MAX_SIZE);
            _MyTCPInPacket.TCPCmdPacketEvent += TCPCmdPacketEvent;

        }

        //定义通知事件
        public event SocketConnectEventHandler SocketConnect;

        /// 连接服务器
        public void Connect(string ip, int port)
        {
            lock (MutexSocket)
            {
                if (null != _Socket) return; //已经连接

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _Socket.ReceiveTimeout = 1;

                try
                {
                    _Socket.Connect(remoteEndPoint);
                    if (_Socket.Connected)
                    {
                        byte[] bytesData = new byte[(int)TCPCmdPacketSize.MAX_SIZE];
                        _Socket.BeginReceive(bytesData, 0, bytesData.Length, SocketFlags.None, new AsyncCallback(SocketReceived), bytesData);
                    }

                }
                catch (Exception)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("{0}, 与GameServer: {1}:{2}建立连接失败", ListIndex, ip, port));
                    _Socket = null;
                    throw; //继续抛出异常
                }

            }
        }

        /// 断开与服务器的连接
        public void Disconnect()
        {
            lock (MutexSocket)
            {
                if (null == _Socket) return; //无连接
                if (_Socket.Connected)
                {
                    _Socket.Shutdown(SocketShutdown.Both);
                }
                _Socket.Close();

                _Socket = null;
            }
        }

        /// 是否还在连接中
        public bool IsConnected()
        {
            bool ret = false;
            lock (MutexSocket)
            {
                ret = (null != _Socket);
            }

            return ret;
        }

        private bool TCPCmdPacketEvent(object sender)
        {
            TCPInPacket tcpInPacket = sender as TCPInPacket;
            Socket s = tcpInPacket.CurrentSocket;

            //接收到了完整的命令包
            bool ret = false;
            ret = TCPCmdHandler.ProcessServerCmd(this, tcpInPacket.PacketCmdID, tcpInPacket.GetPacketBytes(), tcpInPacket.PacketDataSize);
            return ret;
        }

        public void NotifyRecvData(SocketConnectEventArgs e)
        {
            if (null != SocketConnect)
            {
                SocketConnect(this, e);
            }
        }

        /// 发送数据
        public byte[] SendData(TCPOutPacket tcpOutPacket)
        {
            lock (MutexSocket)
            {
                if (null == _Socket) return null; //还没连接

                try
                {
                    //字节排序
                    DataHelper.SortBytes(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);

                    //将数据发送给对方
                    _Socket.Send(tcpOutPacket.GetPacketBytes(), tcpOutPacket.PacketDataSize, SocketFlags.None);
                }
                catch (Exception)
                {
                    //断开连接
                    Disconnect();

                    LogManager.WriteLog(LogTypes.Error, string.Format("{0}, 与GameServer: {1}通讯失败", ListIndex, Global.GetSocketRemoteEndPoint(_Socket)));

                }

                return null;
            }
        }

        private void SocketReceived(IAsyncResult iar)
        {
            if (null == _Socket)
            {
                return;
            }
            try
            {
                SocketError socketError = SocketError.Success;
                int recvLength = _Socket.EndReceive(iar, out socketError);
                byte[] bytesData = iar.AsyncState as byte[];

                if (recvLength <= 0)
                {
                    return;
                }
                //处理收到的包
                if (!_MyTCPInPacket.WriteData(bytesData, 0, recvLength))
                {
                    //TODO 处理包异常通知
                    return;
                }

                if (_Socket.Connected)
                {
                    _Socket.BeginReceive(bytesData, 0, bytesData.Length, SocketFlags.None, new AsyncCallback(SocketReceived), bytesData);
                }
            }
            catch (Exception e)
            {
                MainWindow.GetInstance().ShowText(e.ToString());
            }

    }
    }
}
