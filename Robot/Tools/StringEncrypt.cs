using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Tools
{
    /// 字符串加密类
    public class StringEncrypt
    {
        /// 输入明文和密钥，输出密文
        public static string Encrypt(string plainText, string passwd, string saltValue)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return null;
            }

            byte[] bytesData = null;
            try
            {
                bytesData = new UTF8Encoding().GetBytes(plainText);
            }
            catch (Exception)
            {
                return null;
            }

            byte[] bytesResult = null;

            try
            {
                bytesResult = AesHelper.AesEncryptBytes(bytesData, passwd, saltValue);
            }
            catch (Exception)
            {
                return null;
            }

            return DataHelper.Bytes2HexString(bytesResult);
        }

        /// 输入密文和密钥，输出明文
        public static string Decrypt(string encryptText, string passwd, string saltValue)
        {
            if (string.IsNullOrEmpty(encryptText))
            {
                return null;
            }

            byte[] bytesData = DataHelper.HexString2Bytes(encryptText);
            if (null == bytesData) return null;

            byte[] bytesResult = null;
            try
            {
                bytesResult = AesHelper.AesDecryptBytes(bytesData, passwd, saltValue);
            }
            catch (Exception)
            {
                return null;
            }

            string strResult = null;
            try
            {
                strResult = new UTF8Encoding().GetString(bytesResult, 0, bytesResult.Length);
            }
            catch (Exception) //解析错误
            {
                return null;
            }

            return strResult;
        }
    }
}
