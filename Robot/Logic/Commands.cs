using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Logic
{
    /// 用户命令
    public enum TCPGameServerCmds
    {
        CMD_LOGIN_ON = 100,
        CMD_ROLE_LIST=101,
        CMD_CREATE_ROLE = 102,
        CMD_INIT_GAME = 104,
        CMD_SYNC_TIME = 105,
        CMD_PLAY_GAME=106,
        CMD_SPR_MOVE=107,
        CMD_SPR_MOVEEND=108,
        CMD_SPR_POSITION=112,
        CMD_SPR_ACTTION=114, 
        CMD_SPR_MAGICCODE=116,
        CMD_SPR_ATTACK=117,
        CMD_SPR_INJURE=118,
        CMD_SPR_REALIVE=119,
        CMD_SPR_MAPCHANGE = 123,
        CMD_LOG_OUT =124,
        CMD_SPR_CHAT=157,
        CMD_SPR_CLIENTHEART =255,
        CMD_SPR_ATTACK_ACTION = 614,
    }

    public enum GActions
    {
        /// <summary>
        /// 安全区站立
        /// </summary>
        Stand = 0,
        /// <summary>
        /// 安全区休闲
        /// </summary>     
        Idle = 1,
        /// <summary>
        /// 安全区行走
        /// </summary>
        Walk = 2,
        /// <summary>
        /// 野外持枪站立
        /// </summary>
        ReadyStand = 3,
        /// <summary>
        /// 野外持枪休闲
        /// </summary>
        ReadyIdle = 4,
        /// <summary>
        /// 野外持枪奔跑
        /// </summary>       
        ReadyWalk = 5,
        /// <summary>
        /// 战斗攻击待机，战斗状态下的待机动作
        /// </summary>    
        FightStand = 6,
        /// <summary>
        /// 战斗休闲（现在是换子弹）
        /// </summary>    
        FightIdle = 7,
        /// <summary>
        /// 战斗行走，锁定目标的前后左右行走
        /// </summary> 
        FightWalk = 8,
        /// <summary>
        /// 正常的普通
        /// </summary>  
        FightAttack = 9,
        /// <summary>
        /// 技能1 
        /// </summary> 
        FightSkill1 = 10,
        /// <summary>
        /// 技能2
        /// </summary> 
        FightSkill2 = 11,
        /// <summary>
        /// 技能3
        /// </summary>
        FightSkill3 = 12,
        /// <summary>
        /// 技能4
        /// </summary>
        FightSkill4 = 13,
        /// <summary>
        /// 技能5,BUFF技能
        /// </summary>
        FightSkill5 = 14,
        /// <summary>
        /// 瞬移技
        /// </summary>
        FightRush = 15,
        /// <summary>
        /// 受伤
        /// </summary>     
        Hit = 16,
        /// <summary>
        /// 击飞
        /// </summary>
        HitFly = 17,
        /// <summary>
        /// 采集
        /// </summary>       
        Pick = 18,
        /// <summary>
        /// 受伤状态，但不死亡
        /// </summary>       
        Injured = 19,
        /// <summary>
        /// 死亡
        /// </summary>        
        Death = 20,
        MaxAction = 21,     //  最大的动作

    }
}
