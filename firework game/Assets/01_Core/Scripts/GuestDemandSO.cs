using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 客人需求数据模板（存储单个客人的所有信息，策划可直接在编辑器配置）
/// </summary>
[CreateAssetMenu(fileName = "GuestDemand_", menuName = "Game/Guest Demand")]
public class GuestDemandSO : ScriptableObject
{
    [Header("🎎 基础信息")]
    [Tooltip("客人名称（比如“普通客人1”“顾文才”）")]
    public string guestName;

    [Tooltip("客人需求描述（显示给玩家，比如“要个最寻常的响炮仗”）")]
    [TextArea(2, 4)] // 多行输入，方便填写长文本
    public string demandDesc;

    [Tooltip("完美评价的报酬金额")]
    public int perfectMoney;

    [Tooltip("良好评价的报酬金额")]
    public int goodMoney;

    [Tooltip("失败评价的报酬金额（策划要求“钱少”，可设为完美的1/3）")]
    public int failMoney;

    [Tooltip("好评台词（比如“神乎其技！”）")]
    [TextArea(1, 2)]
    public string praiseText;

    [Tooltip("差评台词（比如“货不对板！”）")]
    [TextArea(1, 2)]
    public string criticizeText;

    [Tooltip("是否是特殊NPC（比如顾文才、英娥）")]
    public bool isSpecialNPC;

    // ========== 新增：客人2D形象配置（直接拖入Sprite） ==========
    [Header("🎨 客人2D形象（直接拖入图片资源）")]
    [Tooltip("基础状态形象（初始显示）")]
    public Sprite normalSprite;

    [Tooltip("高兴状态形象（完美/良好判定时显示）")]
    public Sprite happySprite;

    [Tooltip("生气状态形象（失败/超时判定时显示）")]
    public Sprite angrySprite;

    [Header("✅ 判定规则（严格按策划配置）")]
    [Tooltip("核心判定：必须全满足，否则失败（比如“普通火药+短引线”）")]
    public List<FireworkComponent> coreRequirements = new List<FireworkComponent>();

    [Tooltip("次要判定：部分满足即可（比如“红色彩纸”），全满足则完美")]
    public List<FireworkComponent> secondaryRequirements = new List<FireworkComponent>();

    [Tooltip("禁止组件：不能添加的组件（比如“不要彩珠”）")]
    public List<FireworkComponent> forbiddenComponents = new List<FireworkComponent>();

    [Header("🎁 特殊奖励（仅特殊NPC生效）")]
    [Tooltip("是否发放天宫录碎片（策划要求“特殊NPC全做对才掉落”）")]
    public bool giveFragment;

    [Tooltip("碎片数量（默认1块）")]
    public int fragmentCount = 1;
}