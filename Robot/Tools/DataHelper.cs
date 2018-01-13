using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server.Protocol;
using ProtoBuf;
using ComponentAce.Compression.Libs.zlib;

namespace Server.Tools
{
    /// 数据操作辅助
    class DataHelper
    {
        /// 字节数据拷贝
        public static void CopyBytes(byte[] copyTo, int offsetTo, byte[] copyFrom, int offsetFrom, int count)
        {
            /*for (int i = 0; i < count; i++)
            {
                copyTo[offsetTo + i] = copyFrom[offsetFrom + i];
            }*/
            Array.Copy(copyFrom, offsetFrom, copyTo, offsetTo, count);
        }

        /// 字节数据排序
        public static void SortBytes(byte[] bytesData, int offsetTo, int count)
        {
            byte temp = 0;
            byte[] keyBytes = BitConverter.GetBytes((int)20110529);
            for (int i = 0; i < keyBytes.Length; i++)
            {
                temp += keyBytes[i];
            }

            for (int x = offsetTo; x < (offsetTo + count); x++)
            {
                bytesData[x] ^= temp;
            }
        }

        /// 比较两个字节数组是否相同
        public static bool CompBytes(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }

            bool ret = true;
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        /// 产生并填充随机数
        public static void RandBytes(byte[] buffer, int offset, int count)
        {
            long tick = DateTime.Now.Ticks;
            Random rnd = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = (byte)rnd.Next(0, 0xFF);
            }
        }

        /// 将字节流转换为Hex编码的字符串(无分隔符号)
        public static string Bytes2HexString(byte[] b)
        {
            int ch = 0;
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ch = (b[i] & 0xFF);
                ret += ch.ToString("X2").ToUpper();
            }

