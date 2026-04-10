using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏全局状态数据，记录当前进度、解锁内容、评价和货币等信息。
/// </summary>
[System.Serializable] // 允许序列化，使数据可保存或在Inspector中显示
public class GameState
{
    [Tooltip("当前游戏天数，默认 Day1")]
    public GameDay currentDay = GameDay.Day1;

    [Tooltip("累计好评数，达到目标可通关")]
    public int totalGoodReviews = 0;

    [Tooltip("碎片数量，达到目标可解锁奖励")]
    public int fragmentCount = 0;
    
    [Tooltip("当前金币数量")]
    public int money;

    [Tooltip("每种烟花组件是否已解锁")]
    public bool[] unlockedComponents;

    [Tooltip("当前天是否已完成")]
    public bool isCurrentDayCompleted = false;

    /// <summary>
    /// 初始化游戏状态，将指定组件设为已解锁，并重置所有进度数据。
    /// </summary>
    public void Init(IEnumerable<FireworkComponent> initialUnlockedComponents = null)
    {
        // 初始化解锁数组，长度为所有组件枚举数量
        unlockedComponents = new bool[System.Enum.GetValues(typeof(FireworkComponent)).Length];

        // 根据配置解锁初始组件
        if (initialUnlockedComponents != null)
        {
            foreach (FireworkComponent component in initialUnlockedComponents)
            {
                UnlockComponent(component);
            }
        }

        // 重置统计数据
        totalGoodReviews = 0;
        fragmentCount = 0;
        money = 0;
        currentDay = GameDay.Day1;
        isCurrentDayCompleted = false;
    }

    /// <summary>
    /// 将指定组件解锁。
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
    /// 检查指定组件是否已解锁。
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