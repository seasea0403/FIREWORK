using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏全局管理器：管理游戏状态、天数切换、奖励、好评和碎片统计。
/// 定义了核心函数：AddMoney、AddGoodReview、AddFragment、NextDay、ContinueToNextDay。
/// </summary>
[DefaultExecutionOrder(-100)] // 确保 GameManager 最先执行 Awake
public class GameManager : Singleton<GameManager>
{
    [Header("初始解锁配置")]
    public List<FireworkComponent> initialUnlockedComponents = new List<FireworkComponent>
    {
        FireworkComponent.NormalPowder,
        FireworkComponent.ShortFuse,
        FireworkComponent.Clay,
        FireworkComponent.CylinderShell,
        FireworkComponent.PyramidShell,
        FireworkComponent.RoundShell,
        FireworkComponent.RedPaper,
        FireworkComponent.YellowPaper,
        FireworkComponent.BluePaper,
        FireworkComponent.GreenPaper
    };

    // 核心：唯一的游戏状态实例（运行时状态，不作为 Inspector 配置入口）
    [HideInInspector]
    public GameState gameState;

    /// <summary>
    /// 单例初始化并创建游戏全局状态实例，执行初始状态配置。
    /// </summary>
    protected override void Awake()
    {
        base.Awake(); // 必须调用基类单例逻辑

        // 初始化GameState实例
        gameState = new GameState();
        gameState.Init(initialUnlockedComponents); // 按配置初始化初始解锁组件

        Debug.Log("GameManager初始化完成！初始状态：" +
                  $"Day{(int)gameState.currentDay}，已解锁组件数：{gameState.unlockedComponents.Length}");
    }

    #region 状态操作方法（封装GameState的操作，外部统一调用）
    /// <summary>
    /// 增加金币（客人交付奖励）
    /// </summary>
    public void AddMoney(int value)
    {
        gameState.money += value;
        Debug.Log($"【全局状态】获得金币：{value}，当前总金币：{gameState.money}");
    }
    /// <summary>
    /// 增加好评数（全局调用）
    /// </summary>
    public void AddGoodReview()
    {
        gameState.totalGoodReviews++;
        // 同时更新结算管理器的当天好评统计
        if (SettlementManager.Instance != null)
        {
            SettlementManager.Instance.AddDayGoodReview();
        }
        Debug.Log($"【全局状态】好评数+1，当前总好评：{gameState.totalGoodReviews}");

        // 可扩展：好评数达标触发奖励/剧情
        if (gameState.totalGoodReviews >= 20)
        {
            Debug.Log("🎉 好评数达标！解锁隐藏剧情！");
        }
    }

    /// <summary>
    /// 增加碎片（特殊NPC判定完美时调用）
    /// </summary>
    public void AddFragment()
    {
        gameState.fragmentCount++;
        Debug.Log($"【全局状态】碎片+1，当前碎片：{gameState.fragmentCount}");

        // 可扩展：碎片达标触发结局
        if (gameState.fragmentCount >= 3)
        {
            Debug.Log("🔮 碎片收集完成！解锁真结局！");
        }
    }

    /// <summary>
    /// 切换到下一天
    /// </summary>
    public void NextDay()
    {
        // 显示结算页面，而不是直接切换天数
        if (SettlementManager.Instance != null)
        {
            SettlementManager.Instance.ShowSettlement();
        }
        else
        {
            // 如果没有结算管理器，直接继续到下一天
            ContinueToNextDay();
        }
    }

    /// <summary>
    /// 继续到下一天（从结算页面调用）
    /// </summary>
    public void ContinueToNextDay()
    {
        // 先标记当天完成
        gameState.isCurrentDayCompleted = true;

        // 切换天数（GameDay是枚举，需确保定义）
        if (gameState.currentDay < GameDay.Day5)
        {
            gameState.currentDay++;
            // 切换天数后重置"当天完成"状态
            gameState.isCurrentDayCompleted = false;

            // 重置当天统计数据
            if (SettlementManager.Instance != null)
            {
                SettlementManager.Instance.ResetDayStats();
            }

            // 通知GuestManager加载新一天的客人
            if (GuestManager.Instance != null)
            {
                GuestManager.Instance.LoadDayGuests((int)gameState.currentDay + 1);
            }

            // 下一个天时重置纸筒并移动到工作台位置
            if (PaperTubeManager.Instance != null)
            {
                PaperTubeManager.Instance.ResetAndMoveNewTube();
            }

            Debug.Log($"【全局状态】切换到Day{(int)gameState.currentDay}");
        }
        else
        {
            Debug.Log("【全局状态】已到最后一天！");
        }
    }

    #endregion
}