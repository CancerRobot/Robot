using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using Server.Tools;
//using System.Windows.Forms;
using Server.Protocol;

namespace Server.TCP
{

    public sealed class SocketListener
    {
        Int32 ReceiveBufferSize;

        /// Represents a large reusable set of buffers for all socket operations.
        private BufferManager bufferManager;

        /// The socket used to listen for incoming connection requests.
        private Socket listenSocket;

        /// The total number of clients connected to the server.
        private Int32 numConnectedSockets;

        private Dictionary<Socket, bool> ConnectedSocketsDict;

        public Int32 ConnectedSocketsCount
        {
            get
            {
                Int32 n = 0;
                Interlocked.Exchange(ref n, this.numConnectedSockets);
                return n;
            }
        }

        /// the maximum number of connections the sample is designed to handle simultaneously.
        private Int32 numConnections;

        /// Read, write (don't alloc buffer space for accepts).
        private const Int32 opsToPreAlloc = 1;

        /// Pool of reusable SocketAsyncEventArgs objects for read socket operations.
        private SocketAsyncEventArgsPool readPool; //�̰߳�ȫ

        /// Pool of reusable SocketAsyncEventArgs objects for write socket operations.
        private SocketAsyncEventArgsPool writePool; //�̰߳�ȫ

        /// Controls the total number of clients connected to the server.
        private Semaphore semaphoreAcceptedClients;

        /// Total # bytes counter received by the server.
        private Int32 totalBytesRead;

        /// ��ȡ�ܵĽ��յ��ֽ���
        public Int32 TotalBytesReadSize
        {
            get
            {
                Int32 n = 0;
                Interlocked.Exchange(ref n, this.totalBytesRead);
                return n;
            }
        }

        /// Total # bytes counter received by the server.
        private Int32 totalBytesWrite;

        public Int32 TotalBytesWriteSize
        {
            get
            {
                Int32 n = 0;
                Interlocked.Exchange(ref n, this.totalBytesWrite);
                return n;
            }
        }

        public event SocketConnectedEventHandler SocketConnected = null;

        public event SocketClosedEventHandler SocketClosed = null;

        public event SocketReceivedEventHandler SocketReceived = null;

        public event SocketSendedEventHandler SocketSended = null;

        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method.
        internal SocketListener(Int32 numConnections, Int32 receiveBufferSize)
        {
            this.totalBytesRead = 0;
            this.totalBytesWrite = 0;
            this.numConnectedSockets = 0;
            this.numConnections = numConnections;
            this.ReceiveBufferSize = receiveBufferSize;

            // Allocate buffers such that the maximum number of sockets can have one outstanding read and 
            // write posted to the socket simultaneously .
            this.bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
                receiveBufferSize);

            this.ConnectedSocketsDict = new Dictionary<Socket, bool>(numConnections);

