using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏全局状态（记录运行时的动态数据，比如当前天数、好评数）
/// </summary>
[System.Serializable] // 支持序列化，方便存档
public class GameState
{
    [Tooltip("当前所在天数（默认Day1）")]
    public GameDay currentDay = GameDay.Day1;

    [Tooltip("总好评数（目标20个，用于通关）")]
    public int totalGoodReviews = 0;

    [Tooltip("天宫录碎片数（目标3块，用于通关）")]
    public int fragmentCount = 0;

    [Tooltip("已解锁的组件（true=已解锁，对应FireworkComponent枚举）")]
    public bool[] unlockedComponents;

    [Tooltip("当天是否已完成（完成后可切换下一天）")]
    public bool isCurrentDayCompleted = false;

    /// <summary>
    /// 初始化状态（游戏启动时调用）
    /// </summary>
    public void Init()
    {
        // 初始化已解锁组件数组（长度=FireworkComponent枚举的总数量）
        unlockedComponents = new bool[System.Enum.GetValues(typeof(FireworkComponent)).Length];

        // 第一天默认解锁的基础组件（按策划的新手引导配置）
        UnlockComponent(FireworkComponent.NormalPowder);
        UnlockComponent(FireworkComponent.ShortFuse);
        UnlockComponent(FireworkComponent.Clay);
        UnlockComponent(FireworkComponent.RedBead);
        UnlockComponent(FireworkComponent.BlueBead);
        UnlockComponent(FireworkComponent.GreenBead);
        UnlockComponent(FireworkComponent.GoldBead);
        UnlockComponent(FireworkComponent.CylinderShell);
        UnlockComponent(FireworkComponent.PyramidShell);
        UnlockComponent(FireworkComponent.RoundShell);
        UnlockComponent(FireworkComponent.RedPaper);
        UnlockComponent(FireworkComponent.YellowPaper);
        UnlockComponent(FireworkComponent.GreenPaper);

        // 重置其他状态
        totalGoodReviews = 0;
        fragmentCount = 0;
        currentDay = GameDay.Day1;
        isCurrentDayCompleted = false;
    }

    /// <summary>
    /// 解锁指定组件
    /// </summary>
    public void UnlockComponent(FireworkComponent component)
    {
        int index = (int)component;
        if (index >= 0 && index < unlockedComponents.Length)
        {
            unlockedComponents[index] = true;
        }
    }

    /// <summary>
    /// 检查组件是否已解锁
    /// </summary>
    public bool IsComponentUnlocked(FireworkComponent component)
    {
        int index = (int)component;
        if (index >= 0 && index < unlockedComponents.Length)
        {
            return unlockedComponents[index];
        }
        return false;
    }
}