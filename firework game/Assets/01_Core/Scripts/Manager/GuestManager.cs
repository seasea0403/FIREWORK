using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

/// <summary>
/// 客人管理器：负责加载当天客人、刷新客人UI、显示评价结果，并在客人切换或当天结束时触发状态切换。
/// 定义了核心函数：LoadDayGuests、ShowJudgeResult、NextGuest、UpdateGuestDisplay、HideGuestDisplay、SwitchGuestSprite。
/// </summary>
public class GuestManager : Singleton<GuestManager>
{
    [Header("客人显示配置（Inspector绑定）")]
    public GameObject guestSpriteObject;
    public TMP_Text guestNameText;
    public TMP_Text guestDemandText;
    public GameObject guestUIRoot;
    //缓存根节点的CanvasGroup（控制整体透明度）
    private CanvasGroup m_UIRootCanvasGroup;

    private SpriteRenderer m_GuestSpriteRenderer;
    public GuestDemandSO currentGuest;
    //客人列表
    public List<GuestDemandSO> day1Guests;
    public List<GuestDemandSO> day2Guests;
    public List<GuestDemandSO> day3Guests;
    public List<GuestDemandSO> day4Guests;
    public List<GuestDemandSO> day5Guests;

    /// <summary>
    /// 初始化客人管理器单例，缓存UI组件并加载第一天客人。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (guestSpriteObject != null)
        {
            m_GuestSpriteRenderer = guestSpriteObject.GetComponent<SpriteRenderer>();
            if (m_GuestSpriteRenderer == null)
                m_GuestSpriteRenderer = guestSpriteObject.AddComponent<SpriteRenderer>();
        }

        // 初始化：获取根节点的CanvasGroup
        if (guestUIRoot != null)
        {
            m_UIRootCanvasGroup = guestUIRoot.GetComponent<CanvasGroup>();
            // 默认隐藏根节点
            guestUIRoot.SetActive(false);
        }
        if (guestSpriteObject != null) guestSpriteObject.SetActive(false);

