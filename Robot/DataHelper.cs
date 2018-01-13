using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Robot
{
    [ProtoContract]
    public class SpriteMoveData
    {
        //private static SpriteMoveData instance = new SpriteMoveData();
        public SpriteMoveData() { }

        public SpriteMoveData(int roleID, int mapCode, int action, int toX, int toY, int extAction, int fromX, int fromY, long startMoveTicks, string pathString)
        {
            this.roleID = roleID;
            this.mapCode = mapCode;
            this.action = action;
            this.toX = toX;
            this.toY = toY;
            this.extAction = extAction;
            this.fromX = fromX;
            this.fromY = fromY;
            this.startMoveTicks = startMoveTicks;
            this.pathString = pathString;
        }


        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int mapCode = 0;

        [ProtoMember(3)]
        public int action = 0;

        [ProtoMember(4)]
        public int toX = 0;

        [ProtoMember(5)]
        public int toY = 0;

        [ProtoMember(6)]
        public int extAction = 0;

        [ProtoMember(7)]
        public int fromX = 0;

        [ProtoMember(8)]
        public int fromY = 0;

        [ProtoMember(9)]
        public long startMoveTicks = 0;

        [ProtoMember(10)]
        public string pathString = "";

    }

    [ProtoContract]
    public class SpriteMagicCodeData
    {
        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int mapCode = 0;

        [ProtoMember(3)]
        public int magicCode = 0;
    }

    [ProtoContract]
    public class SpriteAttackData
    {
        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int roleX = 0;

        [ProtoMember(3)]
        public int roleY = 0;

        [ProtoMember(4)]
        public int enemy = 0;

        [ProtoMember(5)]
        public int enemyX = 0;

        [ProtoMember(6)]
        public int enemyY = 0;

        [ProtoMember(7)]
        public int realEnemyX = 0;

        [ProtoMember(8)]
        public int realEnemyY = 0;

        [ProtoMember(9)]
        public int magicCode = 0;

        [ProtoMember(10)]
        public int director = 0;

        [ProtoMember(11)]
        public int magicLevel = 0;
    }

    [ProtoContract]
    public class SpriteActionData
    {
        public SpriteActionData() { }

        public SpriteActionData(int roleID, int mapCode, int direction, int action, int toX, int toY, int targetX, int targetY, int yAngle, int moveToX, int moveToY)
        {
            this.roleID = roleID;
            this.mapCode = mapCode;
            this.direction = direction;
            this.action = action;
            this.toX = toX;
            this.toY = toY;
            this.targetX = targetX;
            this.targetY = targetY;
            this.yAngle = yAngle;
            this.moveToX = moveToX;
            this.moveToY = moveToY;
        }

        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int mapCode = 0;

        [ProtoMember(3)]
        public int direction = 0;

        [ProtoMember(4)]
        public int action = 0;

        [ProtoMember(5)]
        public int toX = 0;

        [ProtoMember(6)]
        public int toY = 0;

        [ProtoMember(7)]
        public int targetX = 0;

        [ProtoMember(8)]
        public int targetY = 0;

        [ProtoMember(9)]
        public int yAngle = 0;

        [ProtoMember(10)]
        public int moveToX = 0;

        [ProtoMember(11)]
        public int moveToY = 0;
    }

    [ProtoContract]
    public class SpritePositionData
    {
        public SpritePositionData() { }

        public SpritePositionData(int roleId, int mapCode, int toX, int toY, long currentPosTicks)
        {
            this.roleID = roleId;
            this.mapCode = mapCode;
            this.toX = toX;
            this.toY = toY;
            this.currentPosTicks = currentPosTicks;
        }

        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int mapCode = 0;

        [ProtoMember(3)]
        public int toX = 0;

        [ProtoMember(4)]
        public int toY = 0;

        [ProtoMember(5)]
        public long currentPosTicks = 0;
    }

    /// <summary>
    /// ��Ұ��ݵĽ�ɫ�����ݶ���
    /// </summary>
    [ProtoContract]
    public class RoleData 
    {
        /// <summary>
        /// ��ǰ�Ľ�ɫID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// ��ǰ�Ľ�ɫID
        /// </summary>
        [ProtoMember(2)]
        public string RoleName = "";

        /// <summary>
        /// ��ǰ��ɫ���Ա�
        /// </summary>
        [ProtoMember(3)]
        public int RoleSex = 0;

        /// <summary>
        /// ��ɫְҵ
        /// </summary>
        [ProtoMember(4)]
        public int Occupation = 0;

        /// <summary>
        /// ��ɫ����
        /// </summary>
        [ProtoMember(5)]
        public int Level = 1;

        /// <summary>
        /// ��ɫ�����İ���
        /// </summary>
        [ProtoMember(6)]
        public int Faction = 0;

        /// <summary>
        /// �󶨽��
        /// </summary>
        [ProtoMember(7)]
        public int BindMoney = 0;

        /// <summary>
        /// �ǰ󶨽��
        /// </summary>
        [ProtoMember(8)]
        public int Money2 = 0;

        /// <summary>
        /// ��ǰ�ľ���
        /// </summary>
        [ProtoMember(9)]
        public long Experience = 0;

        /// <summary>
        /// ��ǰ��PKģʽ
        /// </summary>
        [ProtoMember(10)]
        public int PKMode = 0;

        /// <summary>
        /// ��ǰ��PKֵ
        /// </summary>
        [ProtoMember(11)]
        public int PKValue = 0;

        /// <summary>
        /// ���ڵĵ�ͼ�ı��
        /// </summary>
        [ProtoMember(12)]
        public int MapCode = 0;

        /// <summary>
        /// ��ǰ���ڵ�λ��X����
        /// </summary>
        [ProtoMember(13)]
        public int PosX = 0;

        /// <summary>
        /// ��ǰ���ڵ�λ��Y����
        /// </summary>
        [ProtoMember(14)]
        public int PosY = 0;

        /// <summary>
        /// ��ǰ�ķ���
        /// </summary>
        [ProtoMember(15)]
        public int RoleDirection = 0;

        /// <summary>
        /// ��ǰ������ֵ
        /// </summary>
        [ProtoMember(16)]
        public int LifeV = 0;

        /// <summary>
        /// ��������ֵ
        /// </summary>
        [ProtoMember(17)]
        public int MaxLifeV = 0;

        /// <summary>
        /// ��ǰ��ħ��ֵ
        /// </summary>
        [ProtoMember(18)]
        public int MagicV = 0;

        /// <summary>
        /// ����ħ��ֵ
        /// </summary>
        [ProtoMember(19)]
        public int MaxMagicV = 0;

        /// <summary>
        /// ��ǰ��ͷ��
        /// </summary>
        [ProtoMember(20)]
        public int RolePic = 0;

        /// <summary>
        /// ��ǰ��������Ч������
        /// </summary>
        [ProtoMember(21)]
        public int BagNum = 0;
        
        /// <summary>
        /// �·�����
        /// </summary>
        [ProtoMember(24)]
        public int BodyCode;

        /// <summary>
        /// ��������
        /// </summary>
        [ProtoMember(25)]
        public int WeaponCode;


        /// <summary>
        /// ���������ӳ��
        /// </summary>
        [ProtoMember(29)]
        public string MainQuickBarKeys = "";

        /// <summary>
        /// �����������ӳ��
        /// </summary>
        [ProtoMember(30)]
        public string OtherQuickBarKeys = "";

        /// <summary>
        /// ��½�Ĵ���
        /// </summary>
        [ProtoMember(31)]
        public int LoginNum = 0;

        /// <summary>
        /// ��ֵ��Ǯ��--��ʯ
        /// </summary>
        [ProtoMember(32)]
        public int Gold = 0;

        /// <summary>
        /// ��̯������
        /// </summary>
        [ProtoMember(33)]
        public string StallName;

        /// <summary>
        /// ��ӵ�ID
        /// </summary>
        [ProtoMember(34)]
        public int TeamID;

        /// <summary>
        /// ʣ����Զ��һ�ʱ��
        /// </summary>
        [ProtoMember(35)]
        public int LeftFightSeconds = 0;

        /// <summary>
        /// ӵ�е����������
        /// </summary>
        [ProtoMember(36)]
        public int TotalHorseCount = 0;

        /// <summary>
        /// ��������(��ǰ���)
        /// </summary>
        [ProtoMember(37)]
        public int HorseDbID = -1;

        /// <summary>
        /// ӵ�еĳ��������
        /// </summary>
        [ProtoMember(38)]
        public int TotalPetCount = 0;

        /// <summary>
        /// ��������(��ǰ�ų�)
        /// </summary>
        [ProtoMember(39)]
        public int PetDbID = -1;

        /// <summary>
        /// ��ɫ������ֵ
        /// </summary>
        [ProtoMember(40)]
        public int InterPower = 0;

        /// <summary>
        /// ��ǰ������еĶӳ�ID
        /// </summary>        
        [ProtoMember(41)]
        public int TeamLeaderRoleID = 0;

        /// <summary>
        ///  ϵͳ�󶨵�����
        /// </summary>
        [ProtoMember(42)]
        public int Money = 0;

        /// <summary>
        ///  ��ǰ����������
        /// </summary>
        [ProtoMember(43)]
        public int JingMaiBodyLevel = 0;

        /// <summary>
        ///  ��ǰ�������ۼ�Ѩλ����
        /// </summary>
        [ProtoMember(44)]
        public int JingMaiXueWeiNum = 0;

        /// <summary>
        /// ��һ�ε�����ID
        /// </summary>
        [ProtoMember(45)]
        public int LastHorseID = 0;

        /// <summary>
        /// ȱʡ�ļ���ID
        /// </summary>
        [ProtoMember(46)]
        public int DefaultSkillID = -1;

        /// <summary>
        /// �Զ���Ѫ��ҩ�İٷֱ�
        /// </summary>
        [ProtoMember(47)]
        public int AutoLifeV = 0;

        /// <summary>
        /// �Զ�������ҩ�İٷֱ�
        /// </summary>
        [ProtoMember(48)]
        public int AutoMagicV = 0;


        /// <summary>
        /// �Ѿ���ͨ�ľ���������
        /// </summary>
        [ProtoMember(51)]
        public int JingMaiOkNum = 0;

        /// <summary>
        /// �Զ����������ȵı�������ID
        /// </summary>
        [ProtoMember(53)]
        public int NumSkillID = 0;
     /// <summary>
        /// ����������ȡ����
        /// </summary>
        [ProtoMember(55)]
        public int NewStep = 0;

        /// <summary>
        /// ��ȡ��һ�������������ʱ��
        /// </summary>
        [ProtoMember(56)]
        public long StepTime = 0;

        /// <summary>
        /// �󽱻ID
        /// </summary>
        [ProtoMember(57)]
        public int BigAwardID = 0;

        /// <summary>
        /// ����ID
        /// </summary>
        [ProtoMember(58)]
        public int SongLiID = 0;


        /// <summary>
        /// �ܹ�ѧϰ���ܵļ���
        /// </summary>
        [ProtoMember(60)]
        public int TotalLearnedSkillLevelCount = 0;

        /// <summary>
        /// ��ǰ�Ѿ���ɵ���������ID
        /// </summary>
        [ProtoMember(61)]
        public int CompletedMainTaskID = 0;

        /// <summary>
        /// ��ǰ��PK��
        /// </summary>
        [ProtoMember(62)]
        public int PKPoint = 0;

        /// <summary>
        /// �����ն��
        /// </summary>
        [ProtoMember(63)]
        public int LianZhan = 0;

        /// <summary>
        /// �����Ŀ�ʼʱ��
        /// </summary>
        [ProtoMember(64)]
        public long StartPurpleNameTicks = 0;


        /// <summary>
        /// �Ƕ��������ƺſ�ʼʱ��
        /// </summary>
        [ProtoMember(66)]
        public long BattleNameStart = 0;

        /// <summary>
        /// �Ƕ��������ƺ�
        /// </summary>
        [ProtoMember(67)]
        public int BattleNameIndex = 0;

        /// <summary>
        /// ��ֵTaskID
        /// </summary>
        [ProtoMember(68)]
        public int CZTaskID = 0;

        /// <summary>
        /// Ӣ�����޵Ĳ���
        /// </summary>
        [ProtoMember(69)]
        public int HeroIndex = 0;

        /// <summary>
        /// ȫ��Ʒ�ʵļ���
        /// </summary>
        [ProtoMember(70)]
        public int AllQualityIndex = 0;

        /// <summary>
        /// ȫ�׶��켶��
        /// </summary>
        [ProtoMember(71)]
        public int AllForgeLevelIndex = 0;

        /// <summary>
        /// ȫ�ױ�ʯ����
        /// </summary>
        [ProtoMember(72)]
        public int AllJewelLevelIndex = 0;

        /// <summary>
        /// �����۰��Ż�
        /// </summary>
        [ProtoMember(73)]
        public int HalfYinLiangPeriod = 0;

        /// <summary>
        /// ��ID
        /// </summary>
        [ProtoMember(74)]
        public int ZoneID = 0;

        /// <summary>
        /// ս������
        /// </summary>
        [ProtoMember(75)]
        public string BHName = "";

        /// <summary>
        /// ���������ս��ʱ�Ƿ���֤
        /// </summary>
        [ProtoMember(76)]
        public int BHVerify = 0;

        /// <summary>
        /// ս��ְ��
        /// </summary>
        [ProtoMember(77)]
        public int BHZhiWu = 0;

        /// <summary>
        /// ս�˰ﹱ
        /// </summary>
        [ProtoMember(78)]
        public int BangGong = 0;

        /// <summary>
        /// ��ǰ���Ļʵ۵�ID
        /// </summary>
        [ProtoMember(80)]
        public int HuangDiRoleID = 0;

        /// <summary>
        /// �Ƿ�ʺ�
        /// </summary>
        [ProtoMember(81)]
        public int HuangHou = 0;

        /// <summary>
        /// �Լ��������е�λ���ֵ�
        /// </summary>
        [ProtoMember(82)]
        public Dictionary<int, int> PaiHangPosDict = null;

        /// <summary>
        /// �Ƿ�����˹һ�����״̬
        /// </summary>
        [ProtoMember(83)]
        public int AutoFightingProtect = 0;

        /// <summary>
        /// ��ʦ�Ļ��ܿ�ʼ��ʱ��
        /// </summary>
        [ProtoMember(84)]
        public long FSHuDunStart = 0;

        /// <summary>
        /// ���Ҷ��е���ӪID
        /// </summary>
        [ProtoMember(85)]
        public int BattleWhichSide = -1;

        /// <summary>
        /// �ϴε�mailID
        /// </summary>
        [ProtoMember(86)]
        public int LastMailID = 0;

        /// <summary>
        /// �ϴε�mailID
        /// </summary>
        [ProtoMember(87)]
        public int IsVIP = 0;

        /// <summary>
        /// ���ν�����¼��־λ
        /// </summary>
        [ProtoMember(88)]
        public long OnceAwardFlag = 0;

        /// <summary>
        ///  ����ʯ
        /// </summary>
        [ProtoMember(89)]
        public int BindGold = 0;

        /// <summary>
        /// ���������ʱ��
        /// </summary>
        [ProtoMember(90)]
        public long DSHideStart = 0;

        /// <summary>
        /// ��ɫ�������β���ֵ�б�
        /// </summary>
        [ProtoMember(91)]
        public List<int> RoleCommonUseIntPamams = new List<int>();

        /// <summary>
        /// ��ʦ�Ļ��ܳ���������
        /// </summary>
        [ProtoMember(92)]
        public int FSHuDunSeconds = 0;

        /// <summary>
        /// �ж���ʼ��ʱ��
        /// </summary>
        [ProtoMember(93)]
        public long ZhongDuStart = 0;

        /// <summary>
        /// �ж�����������
        /// </summary>
        [ProtoMember(94)]
        public int ZhongDuSeconds = 0;

        /// <summary>
        /// ��������
        /// </summary>
        [ProtoMember(95)]
        public string KaiFuStartDay = "";

        /// <summary>
        /// ע������
        /// </summary>
        [ProtoMember(96)]
        public string RegTime = "";

        /// <summary>
        /// ���ջ��ʼ����
        /// </summary>
        [ProtoMember(97)]
        public string JieriStartDay = "";

        /// <summary>
        /// ���ջ��������
        /// </summary>
        [ProtoMember(98)]
        public int JieriDaysNum = 0;

        /// <summary>
        /// �������ʼʱ��
        /// </summary>
        [ProtoMember(99)]
        public string HefuStartDay = "";

        /// <summary>
        /// ���ճƺ�
        /// </summary>
        [ProtoMember(100)]
        public int JieriChengHao = 0;

        /// <summary>
        /// ������ʼʱ��
        /// </summary>
        [ProtoMember(101)]
        public string BuChangStartDay = "";

        /// <summary>
        /// ���Ὺʼ��ʱ��
        /// </summary>
        [ProtoMember(102)]
        public long DongJieStart = 0;

        /// <summary>
        /// �������������
        /// </summary>
        [ProtoMember(103)]
        public int DongJieSeconds = 0;


        /// <summary>
        /// �¶ȳ齱���ʼ����
        /// </summary>
        [ProtoMember(104)]
        public string YueduDazhunpanStartDay = "";

        /// <summary>
        /// �¶ȳ齱���������
        /// </summary>
        [ProtoMember(105)]
        public int YueduDazhunpanStartDayNum = 0;


        // ���Ը��� ����һ������ [8/15/2013 LiaoWei]
        /// <summary>
        /// ����
        /// </summary>
        [ProtoMember(106)]
        public int RoleStrength = 0;

        /// <summary>
        /// ����
        /// </summary>
        [ProtoMember(107)]
        public int RoleIntelligence = 0;

        /// <summary>
        /// ����
        /// </summary>
        [ProtoMember(108)]
        public int RoleDexterity = 0;

        /// <summary>
        /// ����
        /// </summary>
        [ProtoMember(109)]
        public int RoleConstitution = 0;

        // ת������ [10/17/2013 LiaoWei]
        [ProtoMember(110)]
        public int ChangeLifeCount = 0;

        // �����Ե� [10/17/2013 LiaoWei]
        [ProtoMember(111)]
        public int TotalPropPoint = 0;

        // ���˱�� [10/17/2013 LiaoWei]
        [ProtoMember(112)]
        public int IsFlashPlayer = 0;

        // ����ݼ���[12/10/2013 LiaoWei]
        [ProtoMember(113)]
        public int AdmiredCount = 0;

        // ս���� [12/17/2013 LiaoWei]
        [ProtoMember(114)]
        public int CombatForce = 0;

        // ��ݼ���[12/10/2013 LiaoWei]
        [ProtoMember(115)]
        public int AdorationCount = 0;

        // ÿ������ʱ�� [1/18/2014 LiaoWei]
        [ProtoMember(116)]
        public int DayOnlineSecond = 0;

        // ������½����(1-7) [1/18/2014 LiaoWei]
        [ProtoMember(117)]
        public int SeriesLoginNum = 0;

        // �Զ��������Ե� [3/3/2014 LiaoWei] 
        [ProtoMember(118)]
        public int AutoAssignPropertyPoint = 0;

        // ������ʱ�� [3/3/2014 LiaoWei] 
        [ProtoMember(119)]
        public int OnLineTotalTime = 0;

        /// <summary>
        /// ȫ��׿Խ����װ������
        /// </summary>
        [ProtoMember(120)]
        public int AllZhuoYueNum = 0;

        /// <summary>
        /// VIP�ȼ� [3/27/2014 LiaoWei]
        /// </summary>
        [ProtoMember(121)]
        public int VIPLevel = 0;

        /// <summary>
        /// �������������Ӽ�ʱ [4/4/2014 LiaoWei]
        /// </summary>
        [ProtoMember(122)]
        public int OpenGridTime = 0;

        /// <summary>
        /// �����ƶ��������Ӽ�ʱ [4/4/2014 LiaoWei]
        /// </summary>
        [ProtoMember(123)]
        public int OpenPortableGridTime = 0;


        //�˵��ƶ��ٶ�
        [ProtoMember(127)]
        public double MoveSpeedPerSec = 0;

    }

}
