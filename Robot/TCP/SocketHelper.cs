using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Server.TCP
{
    /// 连接成功通知函数
    public delegate void SocketConnectedEventHandler(object sender, SocketAsyncEventArgs e);

    /// 断开成功通知函数
    public delegate void SocketClosedEventHandler(object sender, SocketAsyncEventArgs e);

    /// 接收数据通知函数
    public delegate bool SocketReceivedEventHandler(object sender, SocketAsyncEventArgs e);

    /// 发送数据通知函数
    public delegate void SocketSendedEventHandler(object sender, SocketAsyncEventArgs e);

    public class AsyncUserToken : IDisposable
    {
        /// 释放函数
        public void Dispose()
        {
            CurrentSocket = null;
            Tag = null;
        }

        /// Socket属性
        public Socket CurrentSocket
        {
            get;
            set;
        }

        /// 扩展属性
        public object Tag
        {
            get;
            set;
        }
    }
}