        LoadDayGuests(1);
        Debug.Log("GuestManager初始化完成！");
    }

    /// <summary>
    /// 根据指定天数加载当天客人列表，并初始化当前客人显示。
    /// 在加载新的一天时同时重置工作台状态。
    /// </summary>
    public void LoadDayGuests(int day)
    {
        List<GuestDemandSO> dayGuests = null;

        switch (day)
        {
            case 1: dayGuests = day1Guests; break;
            case 2: dayGuests = day2Guests; break;
            case 3: dayGuests = day3Guests; break;
            case 4: dayGuests = day4Guests; break;
            case 5: dayGuests = day5Guests; break;
            default:
                Debug.LogWarning($"不支持的天数：{day}");
                HideGuestDisplay();
                return;
        }

        if (dayGuests != null && dayGuests.Count > 0)
        {
            // 加载新一天时重置工作台状态
            if (PaperTube.Instance != null)
            {
                PaperTube.Instance.ClearStack();
                Debug.Log($"加载Day{day}时重置工作台状态");
            }

            currentGuest = dayGuests[0];
            UpdateGuestDisplay();
            Debug.Log($"加载Day{day}的客人数据，共{dayGuests.Count}个客人");
        }
        else
        {
            Debug.LogWarning($"暂无Day{day}的客人数据！");
            HideGuestDisplay();
        }
    }

    /// <summary>
    /// 判定完成后调用 → 显示表情+台词
    /// </summary>
    public void ShowJudgeResult(JudgeResult result)
    {
        if (currentGuest == null) return;

        // 1. 切换表情
        bool isHappy = result == JudgeResult.Perfect || result == JudgeResult.Good;
        SwitchGuestSprite(isHappy);

        // 🔥 核心修复：强制根节点 100% 不透明 + 显示
        ShowUIRootCompletely();

        // 显示对应台词
        if (guestDemandText != null)
        {
            switch (result)
            {
                case JudgeResult.Perfect:
                    guestDemandText.text = currentGuest.perfectText;
                    break;
                case JudgeResult.Good:
                    guestDemandText.text = currentGuest.goodText;
                    break;
                case JudgeResult.Fail:
                    guestDemandText.text = currentGuest.failText;
                    break;
            }
        }

        // 发放报酬
        int reward = 0;
        switch (result)
        {
            case JudgeResult.Perfect: reward = currentGuest.perfectMoney; break;
            case JudgeResult.Good: reward = currentGuest.goodMoney; break;
            case JudgeResult.Fail: reward = currentGuest.failMoney; break;
        }
        GameManager.Instance.AddMoney(reward);
        // 同时更新结算管理器的当天收入统计
        if (SettlementManager.Instance != null)
        {
            SettlementManager.Instance.AddDayEarnings(reward);
        }
        Debug.Log($"获得报酬：{reward} 文钱");

        // 完美评价+特殊NPC → 发放碎片
        if (result == JudgeResult.Perfect && currentGuest.isSpecialNPC && currentGuest.giveFragment)
        {
            GameManager.Instance.AddFragment();
            Debug.Log($"获得天宫录碎片 x{currentGuest.fragmentCount}");
        }

        // 延迟3秒 → 下一个客人
        StartCoroutine(WaitNextGuest(3f));
    }

    /// <summary>
    /// 🔥 强制显示UI根节点（透明度100%）
    /// </summary>
    private void ShowUIRootCompletely()
    {
        if (guestUIRoot != null)
            guestUIRoot.SetActive(true);

        // 强制根节点完全不透明，解决根节点透明问题
        if (m_UIRootCanvasGroup != null)
        {
            m_UIRootCanvasGroup.alpha = 1f;
            m_UIRootCanvasGroup.interactable = true;
            m_UIRootCanvasGroup.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// 延迟加载下一个客人
    /// </summary>
    private IEnumerator WaitNextGuest(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (guestDemandText != null)
            guestDemandText.text = "";

        NextGuest();
    }

    /// <summary>
    /// 切换到当前天的下一个客人。
    /// 如果当天客人已全部完成，则触发游戏进入下一天流程。
    /// </summary>
    public void NextGuest()
    {
        int currentDay = (int)GameManager.Instance.gameState.currentDay + 1; // 转换为1-based索引
        List<GuestDemandSO> currentDayGuests = null;

        switch (currentDay)
        {
            case 1: currentDayGuests = day1Guests; break;
            case 2: currentDayGuests = day2Guests; break;
            case 3: currentDayGuests = day3Guests; break;
            case 4: currentDayGuests = day4Guests; break;
            case 5: currentDayGuests = day5Guests; break;
        }

        if (currentDayGuests == null || currentDayGuests.Count == 0)
        {
            Debug.LogWarning($"Day{currentDay}客人列表为空！");
            HideGuestDisplay();
            return;
        }

        int currentIndex = currentDayGuests.IndexOf(currentGuest);
        if (currentIndex < currentDayGuests.Count - 1)
        {
            // 重置工作台状态
            if (PaperTube.Instance != null)
            {
                PaperTube.Instance.ClearStack();
                Debug.Log("切换客人时重置工作台状态");
            }

            currentGuest = currentDayGuests[currentIndex + 1];
            UpdateGuestDisplay();
        }
        else
        {
            Debug.Log($"Day{currentDay}客人已接待完毕，切换到下一天！");
            HideGuestDisplay();
            GameManager.Instance.NextDay();
        }
    }

    /// <summary>
    /// 更新当前客人的UI显示，包括头像、名称和需求描述。
    /// </summary>
    private void UpdateGuestDisplay()
    {
        if (currentGuest == null) { HideGuestDisplay(); return; }

        if (guestSpriteObject != null)
        {
            guestSpriteObject.SetActive(true);
            if (m_GuestSpriteRenderer != null)
            {
                m_GuestSpriteRenderer.sprite = currentGuest.normalSprite;
                m_GuestSpriteRenderer.enabled = currentGuest.normalSprite != null;
            }
        }

        // 刷新客人信息时，也强制显示根节点
        ShowUIRootCompletely();
        if (guestNameText != null) guestNameText.text = currentGuest.guestName;
        if (guestDemandText != null) guestDemandText.text = currentGuest.demandDesc;
    }

    /// <summary>
    /// 隐藏客人UI显示，通常在当前天客人结束后调用。
    /// </summary>
    private void HideGuestDisplay()
    {
        if (guestSpriteObject != null) { guestSpriteObject.SetActive(false); }
        if (guestUIRoot != null) { guestUIRoot.SetActive(false); }
    }

    /// <summary>
    /// 根据客人当前评价结果切换客人表情贴图。
    /// </summary>
    public void SwitchGuestSprite(bool isHappy)
    {
        if (currentGuest == null || m_GuestSpriteRenderer == null) return;

        if (isHappy && currentGuest.happySprite != null)
            m_GuestSpriteRenderer.sprite = currentGuest.happySprite;
        else if (!isHappy && currentGuest.angrySprite != null)
            m_GuestSpriteRenderer.sprite = currentGuest.angrySprite;
    }
}