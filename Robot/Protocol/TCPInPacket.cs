using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using System.Net.Sockets;
using System.Net;
using Server;

namespace Server.Protocol
{
    /// 命令包接收完毕后的回调事件
    public delegate bool TCPCmdPacketEventHandler(object sender);

    /// TCP协议接收包处理(线程安全)
    class TCPInPacket : IDisposable
    {
        /// 构造函数
        public TCPInPacket(int recvBufferSize = (int)TCPCmdPacketSize.RECV_MAX_SIZE)
        {
            PacketBytes = new byte[recvBufferSize];
        }

        /// 保存接收到的数据包
        private byte[] PacketBytes = null;

        /// 获取接收缓存的指针
        public byte[] GetPacketBytes()
        {
            return PacketBytes;
        }

        /// 当前的客户端校验秒
        private int _LastCheckTicks = 0;

        /// 当前的客户端校验秒
        public int LastCheckTicks
        {
            get { return _LastCheckTicks; }
            set { _LastCheckTicks = value; }
        }

        /// 当前的Socket
        private Socket _Socket = null;

        /// 设置当前的socket属性
        public Socket CurrentSocket
        {
            get { return _Socket; }
            set { _Socket = value; }
        }

        /// 接收到的数据包的命令ID
        private Int16 _PacketCmdID = 0;

        /// 接收到的数据包的命令ID属性
        public Int16 PacketCmdID
        {
            get
            {
                Int16 ret = 0;
                lock (this)
                {
                    ret = _PacketCmdID;
                }
                return ret;
            }
        }

        /// 接收到的数据包的命令数据长度
        private Int32 _PacketDataSize = 0;

        /// 接收到的数据包的命令数据长度属性
        public Int32 PacketDataSize
        {
            get
            {
                Int32 ret = 0;
                lock (this)
                {
                    ret = _PacketDataSize;
                }
                return ret;
            }
        }

        /// 已经接收到的命令的数据长度
        private Int32 PacketDataHaveSize = 0;

        /// 是否正在等待数据
        private bool IsWaitingData = false;

        /// 释放函数
        public void Dispose()
        {
            Reset();
        }

        /// 接收完毕后通知外部处理的事件
        public event TCPCmdPacketEventHandler TCPCmdPacketEvent;

        /// 命令头缓冲
        private byte[] CmdHeaderBuffer = new byte[6];

        /// 已经接收到的命令头的大小
        private int CmdHeaderSize = 0;

        /// 将收到的数据写入
        public bool WriteData(byte[] buffer, int offset, int count)
        {
            //先锁定，否则字节流的顺序未必会是正确的顺序
            lock (this)
            {
                //一定要在系统设计时避免大的命令包，过长的数据拆分成小指令包发送
                //这个缓冲一定要避免越界, 各个分系统之间一定要设计好指定包的最大长度

                //1.首先判断当前是否正在等待数据
                if (IsWaitingData)
                {
                    int copyCount = (count >= (_PacketDataSize - PacketDataHaveSize)) ? _PacketDataSize - PacketDataHaveSize : count;
                    if (copyCount > 0)
                    {
                        DataHelper.CopyBytes(PacketBytes, PacketDataHaveSize, buffer, offset, copyCount);
                        PacketDataHaveSize += (Int32)copyCount;
                    }

                    //判断命令包的数据是否接收完毕, 否则继续等待
                    if (PacketDataHaveSize >= _PacketDataSize)
                    {
                        bool eventReturn = true;

                        //通知外部事件处理
                        if (null != TCPCmdPacketEvent)
                        {
                            eventReturn = TCPCmdPacketEvent(this);
                        }

                        //清空当前的状态
                        _PacketCmdID = 0;
                        _PacketDataSize = 0;
                        PacketDataHaveSize = 0;
                        IsWaitingData = false;
                        CmdHeaderSize = 0;

                        if (!eventReturn) //处理失败，要求关闭连接
                        {
                            return false;
                        }

                        //处理剩余的字节
                        if (count > copyCount)
                        {
                            //改变参数
                            offset += copyCount;
                            count -= copyCount;

                            //接收数据(递归实现, 简单)
                            return WriteData(buffer, offset, count);
                        }
                    }

                    return true;
                }
                else
                {
                    //正在等待接收命令头
                    int copyLeftSize = count > (6 - CmdHeaderSize) ? (6 - CmdHeaderSize) : count;
                    DataHelper.CopyBytes(CmdHeaderBuffer, CmdHeaderSize, buffer, offset, copyLeftSize);
                    CmdHeaderSize += copyLeftSize;
                    if (CmdHeaderSize < 6)
                    {
                        return true; //继续等待
                    }

                    //首先读取4个字节的整数
                    _PacketDataSize = BitConverter.ToInt32(CmdHeaderBuffer, 0);

                    //再读取2个字节的指令
                    _PacketCmdID = BitConverter.ToInt16(CmdHeaderBuffer, 4);

                    if (_PacketDataSize <= 0 || _PacketDataSize >= (Int32)TCPCmdPacketSize.MAX_SIZE)
                    {
                        //throw new Exception(string.Format("接收到的非法数据长度的tcp命令, Cmd={0}, Length={1} , 需要立即和服务器端断开!", (TCPGameServerCmds)_PacketCmdID, _PacketDataSize));
                        //LogManager.WriteLog(LogTypes.Error, string.Format("接收到的非法数据长度的tcp命令, Cmd={0}, Length={1}, offset={2}, count={3}", (TCPGameServerCmds)_PacketCmdID, _PacketDataSize, offset, count));
                        return false;
                    }

                    //增加偏移
                    offset += copyLeftSize;

                    //减少拷贝量
                    count -= copyLeftSize;

                    //等待数据中...
                    IsWaitingData = true;

                    //接收的字节归0
                    PacketDataHaveSize = 0;
                    _PacketDataSize -= 2; //减去命令的长度

                    //接收数据(递归实现, 简单)
                    return WriteData(buffer, offset, count);
                }
            }
        }

        /// 为不同的连接重复使用
        public void Reset()
        {
            lock (this)
            {
                _Socket = null;
                _PacketCmdID = 0;
                _PacketDataSize = 0;
                PacketDataHaveSize = 0;
                IsWaitingData = false;
                CmdHeaderSize = 0;
                _LastCheckTicks = 0;
            }
        }

    }
}
