using UnityEngine;
using System.Collections.Generic;

//每日解锁模板
[CreateAssetMenu(fileName = "DayUnlock_", menuName = "Game/Day Unlock")]
public class DayUnlockSO : ScriptableObject
{
    public GameDay day;                       // 对应天数
    public List<FireworkComponent> unlockComponents; // 当天解锁的组件
    public int guestCount = 3;                // 普通客人数量
    public List<GuestDemandSO> specialNPCs;   // 当天的特殊NPC
}