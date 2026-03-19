using UnityEngine;
using System.Collections.Generic;

//全局游戏状态
// 全局状态（运行时保存，比如当前天数、好评数）
[System.Serializable]
public class GameState
{
    public GameDay currentDay = GameDay.Day1; // 当前天数
    public int totalGoodReviews = 0;          // 总好评数（目标20）
    public int fragmentCount = 0;             // 天宫录碎片数（目标3）
    public bool[] unlockedComponents;         // 已解锁的组件（对应FireworkComponent枚举）
    public bool isDayCompleted;               // 当天是否完成

    // 初始化状态（游戏开始时调用）
    public void InitState(int componentCount)
    {
        unlockedComponents = new bool[componentCount];
        // 第一天默认解锁普通火药、短引线、圆柱外壳等基础组件
        unlockedComponents[(int)FireworkComponent.ExplodePowder] = true;
        unlockedComponents[(int)FireworkComponent.ShortFuse] = true;
        unlockedComponents[(int)FireworkComponent.CylinderShell] = true;

        totalGoodReviews = 0;
        fragmentCount = 0;
        isDayCompleted = false;
    }
}