            this.readPool = new SocketAsyncEventArgsPool(numConnections);
            this.writePool = new SocketAsyncEventArgsPool(numConnections * 5);
            this.semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);
        }

        private void AddSocket(Socket socket)
        {
            lock (this.ConnectedSocketsDict)
            {
                this.ConnectedSocketsDict.Add(socket, true);
            }
        }

        private void RemoveSocket(Socket socket)
        {
            lock (this.ConnectedSocketsDict)
            {
                this.ConnectedSocketsDict.Remove(socket);
            }
        }

        private bool FindSocket(Socket socket)
        {
            bool ret = false;
            lock (this.ConnectedSocketsDict)
            {
                ret = this.ConnectedSocketsDict.ContainsKey(socket);
            }

            return ret;
        }

        /// Close the socket associated with the client.
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken aut = e.UserToken as AsyncUserToken;

            try
            {
                Socket s = aut.CurrentSocket;
                if (!FindSocket(s)) 
                {
                    return;
                }

                RemoveSocket(s);

                try
                {
                    LogManager.WriteLog(LogTypes.Info, string.Format("�رտͻ�������: {0}, ����: {1}, ԭ��: {2}", s.RemoteEndPoint, e.LastOperation, e.SocketError));
                }
                catch (Exception)
                {
                }

                // Decrement the counter keeping track of the total number of clients connected to the server.
                this.semaphoreAcceptedClients.Release(); 
                Interlocked.Decrement(ref this.numConnectedSockets);

                if (null != SocketClosed)
                {
                    SocketClosed(this, e);
                }

                try
                {
                    s.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                    // Throws if client process has already closed.
                }

                try
                {
                    s.Close();
                }
                catch (Exception)
                {
                }
            }
            finally
            {
                aut.CurrentSocket = null; //�ͷ�
                aut.Tag = null; //�ͷ�

                // Free the SocketAsyncEventArg so they can be reused by another client.
                if (e.LastOperation == SocketAsyncOperation.Send)
                {
                    e.SetBuffer(null, 0, 0); //�����ڴ�
                    this.writePool.Push(e);
                }
                else if (e.LastOperation == SocketAsyncOperation.Receive)
                {
                    this.readPool.Push(e);
                }
            }
        }

        /// Initializes the server by preallocating reusable buffers and 
        /// context objects.  These objects do not need to be preallocated 
        /// or reused, but it is done this way to illustrate how the API can 
        /// easily be used to create reusable objects to increase server performance.
        internal void Init()
        {
            // Allocates one large Byte buffer which all I/O operations use a piece of. This guards 
            // against memory fragmentation.
            this.bufferManager.InitBuffer();

            // Preallocate pool of SocketAsyncEventArgs objects.
            SocketAsyncEventArgs readWriteEventArg;

            for (Int32 i = 0; i < this.numConnections; i++)
            {
                // Preallocate a set of reusable SocketAsyncEventArgs.
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                readWriteEventArg.UserToken = new AsyncUserToken() { CurrentSocket = null, Tag = null };

                // Assign a Byte buffer from the buffer pool to the SocketAsyncEventArg object.
                this.bufferManager.SetBuffer(readWriteEventArg);

                // Add SocketAsyncEventArg to the pool.
                this.readPool.Push(readWriteEventArg);

                for (Int32 j = 0; j < 5; j++)
                {
                    readWriteEventArg = new SocketAsyncEventArgs();
                    readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                    readWriteEventArg.UserToken = new AsyncUserToken() { CurrentSocket = null, Tag = null };
                    this.writePool.Push(readWriteEventArg);
                }
            }
        }

        /// Callback method associated with Socket.AcceptAsync 
        /// operations and is invoked when an accept operation is complete.
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (null == this.listenSocket) return; //�Ѿ���������

                this.ProcessAccept(e);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("��SocketListener::OnAcceptCompleted �з������쳣����"));
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    DataHelper.WriteFormatExceptionLog(ex, "", false);
                //});
            }
        }

        /// Callback called whenever a receive or send operation is completed on a socket.
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                // Determine which type of operation just completed and call the associated handler.
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        this.ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        this.ProcessSend(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("��SocketListener::OnIOCompleted �з������쳣����"));
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    DataHelper.WriteFormatExceptionLog(ex, "", false);
                //});
            }
        }

        /// ��socket��������
        private bool _ReceiveAsync(SocketAsyncEventArgs readEventArgs)
        {
            try
            {
                Socket s = (readEventArgs.UserToken as AsyncUserToken).CurrentSocket;
                return s.ReceiveAsync(readEventArgs);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("��SocketListener::_ReceiveAsync �з������쳣����"));
                this.CloseClientSocket(readEventArgs);
                return true;
            }
        }

        /// ��Socket��������
        private bool _SendAsync(SocketAsyncEventArgs writeEventArgs, out bool exception)
        {
            exception = false;

            try
            {
                Socket s = (writeEventArgs.UserToken as AsyncUserToken).CurrentSocket;
                return s.SendAsync(writeEventArgs);
            }
            catch (Exception) //�˴��п����Ƕ���Ƿ����쳣, ����Socket�����Ѿ���Ч
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("��SocketListener::_SendAsync �з������쳣����"));
                exception = true;
                //this.CloseClientSocket(writeEventArgs);
                return true;
            }
        }

        /// ��ͻ��˷�������
        internal bool SendData(Socket s, TCPOutPacket tcpOutPacket)
        {
            SocketAsyncEventArgs writeEventArgs = this.writePool.Pop(); //�̰߳�ȫ�Ĳ���
            if (null == writeEventArgs)
            {
                writeEventArgs = new SocketAsyncEventArgs();
                writeEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                writeEventArgs.UserToken = new AsyncUserToken() { CurrentSocket = null, Tag = null };
            }

            //�ֽ�����
            //DataHelper.SortBytes(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);

            writeEventArgs.SetBuffer(tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);
            (writeEventArgs.UserToken as AsyncUserToken).CurrentSocket = s;
            (writeEventArgs.UserToken as AsyncUserToken).Tag = (object)tcpOutPacket;

            bool exception = false;
            Boolean willRaiseEvent = _SendAsync(writeEventArgs, out exception);
            if (!willRaiseEvent)
            {
                this.ProcessSend(writeEventArgs);
            }

            return (!exception);
        }

        /// Process the accept for the socket listener.
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs readEventArgs = null;

            //���Ӽ�����
            Interlocked.Increment(ref this.numConnectedSockets);

            // Get the socket for the accepted client connection and put it into the 
            // ReadEventArg object user token.
            Socket s = e.AcceptSocket;
            readEventArgs = this.readPool.Pop();
            (readEventArgs.UserToken as AsyncUserToken).CurrentSocket = e.AcceptSocket;

            AddSocket(s);

            try
            {
                LogManager.WriteLog(LogTypes.Info, string.Format("��Զ������: {0}, ��ǰ�ܹ�: {1}", s.RemoteEndPoint, numConnectedSockets));
            }
            catch (Exception)
            {
            }

            /// ���ӳɹ�֪ͨ����
            if (null != SocketConnected)
            {
                SocketConnected(this, readEventArgs);
            }

            // As soon as the client is connected, post a receive to the connection.
            Boolean willRaiseEvent = _ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                this.ProcessReceive(readEventArgs);
            }

            // Accept the next connection request.
            this.StartAccept(e);
        }

        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // Check if the remote host closed the connection.
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // Increment the count of the total bytes receive by the server.
                Interlocked.Add(ref this.totalBytesRead, e.BytesTransferred);

                bool recvReturn = true;

                // Get the message received from the listener.
                //e.Buffer, e.Offset, e.bytesTransferred);
                //֪ͨ�ⲿ���µ�socket����
                if (null != SocketReceived)
                {
                    //�ֽ�����
                    DataHelper.SortBytes(e.Buffer, e.Offset, e.BytesTransferred);
                    recvReturn = SocketReceived(this, e);
                }

                if (recvReturn)
                {
                    //������������(��������Ϊ����)
                    Boolean willRaiseEvent = _ReceiveAsync(e);
                    if (!willRaiseEvent)
                    {
                        this.ProcessReceive(e);
                    }
                }
                else
                {
                    this.CloseClientSocket(e);
                }
            }
            else
            {
                this.CloseClientSocket(e);
            }
        }

        /// This method is invoked when an asynchronous send operation completes.  
        /// The method issues another receive on the socket to read any additional 
        /// data sent from the client.
        private void ProcessSend(SocketAsyncEventArgs e)
        {
             /// ��������֪ͨ����
            if (null != SocketSended)
            {
                SocketSended(this, e);
            }

            if (e.SocketError == SocketError.Success)
            {
                Interlocked.Add(ref this.totalBytesWrite, e.BytesTransferred);
            }
            else
            {
                //this.CloseClientSocket(e);
            }

            //ʲô���鶼����, �ջ�ʹ�õ�e��buffer
            // Free the SocketAsyncEventArg so they can be reused by another client.
            e.SetBuffer(null, 0, 0); //�����ڴ�
            (e.UserToken as AsyncUserToken).CurrentSocket = null; //�ͷ�
            (e.UserToken as AsyncUserToken).Tag = null; //�ͷ�
            this.writePool.Push(e);
        }

        /// Starts the server such that it is listening for incoming connection requests.    
        internal void Start(string ip, int port)
        {
            if ("" == ip) ip = "0.0.0.0"; //��ֹIP��Ч

            // Get endpoint for the listener.
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            // Create the socket which listens for incoming connections.
            this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Associate the socket with the local endpoint.
            this.listenSocket.Bind(localEndPoint);

            // Start the server with a listen backlog of 100 connections.
            this.listenSocket.Listen(100);

            // Post accepts on the listening socket.
            this.StartAccept(null);
        }
        
        /// �ر�������Socket
        public void Stop()
        {
            Socket s = this.listenSocket;
            this.listenSocket = null;
            s.Close();
        }

        /// �ر�ָ����Socket����
        private void CloseSocket(Socket s)
        {
            try
            {
                s.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                // Throws if client process has already closed.
            }

            //�ɽ����¼�ȥ�ͷŴ���
        }

        /// Begins an operation to accept a connection request from the client.
        /// the accept operation on the server's listening socket.</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                // Socket must be cleared since the context object is being reused.
                acceptEventArg.AcceptSocket = null;
            }

            this.semaphoreAcceptedClients.WaitOne(); //�����ܵ�������
            Boolean willRaiseEvent = this.listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                this.ProcessAccept(acceptEventArg);
            }
        }
    }
}
