using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.Threading;

namespace Server
{
    /// 连接到GameServer的客户端连接池类
    public class TCPClientPool
    {
        /// 初始化
        public TCPClientPool(int capacity)
        {
            this.pool = new Queue<TCPClient>(capacity);
        }

        /// 错误的数量
        private int ErrCount = 0;

        /// 连接项的数量
        private int ItemCount = 0;

        /// 远端IP
        private string RemoteIP = "";

        /// 远端端口
        private int RemotePort = 0;

        /// 连接栈对象
        private Queue<TCPClient> pool;

        /// 控制获取连接数
        private Semaphore semaphoreClients = null;


        /// 初始化连接池
        public void Init(int count, string ip, int port)
        {
            ItemCount = 0;
            RemoteIP = ip;
            RemotePort = port;
        }

        public TCPClient GetNewTCPClient()
        {
            TCPClient tcpClient = new TCPClient() { ListIndex = ItemCount };
            tcpClient.Connect(RemoteIP, RemotePort);
            ItemCount++;

            return tcpClient;
        }

        /// 删除连接池
        public void Clear()
        {
            lock (this.pool)
            {
                for (int i = 0; i < this.pool.Count; i++)
                {
                    TCPClient tcpClient = this.pool.ElementAt(i);
                    tcpClient.Disconnect();
                }

                this.pool.Clear();
            }
        }

        ///  补充断开的连接
        public void Supply()
        {
            lock (this.pool)
            {
                if (ErrCount <= 0)
                {
                    return;
                }

                if (ErrCount > 0)
                {
                    try
                    {
                        TCPClient tcpClient = new TCPClient() {  ListIndex = ItemCount };


                        ItemCount++;

                        tcpClient.Connect(RemoteIP, RemotePort);
                        this.pool.Enqueue(tcpClient);
                        ErrCount--;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// 获取一个连接
        public TCPClient Pop()
        {
            this.semaphoreClients.WaitOne(); //防止无法获取， 阻塞等待
            lock (this.pool)
            {
                return this.pool.Dequeue();
            }
        }

        /// 压入一个连接
        public void Push(TCPClient tcpClient)
        {
            //如果是已经无效的连接，则不再放入缓存池
            if (!tcpClient.IsConnected())
            {
                lock (this.pool)
                {
                    ErrCount++;
                }

                return;
            }

            lock (this.pool)
            {
                this.pool.Enqueue(tcpClient);
            }

            this.semaphoreClients.Release();
        }
    }
}
