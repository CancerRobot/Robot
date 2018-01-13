using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace Robot
{
    class RobotClientMgr
    {
        string ServerIP { get; set; }
        int ServerPort { get; set; }

        private TCPRandKey tcpRandKey = new TCPRandKey(10000);
        private Dictionary<int, RobotClient> AllRobots = new Dictionary<int, RobotClient>();

        public void Init(string ip)
        {
            ServerIP = ip;
            ServerPort = 4403;
            tcpRandKey.Init(10000, 123456);
            TCPCmdHandler.KeySHA1 = "12345";
            TCPCmdHandler.KeyData = "12345";
            TCPCmdHandler.WebKey = "9377(*)#mst9";

        }

        public async void GenerateRobot(int startID, int endID, int mapCode, int mapSize)
        {
            int baseID = 100000;
            for (int i = startID; i <= endID; i++)
            {
                Task<RobotClient> task = CreateRobot(baseID + i, mapCode, mapSize);
                var robot = await task;
                AllRobots.Add(robot.UserID, robot);
            }
            StartTest();
        }

        Task<RobotClient> CreateRobot(int id, int mapcode, int mapSize)
        {
            return Task.Run(() =>
            {
                var robot = new RobotClient();
                robot.MapCode = mapcode;
                robot.MapSize = mapSize;
                robot.Connect(id, tcpRandKey.GetKey(), ServerIP, ServerPort);
                return robot;

            });
        }

        public void StartTest()
        {
            Task.Run(() =>
            {
                while (!IsStoped)
                {
                    if (IsPaused)
                    {
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    long ticks = DateTime.Now.Ticks / 10000;
                    foreach (var robot in AllRobots.Values)
                    {
                        robot.RunTest(ticks);
                    }
                    System.Threading.Thread.Sleep(100);
                }
                foreach (var robot in AllRobots.Values)
                {
                    robot.Disconnect();
                }
                AllRobots.Clear();
                GC.Collect();

            });
        }

        public  bool IsStoped = false;
        public  bool IsPaused = false;

    }
}
