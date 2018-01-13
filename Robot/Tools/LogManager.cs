using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Server.Tools
{
    /// 日志类型
    public enum LogTypes
    {
        Ignore = -1, //忽略
        Info = 0,
        Warning = 1,
        Error = 2,
        SQL = 3,
    }

    /// 日志管理类
    class LogManager
    {
        public LogManager()
        {
        }

        /// 允许实际写的日志级别
        public static LogTypes LogTypeToWrite
        {
            get;
            set;
        }

        /// 是否允许输出到dbgView窗口
        public static bool EnableDbgView = false;

        /// 日志输出目录
        private static string _LogPath = string.Empty;

        /// 日志输出目录
        public static string LogPath
        {
            get
            {
                lock (mutex)
                {
                    if (_LogPath == string.Empty)
                    {
                        _LogPath = AppDomain.CurrentDomain.BaseDirectory + @"log/";
                        if (!System.IO.Directory.Exists(_LogPath))
                        {
                            System.IO.Directory.CreateDirectory(_LogPath);
                        }
                    }
                }

                return _LogPath;
            }
            set
            {
                lock (mutex)
                {
                    _LogPath = value;
                }

                if (!System.IO.Directory.Exists(_LogPath))
                {
                    System.IO.Directory.CreateDirectory(_LogPath);
                }
            }
        }

        /// 异常输出目录
        private static string _ExceptionPath = string.Empty;

        /// 异常输出目录
        public static string ExceptionPath
        {
            get
            {
                lock (mutex)
                {
                    if (_ExceptionPath == string.Empty)
                    {
                        _ExceptionPath = AppDomain.CurrentDomain.BaseDirectory + @"Exception/";
                        if (!System.IO.Directory.Exists(_ExceptionPath))
                        {
                            System.IO.Directory.CreateDirectory(_ExceptionPath);
                        }
                    }
                }

                return _ExceptionPath;
            }
            set
            {
                lock (mutex)
                {
                    _ExceptionPath = value;
                }

                if (!System.IO.Directory.Exists(_ExceptionPath))
                {
                    System.IO.Directory.CreateDirectory(_ExceptionPath);
                }
            }
        }

        /// 将日志写入指定的文件
        private static void WriteLog(string logFile, string logMsg)
        {
            try
            {
                StreamWriter sw = File.AppendText(
                    LogPath + logFile + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + logMsg;
                if (EnableDbgView)
                {
                    System.Diagnostics.Debug.WriteLine(text);
                }

                sw.WriteLine(text);
                sw.Close();
                sw = null;
            }
            catch
            {
            }
        }

        /// 将异常写入指定的文件
        private static void _WriteException(string exceptionMsg)
        {
            try
            {
                StreamWriter sw = File.CreateText(
                    ExceptionPath + "Exception_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
                sw.WriteLine(exceptionMsg);
                sw.Close();
                sw = null;
            }
            catch
            {
            }
        }

        /// 写日志的锁对象
        private static object mutex = new object();

        /// 写日志

        public static void WriteLog(LogTypes logType, string logMsg)
        {
            if ((int)logType < (int)LogTypeToWrite) //不必记录
            {
                return;
            }

            lock (mutex)
            {
                WriteLog(logType.ToString(), logMsg);
            }
        }

        /// 写异常
        public static void WriteException(string exceptionMsg)
        {
            lock (mutex)
            {
                _WriteException(exceptionMsg);
            }
        }
    }
}
