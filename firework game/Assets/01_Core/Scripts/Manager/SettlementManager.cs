using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 每日结算管理器：负责显示当天结算页面、展示收入/好评/碎片统计，并处理继续按钮进入下一天。
/// 定义了核心函数：ShowSettlement、HideSettlement、AddDayEarnings、AddDayGoodReview、ResetDayStats、UpdateSettlementUI、OnContinueButtonClicked。
/// </summary>
public class SettlementManager : Singleton<SettlementManager>
{
    [Header("UI组件绑定")]
    public GameObject settlementPanel;
    public TMP_Text dayText;
    public TMP_Text totalMoneyText;
    public TMP_Text dayEarningsText;
    public TMP_Text totalGoodReviewsText;
    public TMP_Text dayGoodReviewsText;
    public TMP_Text fragmentCountText;
    public Button continueButton;

    // 当天统计数据
    private int dayEarnings = 0;
    private int dayGoodReviews = 0;

    /// <summary>
    /// 初始化结算界面，隐藏结算面板并绑定继续按钮事件。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (settlementPanel != null)
            settlementPanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButtonClicked);
    }

    /// <summary>
    /// 显示每日结算页面
    /// </summary>
    public void ShowSettlement()
    {
        if (settlementPanel == null) return;

        // 更新UI显示
        UpdateSettlementUI();

        // 显示结算面板
        settlementPanel.SetActive(true);

        Debug.Log("显示每日结算页面");
    }

    /// <summary>
    /// 隐藏结算页面
    /// </summary>
    public void HideSettlement()
    {
        if (settlementPanel != null)
            settlementPanel.SetActive(false);
    }

    /// <summary>
    /// 添加当天收入
    /// </summary>
    public void AddDayEarnings(int amount)
    {
        dayEarnings += amount;
    }

    /// <summary>
    /// 添加当天好评
    /// </summary>
    public void AddDayGoodReview()
    {
        dayGoodReviews++;
    }

    /// <summary>
    /// 重置当天统计数据
    /// </summary>
    public void ResetDayStats()
    {
        dayEarnings = 0;
        dayGoodReviews = 0;
    }

    /// <summary>
    /// 更新结算UI
    /// </summary>
    private void UpdateSettlementUI()
    {
        if (GameManager.Instance?.gameState == null) return;

        var gameState = GameManager.Instance.gameState;

        // 显示天数
        if (dayText != null)
            dayText.text = $"第{(int)gameState.currentDay}天结算";

        // 显示总金钱
        if (totalMoneyText != null)
            totalMoneyText.text = $"总金钱: {gameState.money} 文钱";

        // 显示当天收入
        if (dayEarningsText != null)
            dayEarningsText.text = $"当天收入: {dayEarnings} 文钱";

        // 显示总好评数
        if (totalGoodReviewsText != null)
            totalGoodReviewsText.text = $"总好评数: {gameState.totalGoodReviews}";

        // 显示当天好评数
        if (dayGoodReviewsText != null)
            dayGoodReviewsText.text = $"当天好评: {dayGoodReviews}";

        // 显示碎片数量
        if (fragmentCountText != null)
            fragmentCountText.text = $"天宫录碎片: {gameState.fragmentCount}/3";
    }

    /// <summary>
    /// 继续按钮点击事件
    /// </summary>
    private void OnContinueButtonClicked()
    {
        HideSettlement();

        // 通知GameManager继续到下一天
        GameManager.Instance.ContinueToNextDay();
    }
}