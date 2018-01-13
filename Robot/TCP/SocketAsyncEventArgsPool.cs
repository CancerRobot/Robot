using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
    /// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx
    /// Represents a collection of reusable SocketAsyncEventArgs objects.  
    internal sealed class SocketAsyncEventArgsPool
    {
        /// Pool of SocketAsyncEventArgs.
        Stack<SocketAsyncEventArgs> pool;

        /// Initializes the object pool to the specified size.
        internal SocketAsyncEventArgsPool(Int32 capacity)
        {
            this.pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// The number of SocketAsyncEventArgs instances in the pool. 
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

        /// Removes a SocketAsyncEventArgs instance from the pool.
        internal SocketAsyncEventArgs Pop()
        {
            lock (this.pool)
            {
                if (this.pool.Count <= 0) return null; //��ֹ�̶����岻��ʹ��
                return this.pool.Pop();
            }
        }

        /// Add a SocketAsyncEventArg instance to the pool. 
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null) 
            { 
                throw new ArgumentNullException("��ӵ�SocketAsyncEventArgsPool ��item�����ǿ�(null)"); 
            }
            lock (this.pool)
            {
                this.pool.Push(item);
            }
        }
    }
}
