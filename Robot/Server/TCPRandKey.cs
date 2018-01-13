using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    /// 通过一个指定的随机数种子来构建指定数据的随机key
    public class TCPRandKey
    {
        /// 初始化
        public TCPRandKey(int capacity)
        {
            ListRandKey = new List<Int32>(capacity);
            DictRandKey = new Dictionary<Int32, bool>(capacity);
        }

        /// 随机数发生器
        private Random Rand = null;

        /// 保存随机的密码
        private List<Int32> ListRandKey = null;

        /// 快速访问密码是否存在
        private Dictionary<Int32, bool> DictRandKey = null;

        /// 
        public void Init(int count, int randSeed )
        {
            if (ListRandKey.Count > 0) return;

            Int32 key = 0;
            Rand = new Random(randSeed);
            for (int i = 0; i < count; i++)
            {
                key = Rand.Next(0, Int32.MaxValue);
                ListRandKey.Add(key);
                DictRandKey.Add(key, true);
            }
        }

        /// 查找指定的Key是否存在
        public bool FindKey(Int32 key)
        {
            return DictRandKey.ContainsKey(key);
        }

        /// 随机获取一个Key
        public Int32 GetKey()
        {
            int randIndex = Rand.Next(0, ListRandKey.Count);
            return ListRandKey[randIndex];
        }
    }
}
