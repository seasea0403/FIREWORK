using UnityEngine;

/// <summary>
/// 游戏全局管理器（继承泛型单例，仅管理GameState，不存储状态）
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // 核心：唯一的游戏状态实例（所有状态都存在这里，消除冗余）
    public GameState gameState;

    // 重写Awake：初始化游戏状态
    protected override void Awake()
    {
        base.Awake(); // 必须调用基类单例逻辑

        // 初始化GameState实例
        gameState = new GameState();
        gameState.Init(); // 调用GameState的初始化方法

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
        // 先标记当天完成
        gameState.isCurrentDayCompleted = true;

        // 切换天数（GameDay是枚举，需确保定义）
        if (gameState.currentDay < GameDay.Day5)
        {
            gameState.currentDay++;
            // 切换天数后重置“当天完成”状态
            gameState.isCurrentDayCompleted = false;

            // 可扩展：解锁新组件（比如Day2解锁高级火药）
            UnlockDayComponent(gameState.currentDay);

            Debug.Log($"【全局状态】切换到Day{(int)gameState.currentDay}");
        }
        else
        {
            Debug.Log("【全局状态】已到最后一天！");
        }
    }

    /// <summary>
    /// 按天数解锁组件（扩展逻辑）
    /// </summary>
    private void UnlockDayComponent(GameDay day)
    {
        switch (day)
        {
            case GameDay.Day2:
                gameState.UnlockComponent(FireworkComponent.PushPowder);
                gameState.UnlockComponent(FireworkComponent.LongFuse);
                Debug.Log("Day2解锁：高威力火药、长引线");
                break;
            case GameDay.Day3:
                gameState.UnlockComponent(FireworkComponent.SilverBead);
                gameState.UnlockComponent(FireworkComponent.HeartShell);
                Debug.Log("Day3解锁：银珠、星形外壳");
                break;
                // 可继续扩展Day4/Day5的解锁逻辑
        }
    }
    #endregion
}