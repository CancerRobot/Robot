using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using System.Net.Sockets;
using System.Net;

namespace Server.Protocol
{
    /// TCP命令发送包处理(非线程安全)
    public class TCPOutPacket : IDisposable
    {
        /// 构造函数
        public TCPOutPacket()
        {
        }

        /// 保存接收到的数据包
        private byte[] PacketBytes = null;

        /// 获取接收缓存的指针
        public byte[] GetPacketBytes()
        {
            return PacketBytes;
        }

        /// 接收到的数据包的命令ID
        private Int16 _PacketCmdID = 0;

        /// 数据包的命令ID属性
        public Int16 PacketCmdID
        {
            get { return _PacketCmdID; }
            set { _PacketCmdID = value; }
        }

        /// 发送的数据包的命令数据长度
        private Int32 _PacketDataSize = 0;

        /// 要发送的数据包的命令数据长度属性
        public Int32 PacketDataSize
        {
            get { return (Int32)(_PacketDataSize + 4 + 2 + 1 + 4); }
        }

        /// 扩展对象
        public object Tag
        {
            get;
            set;
        }

        /// 数据写入
        public bool FinalWriteData(byte[] buffer, int offset, int count)
        {
            if (null != PacketBytes)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("TCP发出命令包不能被重复写入数据, 命令ID: {0}", PacketCmdID));
                return false;
            }

            //先判断是否超出的最大包的大小
            if ((4 + 2 + 1 + 4 + count) >= (int)TCPCmdPacketSize.MAX_SIZE)
            {
                //throw new Exception(string.Format("TCP命令包长度最大不能超过: {0}", (int)TCPCmdPacketSize.MAX_SIZE));
                LogManager.WriteLog(LogTypes.Error, string.Format("TCP命令包长度:{0}, 最大不能超过: {1}, 命令ID: {2}", count, (int)TCPCmdPacketSize.MAX_SIZE, PacketCmdID));
                return false;
            }

            PacketBytes = new byte[count + 4 + 2 + 1 + 4];

            //写入数据
            int offsetTo = (int)(4 + 2 + 1 + 4);
            DataHelper.CopyBytes(PacketBytes, offsetTo, buffer, offset, count);
            _PacketDataSize = count;
            Final();

            return true;
        }
        public static long Before1970Ticks = 621356256000000000L;

        /// 生成要发送的指令包字节序
        private void Final()
        {
            //拷贝数据长度
            Int32 length = (Int32)(_PacketDataSize + 2 + 1 + 4);
            DataHelper.CopyBytes(PacketBytes, 0, BitConverter.GetBytes(length), 0, 4);

            //拷贝指令
            DataHelper.CopyBytes(PacketBytes, 4, BitConverter.GetBytes(_PacketCmdID), 0, 2);

            int localTimer = (int)(long)(((DateTime.Now.Ticks / 10000) - Before1970Ticks) / 1000);
            byte[] localTimerBytes = BitConverter.GetBytes(localTimer);

            CRC32 crc32 = new CRC32();
            crc32.update(localTimerBytes);

            int offsetTo = (int)(4 + 2 + 1 + 4);
            crc32.update(PacketBytes, offsetTo, _PacketDataSize);
            uint cc = crc32.getValue() % 255;
            uint cc2 = (uint)(_PacketCmdID % 255);
            int cc3 = (int)(cc ^ cc2);

            //拷贝校验和
            DataHelper.CopyBytes(PacketBytes, 6, BitConverter.GetBytes((byte)cc3), 0, 1);

            //拷贝时间戳
            DataHelper.CopyBytes(PacketBytes, 7, localTimerBytes, 0, 4);
        }

        /// 重复利用指令包
        public void Reset()
        {
            PacketBytes = null;
            PacketCmdID = 0;
            _PacketDataSize = 0;
        }

        /// 释放函数
        public void Dispose()
        {
            Tag = null;
        }

        /// 生成TCPOutPacket
        public static TCPOutPacket MakeTCPOutPacket(string data, int cmd)
        {

            //连接成功, 立即发送请求登陆的指令
            TCPOutPacket tcpOutPacket = new TCPOutPacket();
            tcpOutPacket.PacketCmdID = (Int16)cmd;
            byte[] bytesCmd = new UTF8Encoding().GetBytes(data);
            tcpOutPacket.FinalWriteData(bytesCmd, 0, bytesCmd.Length);
            return tcpOutPacket;
        }

        /// 生成TCPOutPacket
        public static TCPOutPacket MakeTCPOutPacket(byte[] data, int offset, int length, int cmd)
        {
            //连接成功, 立即发送请求登陆的指令
            TCPOutPacket tcpOutPacket = new TCPOutPacket();
            tcpOutPacket.PacketCmdID = (Int16)cmd;
            tcpOutPacket.FinalWriteData(data, offset, length);
            return tcpOutPacket;
        }
    }
}
