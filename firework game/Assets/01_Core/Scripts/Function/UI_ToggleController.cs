using UnityEngine;
using UnityEngine.UI;
using System.Collections; // 必须！协程的IEnumerator在这个命名空间里
using System.Collections.Generic; // 用于otherPanelsToHide的List

/// <summary>
/// 通用UI显隐控制器（可复用，绑定到任意Button）
/// </summary>
public class UI_ToggleController : MonoBehaviour
{
    [Header("📌 核心配置")]
    [Tooltip("要控制显隐的UI面板（拖入面板根节点）")]
    public GameObject targetUIPanel;

    [Tooltip("控制类型：0=切换显隐 1=仅显示 2=仅隐藏")]
    public int controlType = 0; // 默认切换

    [Header("🔧 扩展配置（可选）")]
    [Tooltip("是否隐藏其他面板（比如打开A时关闭B/C）")]
    public bool hideOtherPanels = false;
    [Tooltip("需要互斥的其他面板列表（仅hideOtherPanels=true时生效）")]
    public List<GameObject> otherPanelsToHide;

    [Tooltip("显隐动画时长（0=无动画）")]
    public float animateDuration = 0.2f;

    // 缓存目标面板的CanvasGroup（用于淡入淡出动画）
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        // 自动获取CanvasGroup（没有则添加），用于动画
        if (targetUIPanel != null)
        {
            _canvasGroup = targetUIPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null && animateDuration > 0)
            {
                _canvasGroup = targetUIPanel.AddComponent<CanvasGroup>();
            }
        }

        // 空值保护
        if (targetUIPanel == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 未绑定目标UI面板！");
        }
    }

    /// <summary>
    /// 核心方法：控制UI显隐（绑定到Button的OnClick）
    /// </summary>
    public void ToggleUI()
    {
        if (targetUIPanel == null) return;

        // 扩展：互斥隐藏其他面板
        if (hideOtherPanels && otherPanelsToHide != null)
        {
            foreach (var panel in otherPanelsToHide)
            {
                if (panel != null && panel.activeSelf)
                {
                    HidePanel(panel);
                }
            }
        }

        // 核心：根据控制类型执行操作
        switch (controlType)
        {
            case 0: // 切换显隐
                if (targetUIPanel.activeSelf)
                {
                    HidePanel(targetUIPanel);
                }
                else
                {
                    ShowPanel(targetUIPanel);
                }
                break;
            case 1: // 仅显示
                ShowPanel(targetUIPanel);
                break;
            case 2: // 仅隐藏
                HidePanel(targetUIPanel);
                break;
        }
    }

    /// <summary>
    /// 显示面板（支持动画）
    /// </summary>
    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        if (_canvasGroup != null && animateDuration > 0)
        {
            _canvasGroup.alpha = 0;
            // 无插件：启用协程淡入
            StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// 隐藏面板（支持动画）
    /// </summary>
    private void HidePanel(GameObject panel)
    {
        if (_canvasGroup != null && animateDuration > 0)
        {
            // 无插件：启用协程淡出
            StartCoroutine(FadeOut(panel));
        }
        else
        {
            panel.SetActive(false);
        }
    }

    // 🌟 修复核心：非泛型IEnumerator（Unity协程专用）
    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < animateDuration)
        {
            t += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0, 1, t / animateDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1;
    }

    // 🌟 修复核心：非泛型IEnumerator
    IEnumerator FadeOut(GameObject panel)
    {
        float t = 0;
        while (t < animateDuration)
        {
            t += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1, 0, t / animateDuration);
            yield return null;
        }
        _canvasGroup.alpha = 0;
        panel.SetActive(false);
    }
}