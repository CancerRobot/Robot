using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server.Protocol
{
    /// TCPOutPacket缓冲池实现
    public class TCPOutPacketPool
    {
        /// Pool of TCPOutPacket.
        Stack<TCPOutPacket> pool;

        /// Initializes the object pool to the specified size.
        internal TCPOutPacketPool(Int32 capacity)
        {
            this.pool = new Stack<TCPOutPacket>(capacity);
        }

        /// The number of TCPOutPacket instances in the pool. 
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

        /// Removes a TCPOutPacket instance from the pool.
        internal TCPOutPacket Pop()
        {
            lock (this.pool)
            {
                if (this.pool.Count <= 0)
                {
                    //临时分配
                    return new TCPOutPacket();
                }

                return this.pool.Pop();
            }
        }

        /// Add a TCPOutPacket instance to the pool. 
        internal void Push(TCPOutPacket item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("添加到TCPOutPacketPool 的item不能是空(null)");
            }
            lock (this.pool)
            {
                item.Reset();
                this.pool.Push(item);
            }
        }
    }
}
