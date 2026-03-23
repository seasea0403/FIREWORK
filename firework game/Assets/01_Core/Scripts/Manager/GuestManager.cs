using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 客人管理器（继承泛型单例，全局唯一）
/// </summary>
public class GuestManager : Singleton<GuestManager>
{
    // 当前接待的客人
    public GuestDemandSO currentGuest;

    // Day1的普通客人列表（可在Inspector赋值）
    public List<GuestDemandSO> day1Guests;

    // 初始化：加载Day1的第一个客人
    protected override void Awake()
    {
        base.Awake();
        LoadDayGuests(1);
        Debug.Log("GuestManager初始化完成！");
    }

    /// <summary>
    /// 加载指定天数的客人列表
    /// </summary>
    /// <param name="day">天数</param>
    public void LoadDayGuests(int day)
    {
        // 简化版：仅处理Day1，后续可扩展Day2-Day5
        if (day == 1 && day1Guests != null && day1Guests.Count > 0)
        {
            currentGuest = day1Guests[0];
            Debug.Log($"加载Day1第一个客人：{currentGuest.guestName}");
        }
        else
        {
            Debug.LogWarning($"暂无Day{day}的客人数据！");
        }
    }

    /// <summary>
    /// 接待下一个客人
    /// </summary>
    public void NextGuest()
    {
        int currentIndex = day1Guests.IndexOf(currentGuest);
        if (currentIndex < day1Guests.Count - 1)
        {
            currentGuest = day1Guests[currentIndex + 1];
            Debug.Log($"切换到下一个客人：{currentGuest.guestName}");
        }
        else
        {
            Debug.Log("Day1客人已接待完毕，切换到下一天！");
            GameManager.Instance.NextDay();
            // 这里可扩展：加载Day2的客人
        }
    }
}