using UnityEngine;
using System.Collections.Generic;

//客人需求模板
[CreateAssetMenu(fileName = "GuestDemand_", menuName = "Game/Guest Demand")]
public class GuestDemandSO : ScriptableObject
{
    [Header("基础信息")]
    public string guestName;          // 客人名称（比如“顾文才”）
    public string demandDesc;         // 需求描述（显示给玩家）
    public int moneyReward;           // 报酬金额
    public string PerfectText;         // 好评台词
    public string GoodText;             //良好台词
    public string FailText;            // 差评台词
    public string TimeOutText;            // 时间到台词
    public bool isSpecialNPC;         // 是否是特殊NPC（比如顾文才、英娥）

    [Header("判定规则")]
    public List<FireworkComponent> coreRequirements;   // 核心判定（必须全满足）
    public List<FireworkComponent> secondaryRequirements; // 次要判定（部分满足即可）
    public List<FireworkComponent> forbiddenComponents; // 禁止组件（不能加）

    [Header("特殊奖励")]
    public bool giveFragment;         // 是否发放天宫录碎片
    public int fragmentCount = 1;     // 碎片数量
}