using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.TCP
{
    /// Based on example from http://msdn2.microsoft.com/en-us/library/bb517542.aspx
    /// This class creates a single large buffer which can be divided up 
    /// and assigned to SocketAsyncEventArgs objects for use with each 
    /// socket I/O operation.  
    /// This enables bufffers to be easily reused and guards against 
    /// fragmenting heap memory.
    internal sealed class BufferManager
    {
        /// The underlying Byte array maintained by the Buffer Manager.
        private Byte[] buffer;                

        /// Size of the underlying Byte array.
        private Int32 bufferSize;

        /// Current index of the underlying Byte array.
        private Int32 currentIndex;

        /// Pool of indexes for the Buffer Manager.
        private Stack<Int32> freeIndexPool;     

        /// The total number of bytes controlled by the buffer pool.
        private Int32 numBytes;

        /// Instantiates a buffer manager.
        internal BufferManager(Int32 totalBytes, Int32 bufferSize)
        {
            this.numBytes = totalBytes;
            this.currentIndex = 0;
            this.bufferSize = bufferSize;
            this.freeIndexPool = new Stack<Int32>();
        }

        /// Removes the buffer from a SocketAsyncEventArg object. 
        /// This frees the buffer back to the buffer pool.
        internal void FreeBuffer(SocketAsyncEventArgs args)
        {
            this.freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        ///  Allocates buffer space used by the buffer pool. 
        internal void InitBuffer()
        {
            // Create one big large buffer and divide that out to each SocketAsyncEventArg object.
            this.buffer = new Byte[this.numBytes];
        }

        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object.
        internal Boolean SetBuffer(SocketAsyncEventArgs args)
        {
            if (this.freeIndexPool.Count > 0)
            {
                args.SetBuffer(this.buffer, this.freeIndexPool.Pop(), this.bufferSize);
            }
            else
            {
                if ((this.numBytes - this.bufferSize) < this.currentIndex)
                {
                    return false;
                }
                args.SetBuffer(this.buffer, this.currentIndex, this.bufferSize);
                this.currentIndex += this.bufferSize;
            }

            return true;
        }
    }
}
