using UnityEngine;
using System.Collections.Generic;

// 每日解锁配置
[CreateAssetMenu(fileName = "DayUnlock_", menuName = "Game/Day Unlock")]
public class DayUnlockSO : ScriptableObject
{
    public GameDay day;                       // 对应的游戏天数
    public List<FireworkComponent> unlockComponents; // 本日解锁的组件
    public int guestCount = 3;                // 当日客人数量
    public List<GuestDemandSO> specialNPCs;   // 特殊NPC配置
}