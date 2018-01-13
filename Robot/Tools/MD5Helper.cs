using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Tools
{
    class MD5Helper
    {
        /// 获取字符串的MD5值(HEX编码)
        public static string get_md5_string(string str)
        {
            byte[] bytes_md5_out = MD5Core.GetHash(str);
            return DataHelper.Bytes2HexString(bytes_md5_out);
        }

        /// 获取指定字节流的MD5值(HEX编码)
        public static string get_md5_string(byte[] data)
        {
            byte[] bytes_md5_out = MD5Core.GetHash(data);
            return DataHelper.Bytes2HexString(bytes_md5_out);
        }

        /// 获取字符串的MD5值(字节流)
        public static byte[] get_md5_bytes(string str)
        {
            byte[] bytes_md5_out = MD5Core.GetHash(str);
            return bytes_md5_out;
        }

        /// 获取指定字节流的MD5值(HEX编码)
        public static byte[] get_md5_bytes(byte[] data)
        {
            byte[] bytes_md5_out = MD5Core.GetHash(data);
            return bytes_md5_out;
        }
    }
}
