using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server.Protocol
{
    /// TCPInPacket缓冲池实现
    public class TCPInPacketPool
    {
        /// Pool of TCPInPacket.
        Stack<TCPInPacket> pool;

        /// Initializes the object pool to the specified size.
        internal TCPInPacketPool(Int32 capacity)
        {
            this.pool = new Stack<TCPInPacket>(capacity);
        }

        /// The number of TCPInPacket instances in the pool. 
        internal Int32 Count
        {
            get 
            {
                int count = 0;
                lock (this.pool)
                {
                    count = this.pool.Count;
                }

                return count;
            }
        }

        /// Removes a TCPInPacket instance from the pool.
        internal TCPInPacket Pop(Socket s, TCPCmdPacketEventHandler TCPCmdPacketEvent)
        {
            lock (this.pool)
            {
                TCPInPacket tcpInPacket = null;
                if (this.pool.Count <= 0)
                {
                    //临时分配
                    tcpInPacket = new TCPInPacket() { CurrentSocket = s };
                    tcpInPacket.TCPCmdPacketEvent += TCPCmdPacketEvent;
                    return tcpInPacket;
                }

                tcpInPacket = this.pool.Pop();
                tcpInPacket.CurrentSocket = s;
                return tcpInPacket;
            }
        }

        /// Add a TCPInPacket instance to the pool. 
        internal void Push(TCPInPacket item)
        {
            if (item == null) 
            {
                throw new ArgumentNullException("添加到TCPInPacketPool 的item不能是空(null)"); 
            }
            lock (this.pool)
            {
                item.Reset();
                this.pool.Push(item);
            }
        }
    }
}
