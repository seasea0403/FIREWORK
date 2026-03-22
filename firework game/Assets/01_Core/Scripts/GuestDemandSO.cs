using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 客人需求数据模板（存储单个客人的所有信息，策划可直接在编辑器配置）
/// </summary>
[CreateAssetMenu(fileName = "GuestDemand_", menuName = "Game/Guest Demand")]
public class GuestDemandSO : ScriptableObject
{
    [Header("🎎 基础信息")]
    [Tooltip("客人名称")]
    public string guestName;

    [Tooltip("客人需求描述")]
    [TextArea(2, 4)] // 多行输入，方便填写长文本
    public string demandDesc;

    [Tooltip("完美评价的报酬金额")]
    public int perfectMoney = 100;

    [Tooltip("良好评价的报酬金额")]
    public int goodMoney = 60;

    [Tooltip("失败评价的报酬金额）")]
    public int failMoney = 20;

    [Tooltip("超时的报酬金额）")]
    public int timeoutMoney =0 ;

    [Tooltip("完美评价台词")]
    [TextArea(1, 2)]
    // 给完美台词加默认值
    public string perfectText = "神乎其技！比某想的还要好，下回定当再来光顾！";

    [Tooltip("良好评价台词")]
    [TextArea(1, 2)]
    // 给良好台词加默认值
    public string goodText = "尚可尚可，勉强合某心意，赏你些碎银！";

    [Tooltip("差评台词")]
    [TextArea(1, 2)]
    // 给差评台词也加默认值，更实用
    public string failText = "货不对板！这般手艺也敢出来做生意？扣钱！";

    [Tooltip("时间台词")]
    [TextArea(1, 2)]
    // 给差评台词也加默认值，更实用
    public string timeoutText = "这……做得也太慢了，某去别家看看。";

    [Tooltip("是否是特殊NPC（比如顾文才、英娥）")]
    public bool isSpecialNPC;

    // ========== 新增：客人2D形象配置（直接拖入Sprite） ==========
    [Header("🎨 客人2D形象")]
    [Tooltip("基础状态形象（初始显示）")]
    public Sprite normalSprite;

    [Tooltip("高兴状态形象（完美/良好判定时显示）")]
    public Sprite happySprite;

    [Tooltip("生气状态形象（失败/超时判定时显示）")]
    public Sprite angrySprite;

    [Header("✅ 判定规则（严格按策划配置）")]
    [Tooltip("核心判定-指定单个：必须包含这些组件（比如“必须是红彩珠”）")]
    public List<FireworkComponent> coreSpecificComponents = new List<FireworkComponent>();

    [Tooltip("核心判定-类别任选：每个类别至少选一个（比如“彩珠类选任意一个”）")]
    public List<ComponentCategory> coreCategoryAnyOne = new List<ComponentCategory>();

    [Tooltip("次要判定-指定单个：包含这些组件加分（全满足则完美）")]
    public List<FireworkComponent> secondarySpecificComponents = new List<FireworkComponent>();

    [Tooltip("次要判定-类别任选：每个类别至少选一个加分（全满足则完美）")]
    public List<ComponentCategory> secondaryCategoryAnyOne = new List<ComponentCategory>();

    [Tooltip("禁止组件：绝对不能包含的组件")]
    public List<FireworkComponent> forbiddenComponents = new List<FireworkComponent>();

    [Header("🎁 特殊奖励（仅特殊NPC生效）")]
    [Tooltip("是否发放天宫录碎片（特殊NPC全做对才掉落")]
    public bool giveFragment;

    [Tooltip("碎片数量（默认1块）")]
    public int fragmentCount = 1;
}