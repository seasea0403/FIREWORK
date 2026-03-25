using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GuestManager : Singleton<GuestManager>
{
    [Header("🎎 客人显示配置（Inspector绑定）")]
    public GameObject guestSpriteObject;
    public TMP_Text guestNameText;
    public TMP_Text guestDemandText;
    public GameObject guestUIRoot;
    // 🔥 新增：评价台词UI（在Inspector拖入一个TextMeshPro）
    [Header("✅ 新增评价台词UI")]
    public TMP_Text guestResultText;

    private SpriteRenderer m_GuestSpriteRenderer;
    public GuestDemandSO currentGuest;
    public List<GuestDemandSO> day1Guests;

    protected override void Awake()
    {
        base.Awake();
        if (guestSpriteObject != null)
        {
            m_GuestSpriteRenderer = guestSpriteObject.GetComponent<SpriteRenderer>();
            if (m_GuestSpriteRenderer == null)
                m_GuestSpriteRenderer = guestSpriteObject.AddComponent<SpriteRenderer>();
        }

        if (guestUIRoot != null) guestUIRoot.SetActive(false);
        if (guestSpriteObject != null) guestSpriteObject.SetActive(false);
        // 初始化隐藏台词
        if (guestResultText != null) guestResultText.text = "";

        LoadDayGuests(1);
        Debug.Log("GuestManager初始化完成！");
    }

    public void LoadDayGuests(int day)
    {
        if (day == 1 && day1Guests != null && day1Guests.Count > 0)
        {
            currentGuest = day1Guests[0];
            UpdateGuestDisplay();
        }
        else
        {
            Debug.LogWarning($"暂无Day{day}的客人数据！");
            HideGuestDisplay();
        }
    }

    /// <summary>
    /// 🔥 核心：判定完成后调用 → 显示表情+台词
    /// </summary>
    public void ShowJudgeResult(JudgeResult result)
    {
        if (currentGuest == null) return;

        // 1. 切换表情
        bool isHappy = result == JudgeResult.Perfect || result == JudgeResult.Good;
        SwitchGuestSprite(isHappy);

        // 2. 显示对应台词
        if (guestResultText != null)
        {
            switch (result)
            {
                case JudgeResult.Perfect:
                    guestResultText.text = currentGuest.perfectText;
                    break;
                case JudgeResult.Good:
                    guestResultText.text = currentGuest.goodText;
                    break;
                case JudgeResult.Fail:
                    guestResultText.text = currentGuest.failText;
                    break;
            }
        }

        // 3. 发放报酬
        int reward = 0;
        switch (result)
        {
            case JudgeResult.Perfect: reward = currentGuest.perfectMoney; break;
            case JudgeResult.Good: reward = currentGuest.goodMoney; break;
            case JudgeResult.Fail: reward = currentGuest.failMoney; break;
        }
        GameManager.Instance.AddMoney(reward);
        Debug.Log($"获得报酬：{reward} 文钱");

        // 4. 完美评价+特殊NPC → 发放碎片
        if (result == JudgeResult.Perfect && currentGuest.isSpecialNPC && currentGuest.giveFragment)
        {
            GameManager.Instance.AddFragment();
            Debug.Log($"获得天宫录碎片 x{currentGuest.fragmentCount}");
        }

        // 5. 延迟2秒 → 下一个客人
        StartCoroutine(WaitNextGuest(2f));
    }

    /// <summary>
    /// 延迟加载下一个客人
    /// </summary>
    private IEnumerator WaitNextGuest(float delay)
    {
        yield return new WaitForSeconds(delay);
        guestResultText.text = ""; // 清空台词
        NextGuest();
    }

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
            UpdateGuestDisplay();
        }
        else
        {
            Debug.Log("Day1客人已接待完毕，切换到下一天！");
            HideGuestDisplay();
            GameManager.Instance.NextDay();
        }
    }

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

        if (guestUIRoot != null) guestUIRoot.SetActive(true);
        if (guestNameText != null) guestNameText.text = currentGuest.guestName;
        if (guestDemandText != null) guestDemandText.text = currentGuest.demandDesc;
        if (guestResultText != null) guestResultText.text = "";
    }

    private void HideGuestDisplay()
    {
        if (guestSpriteObject != null) { guestSpriteObject.SetActive(false); }
        if (guestUIRoot != null) guestUIRoot.SetActive(false);
        if (guestResultText != null) guestResultText.text = "";
    }

    public void SwitchGuestSprite(bool isHappy)
    {
        if (currentGuest == null || m_GuestSpriteRenderer == null) return;

        if (isHappy && currentGuest.happySprite != null)
            m_GuestSpriteRenderer.sprite = currentGuest.happySprite;
        else if (!isHappy && currentGuest.angrySprite != null)
            m_GuestSpriteRenderer.sprite = currentGuest.angrySprite;
    }
}