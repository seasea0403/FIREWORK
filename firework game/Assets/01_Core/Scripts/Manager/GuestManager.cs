using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 客人管理器（继承泛型单例，全局唯一）
/// 修正：客人形象改用SpriteRenderer（场景渲染），避免UI图层冲突
/// </summary>
public class GuestManager : Singleton<GuestManager>
{
    [Header("🎎 客人显示配置（Inspector绑定）")]
    [Tooltip("客人2D形象的GameObject（挂载SpriteRenderer，放场景特定位置）")]
    public GameObject guestSpriteObject; // 替换Image为承载SpriteRenderer的物体
    [Tooltip("显示客人名字的UI Text组件")]
    public TMP_Text guestNameText;
    [Tooltip("显示客人需求的UI Text组件")]
    public TMP_Text guestDemandText;
    [Tooltip("客人UI根节点（名字+需求，可控制显示/隐藏）")]
    public GameObject guestUIRoot;

    // 缓存SpriteRenderer组件（避免重复GetComponent）
    private SpriteRenderer m_GuestSpriteRenderer;

    // 当前接待的客人
    public GuestDemandSO currentGuest;
    // Day1的普通客人列表（可在Inspector赋值）
    public List<GuestDemandSO> day1Guests;

    // 初始化：加载Day1的第一个客人 + 初始化SpriteRenderer
    protected override void Awake()
    {
        base.Awake();

        // 初始化SpriteRenderer缓存
        if (guestSpriteObject != null)
        {
            m_GuestSpriteRenderer = guestSpriteObject.GetComponent<SpriteRenderer>();
            // 若无SpriteRenderer则自动添加
            if (m_GuestSpriteRenderer == null)
            {
                m_GuestSpriteRenderer = guestSpriteObject.AddComponent<SpriteRenderer>();
                Debug.LogWarning("客人形象物体无SpriteRenderer，已自动添加！");
            }
        }

        // 初始化UI状态（默认隐藏）
        if (guestUIRoot != null) guestUIRoot.SetActive(false);
        if (guestSpriteObject != null) guestSpriteObject.SetActive(false);

        LoadDayGuests(1);
        Debug.Log("GuestManager初始化完成！");
    }

    /// <summary>
    /// 加载指定天数的客人列表 + 更新显示
    /// </summary>
    /// <param name="day">天数</param>
    public void LoadDayGuests(int day)
    {
        if (day == 1 && day1Guests != null && day1Guests.Count > 0)
        {
            currentGuest = day1Guests[0];
            Debug.Log($"加载Day1第一个客人：{currentGuest.guestName}");
            UpdateGuestDisplay(); // 替换原UpdateGuestUI，包含SpriteRenderer逻辑
        }
        else
        {
            Debug.LogWarning($"暂无Day{day}的客人数据！");
            HideGuestDisplay();
        }
    }

    /// <summary>
    /// 接待下一个客人 + 更新显示
    /// </summary>
    public void NextGuest()
    {
        if (day1Guests == null || day1Guests.Count == 0)
        {
            Debug.LogWarning("Day1客人列表为空！");
            HideGuestDisplay();
            return;
        }

        int currentIndex = day1Guests.IndexOf(currentGuest);
        if (currentIndex < day1Guests.Count - 1)
        {
            currentGuest = day1Guests[currentIndex + 1];
            Debug.Log($"切换到下一个客人：{currentGuest.guestName}");
            UpdateGuestDisplay();
        }
        else
        {
            Debug.Log("Day1客人已接待完毕，切换到下一天！");
            HideGuestDisplay();
            GameManager.Instance.NextDay();
            // 扩展：LoadDayGuests(2);
        }
    }

    /// <summary>
    /// 更新客人显示（SpriteRenderer+UI Text）
    /// </summary>
    private void UpdateGuestDisplay()
    {
        if (currentGuest == null)
        {
            HideGuestDisplay();
            return;
        }

        // 1. 显示客人形象物体
        if (guestSpriteObject != null)
        {
            guestSpriteObject.SetActive(true);
            // 更新SpriteRenderer的sprite（显示normalSprite）
            if (m_GuestSpriteRenderer != null)
            {
                m_GuestSpriteRenderer.sprite = currentGuest.normalSprite;
                // 无图片则隐藏SpriteRenderer（保留物体激活状态）
                m_GuestSpriteRenderer.enabled = currentGuest.normalSprite != null;
                // 可选：设置排序层（解决图层问题）
                // m_GuestSpriteRenderer.sortingLayerName = "Guest"; // 需先在项目中创建该图层
                // m_GuestSpriteRenderer.sortingOrder = 5;
            }
        }
        else
        {
            Debug.LogWarning("未绑定客人形象GameObject！");
        }

        // 2. 显示UI根节点（名字+需求）
        if (guestUIRoot != null) guestUIRoot.SetActive(true);

        // 3. 更新客人名字
        if (guestNameText != null)
        {
            guestNameText.text = currentGuest.guestName;
        }
        else
        {
            Debug.LogWarning("未绑定客人名字Text组件！");
        }

        // 4. 更新客人需求描述
        if (guestDemandText != null)
        {
            guestDemandText.text = currentGuest.demandDesc;
        }
        else
        {
            Debug.LogWarning("未绑定客人需求Text组件！");
        }
    }

    /// <summary>
    /// 隐藏客人显示（无客人/切换天数时调用）
    /// </summary>
    private void HideGuestDisplay()
    {
        // 隐藏形象物体
        if (guestSpriteObject != null)
        {
            guestSpriteObject.SetActive(false);
            if (m_GuestSpriteRenderer != null) m_GuestSpriteRenderer.sprite = null;
        }

        // 隐藏UI
        if (guestUIRoot != null) guestUIRoot.SetActive(false);
        if (guestNameText != null) guestNameText.text = "";
        if (guestDemandText != null) guestDemandText.text = "";
    }

    /// <summary>
    /// 切换客人形象（判定后调用：高兴/生气）
    /// </summary>
    /// <param name="isHappy">true=高兴形象，false=生气形象</param>
    public void SwitchGuestSprite(bool isHappy)
    {
        if (currentGuest == null || m_GuestSpriteRenderer == null) return;

        if (isHappy && currentGuest.happySprite != null)
        {
            m_GuestSpriteRenderer.sprite = currentGuest.happySprite;
        }
        else if (!isHappy && currentGuest.angrySprite != null)
        {
            m_GuestSpriteRenderer.sprite = currentGuest.angrySprite;
        }
        // 无对应形象则保留normalSprite
    }

    /// <summary>
    /// 手动设置客人形象的排序层（解决图层冲突）
    /// </summary>
    /// <param name="layerName">排序层名称</param>
    /// <param name="order">排序顺序</param>
    public void SetGuestSpriteLayer(string layerName, int order = 0)
    {
        if (m_GuestSpriteRenderer == null) return;
        m_GuestSpriteRenderer.sortingLayerName = layerName;
        m_GuestSpriteRenderer.sortingOrder = order;
        Debug.Log($"客人形象排序层已设置：{layerName}，顺序：{order}");
    }
}