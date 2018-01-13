using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Tools
{
    /// RC4加密辅助类
    class RC4Helper
    {
        /// RC4加解密函数
        public static void RC4(byte[] bytesData, int offset, int count, string key)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(key);
            RC4(bytesData, offset, count, keyBytes);
        }

        /// RC4加解密函数
        public static void RC4(byte[] bytesData, int offset, int count, byte[] key)
        {
            byte[] s = new byte[256];
            byte[] k = new byte[256];
            byte temp;
            int i, j;

            for (i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
                k[i] = key[i % key.GetLength(0)];
            }

            j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + k[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            i = j = 0;
            for (int x = offset; x < (offset + count); x++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
                int t = (s[i] + s[j]) % 256;
                bytesData[x] ^= s[t];
            }
        }

        /// RC4加解密函数
        public static void RC4(byte[] bytesData, byte[] key)
        {
            RC4(bytesData, 0, bytesData.Length, key);
        }

        /// RC4加解密函数
        public static void RC4(byte[] bytesData, string key)
        {
            byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(key);
            RC4(bytesData, keyBytes);
        }
    }
}
