using System;
using System.Linq;
using System.Xml.Linq;
using System.Net.Sockets;

namespace Server.Logic
{
    /// 全局类
    public class Global
    {
        public static bool ExitServer = false;

        #region XML操作相关

        /// 获取指定的xml节点的节点路径
        public static string GetXElementNodePath(XElement element)
        {
            try
            {
                string path = element.Name.ToString();
                element = element.Parent;
                while (null != element)
                {
                    path = element.Name.ToString() + "/" + path;
                    element = element.Parent;
                }

                return path;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// 获取XML文件树节点段XElement
        public static XElement GetXElement(XElement XML, string newroot)
        {
            return XML.DescendantsAndSelf(newroot).Single();
        }

        /// 获取XML文件树节点段XElement
        public static XElement GetSafeXElement(XElement XML, string newroot)
        {
            try
            {
                return XML.DescendantsAndSelf(newroot).Single();
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取: {0} 失败, xml节点名: {1}", newroot, GetXElementNodePath(XML)));
            }
        }

        /// 获取XML文件树节点段XElement
        public static XElement GetXElement(XElement XML, string newroot, string attribute, string value)
        {
            return XML.DescendantsAndSelf(newroot).Single(X => X.Attribute(attribute).Value == value);
        }

        /// 获取XML文件树节点段XElement
        public static XElement GetSafeXElement(XElement XML, string newroot, string attribute, string value)
        {
            try
            {
                return XML.DescendantsAndSelf(newroot).Single(X => X.Attribute(attribute).Value == value);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取: {0}/{1}={2} 失败, xml节点名: {3}", newroot, attribute, value, GetXElementNodePath(XML)));
            }
        }

        /// 获取属性值
        public static XAttribute GetSafeAttribute(XElement XML, string attribute)
        {
            try
            {
                XAttribute attrib = XML.Attribute(attribute);
                if (null == attrib)
                {
                    throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
                }

                return attrib;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
            }
        }

        /// 获取属性值(字符串)
        public static string GetSafeAttributeStr(XElement XML, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, attribute);
            return (string)attrib;
        }

        /// 获取属性值(整型)
        public static long GetSafeAttributeLong(XElement XML, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return -1;

            try
            {
                return (long)Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
            }
        }

        /// 获取属性值(浮点)
        public static double GetSafeAttributeDouble(XElement XML, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return 0.0;

            try
            {
                return Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
            }
        }

        /// 取得xml的属性值
        public static XAttribute GetSafeAttribute(XElement XML, string root, string attribute)
        {
            try
            {
                XAttribute attrib = XML.Element(root).Attribute(attribute);
                if (null == attrib)
                {
                    throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
                }

                return attrib;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
            }
        }

        /// 取得xml的属性值(字符串)
        public static string GetSafeAttributeStr(XElement XML, string root, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, root, attribute);
            return (string)attrib;
        }

        /// 取得xml的属性值(整型值)
        public static long GetSafeAttributeLong(XElement XML, string root, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, root, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return -1;

            try
            {
                return (long)Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
            }
        }

        /// 取得xml的属性值(浮点型)
        public static double GetSafeAttributeDouble(XElement XML, string root, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, root, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return -1;

            try
            {
                return Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
            }
        }

        #endregion //XML操作相关

        #region 调试辅助

        /// 获取Socket的远端IP地址
        public static string GetSocketRemoteEndPoint(Socket s)
        {
            try
            {
                return string.Format("{0} ", s.RemoteEndPoint);
            }
            catch (Exception)
            {
            }

            return "";
        }

        public static string GetDebugHelperInfo(Socket socket)
        {
            if (null == socket)
            {
                return "socket为null, 无法打印错误信息";
            }

            string ret = "";
            try
            {
                ret += string.Format("IP={0} ", GetSocketRemoteEndPoint(socket));
            }
            catch (Exception)
            {
            }

            return ret;
        }

        #endregion 调试辅助
    }
}