            return ret;
        }

        /// 将Hex编码的字符串转换为字节流(无分隔符号)
        public static byte[] HexString2Bytes(string s)
        {
            if (s.Length % 2 != 0) //非法的字符串
            {
                return null;
            }

            int b = 0;
            string hexstr = "";
            byte[] bytesData = new byte[s.Length / 2];
            for (int i = 0; i < s.Length / 2; i++)
            {
                hexstr = s.Substring(i * 2, 2);
                b = Int32.Parse(hexstr, System.Globalization.NumberStyles.HexNumber) & 0xFF;
                bytesData[i] = (byte)b;
            }

            return bytesData;
        }

        /// 格式化异常错误信息[控制台版这个函数理论上是线程安全的]
        public static void WriteFormatExceptionLog(Exception e, string extMsg, bool showMsgBox, bool finalReport = false)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("应用程序出现了异常[{0}]:\r\n{1}\r\n", finalReport ? 1 : 0, e.Message);

                stringBuilder.AppendFormat("\r\n 额外信息: {0}", extMsg);
                if (null != e)
                {
                    if (e.InnerException != null)
                    {
                        stringBuilder.AppendFormat("\r\n {0}", e.InnerException.Message);
                    }
                    stringBuilder.AppendFormat("\r\n {0}", e.StackTrace);
                }

                //记录异常日志文件
                LogManager.WriteException(stringBuilder.ToString());
                if (showMsgBox)
                {
                    //弹出异常日志窗口
                    System.Console.WriteLine(stringBuilder.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        /// 格式化堆栈信息
        public static void WriteFormatStackLog(System.Diagnostics.StackTrace stackTrace, string extMsg)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("应   用程序出现了对象锁定超时错误:\r\n");

                stringBuilder.AppendFormat("\r\n 额外信息: {0}", extMsg);
                stringBuilder.AppendFormat("\r\n {0}", stackTrace.ToString());

                //记录异常日志文件
                LogManager.WriteException(stringBuilder.ToString());
            }
            catch (Exception)
            {
            }
        }

        /// 判断如果不是 "*", 则转为指定的值, 否则默认值
        public static Int32 ConvertToInt32(string str, Int32 defVal)
        {
            try
            {
                if ("*" != str)
                {
                    return Convert.ToInt32(str);
                }

                return defVal;
            }
            catch (Exception)
            {
            }

            return defVal;
        }

        /// 判断如果不是 "*", 则转为指定的值, 否则默认值
        public static string ConvertToStr(string str, string defVal)
        {
            if ("*" != str)
            {
                return str;
            }

            return defVal;
        }

        /// 将日期时间字符串转为整数表示
        public static long ConvertToTicks(string str, long defVal)
        {
            if ("*" == str)
            {
                return defVal;
            }

            str = str.Replace('$', ':');

            try
            {
                DateTime dt;
                if (!DateTime.TryParse(str, out dt))
                {
                    return 0L;
                }

                return dt.Ticks / 10000;
            }
            catch (Exception)
            {
            }

            return 0L;
        }

        /// 将日期时间字符串转为整数表示
        public static long ConvertToTicks(string str)
        {
            try
            {
                DateTime dt;
            if (!DateTime.TryParse(str, out dt))
            {
                return 0L;
            }

            return dt.Ticks / 10000;
            }
            catch (Exception)
            {
            }

            return 0L;
        }

        /// Unix秒的起始计算毫秒时间(相对系统时间)
        private static long UnixStartTicks = DataHelper.ConvertToTicks("1970-01-01 08:00");

        /// 将Unix秒表示的时间转换为系统毫秒表示的时间
        public static long UnixSecondsToTicks(int secs)
        {
            return UnixStartTicks + ((long)secs * 1000);
        }

        /// 将Unix秒表示的时间转换为系统毫秒表示的时间
        public static long UnixSecondsToTicks(string secs)
        {
            int intSecs = Convert.ToInt32(secs);
            return UnixSecondsToTicks(intSecs);
        }

        /// 获取Unix秒表示的当前时间
        public static int UnixSecondsNow()
        {
            long ticks = DateTime.Now.Ticks / 10000;
            return SysTicksToUnixSeconds(ticks);
        }

        /// 将系统毫秒表示的时间转换为Unix秒表示的时间
        public static int SysTicksToUnixSeconds(long ticks)
        {
            long secs = (ticks - UnixStartTicks) / 1000;
            return (int)secs;
        }


        public static TCPOutPacket ObjectToTCPOutPacket<T>(T instance,  int cmdID)
        {
            byte[] bytesCmd = ObjectToBytes<T>(instance);
            return TCPOutPacket.MakeTCPOutPacket( bytesCmd, 0, bytesCmd.Length, cmdID);
        }

        public static int MinZipBytesSize = 256;
        /// 将字节数据转为对象
        public static T BytesToObject<T>(byte[] bytesData, int offset, int length)
        {
            if (bytesData.Length == 0) return default(T);

                //zlib解压缩算法
                byte[] copyData = new byte[length];
                DataHelper.CopyBytes(copyData, 0, bytesData, offset, length);
                copyData = DataHelper.Uncompress(copyData);

                MemoryStream ms = new MemoryStream();
                ms.Write(copyData, 0, copyData.Length);
                ms.Position = 0;
                T t = Serializer.Deserialize<T>(ms);
                ms.Dispose();
                ms = null;
                return t;
            

        }

        /// zlib 压缩算法
        public static byte[] Compress(byte[] bytes)
        {
            using (var ms = new MemoryStream())
            {
                using (ZOutputStream outZStream = new ZOutputStream(ms, zlibConst.Z_DEFAULT_COMPRESSION))
                {
                    outZStream.Write(bytes, 0, bytes.Length);
                    outZStream.Flush();
                }

                return ms.ToArray();
            }
        }

        /// zlib 解压缩算法
        public static byte[] Uncompress(byte[] bytes)
        {
            //小于2个字节肯定是非压缩的
            if (bytes.Length < 2)
            {
                return bytes;
            }

            //判断是否是压缩数据，是才执行解开压缩操作
            if (0x78 != bytes[0])
            {
                return bytes;
            }

            if (0x9C != bytes[1] && 0xDA != bytes[1])
            {
                return bytes;
            }

            using (var ms = new MemoryStream())
            {
                using (ZOutputStream outZStream = new ZOutputStream(ms))
                {
                    outZStream.Write(bytes, 0, bytes.Length);
                    outZStream.Flush();
                }

                return ms.ToArray();
            }
        }

        public static byte[] ObjectToBytes<T>(T instance)
        {
            {
                byte[] bytesCmd = null;
                if (null == instance)
                {
                    bytesCmd = new byte[0];
                }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    Serializer.Serialize<T>(ms, instance);
                    bytesCmd = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(bytesCmd, 0, bytesCmd.Length);
                    ms.Dispose();
                    ms = null;
                }

                if (bytesCmd.Length > DataHelper.MinZipBytesSize) //大于256字节的才压缩, 节省cpu占用，想一想，每秒10兆小流量的吐出，都在压缩，cpu占用当然会高, 带宽其实不是问题, 不会达到上限(100兆共享)
                {
                    //zlib压缩算法
                    byte[] newBytes = DataHelper.Compress(bytesCmd);
                    if (null != newBytes)
                    {
                        if (newBytes.Length < bytesCmd.Length)
                        {
                            bytesCmd = newBytes;
                        }
                    }
                }

                return bytesCmd;
            }

            return new byte[0];
        }


    }
}
