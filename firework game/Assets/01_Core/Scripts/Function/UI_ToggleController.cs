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
    [Tooltip("要显示的UI面板列表")]
    public List<GameObject> panelsToShow = new List<GameObject>();

    [Tooltip("要隐藏的UI面板列表")]
    public List<GameObject> panelsToHide = new List<GameObject>();

    [Tooltip("要切换显隐的UI面板列表（点击时切换）")]
    public List<GameObject> panelsToToggle = new List<GameObject>();

    [Header("🔧 扩展配置（可选）")]
    [Tooltip("操作动画时长")]
    public float showDuration = 0.2f;

    [Tooltip("隐去动画时长，为了避免本身隐去引起bug")]
    public float hideDuration = 0.3f;

    // 缓存CanvasGroup（用于淡入淡出动画）
    private Dictionary<GameObject, CanvasGroup> _canvasGroups = new Dictionary<GameObject, CanvasGroup>();
    
    // 当前正在隐藏的协程，用于当隐藏按钮本身时禁用交互
    private Coroutine _currentHideCoroutine;
    private Button _button;

    void Awake()
    {
        // 获取Button组件
        _button = GetComponent<Button>();

        // 自动获取或添加所有目标面板的CanvasGroup
        var allPanels = new HashSet<GameObject>();
        allPanels.UnionWith(panelsToShow);
        allPanels.UnionWith(panelsToHide);

        foreach (var panel in allPanels)
        {
            if (panel != null)
            {
                CanvasGroup cg = panel.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = panel.AddComponent<CanvasGroup>();
                }
                _canvasGroups[panel] = cg;
            }
        }

        if (panelsToShow.Count == 0 && panelsToHide.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] 未配置任何UI面板！请在panelsToShow或panelsToHide中添加面板");
        }
    }

    /// <summary>
    /// 核心方法：控制UI显隐（绑定到Button的OnClick）
    /// </summary>
    public void ToggleUI()
    {
        // 隐藏面板
        foreach (var panel in panelsToHide)
        {
            if (panel != null && panel.activeSelf)
            {
                // 检查是否要隐藏按钮本身所在的面板
                bool isHidingSelfPanel = (panel == gameObject || panel.transform.IsChildOf(panel.transform));
                if (isHidingSelfPanel && _button != null)
                {
                    _button.interactable = false; // 先禁用交互，防止动画中断
                }
                HidePanel(panel);
            }
        }

        // 显示面板
        foreach (var panel in panelsToShow)
        {
            if (panel != null && !panel.activeSelf)
            {
                ShowPanel(panel);
            }
        }

        // 切换面板
        foreach (var panel in panelsToToggle)
        {
            if (panel != null)
            {
                if (panel.activeSelf)
                {
                    // 检查是否要隐藏按钮本身所在的面板
                    bool isHidingSelfPanel = (panel == gameObject || panel.transform.IsChildOf(panel.transform));
                    if (isHidingSelfPanel && _button != null)
                    {
                        _button.interactable = false; // 先禁用交互，防止动画中断
                    }
                    HidePanel(panel);
                }
                else
                {
                    ShowPanel(panel);
                }
            }
        }
    }

    /// <summary>
    /// 显示面板（支持动画）
    /// </summary>
    private void ShowPanel(GameObject panel)
    {
        if (!_canvasGroups.ContainsKey(panel))
        {
            Debug.LogWarning($"[{gameObject.name}] 面板 {panel.name} 未初始化CanvasGroup");
            panel.SetActive(true);
            return;
        }

        // 激活面板及所有子物体
        SetPanelActive(panel, true);
        
        if (showDuration > 0)
        {
            StartCoroutine(FadeIn(panel));
        }
    }

    /// <summary>
    /// 隐藏面板（支持动画）
    /// </summary>
    private void HidePanel(GameObject panel)
    {
        if (!_canvasGroups.ContainsKey(panel))
        {
            Debug.LogWarning($"[{gameObject.name}] 面板 {panel.name} 未初始化CanvasGroup");
            panel.SetActive(false);
            return;
        }

        if (hideDuration > 0)
        {
            _currentHideCoroutine = StartCoroutine(FadeOut(panel));
        }
        else
        {
            SetPanelActive(panel, false);
        }
    }

    /// <summary>
    /// 递归激活/禁用面板及其所有子物体，并恢复/隐藏CanvasGroup
    /// </summary>
    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel == null) return;

        // 激活/禁用主面板
        if (panel.activeSelf != active)
        {
            panel.SetActive(active);
        }

        // 递归激活/禁用所有子物体
        foreach (Transform child in panel.GetComponentsInChildren<Transform>(true))
        {
            if (child.gameObject != panel)
            {
                if (child.gameObject.activeSelf != active)
                {
                    child.gameObject.SetActive(active);
                }
            }
        }

        // 恢复/隐藏所有子物体的CanvasGroup alpha
        CanvasGroup[] childCanvasGroups = panel.GetComponentsInChildren<CanvasGroup>(true);
        foreach (var cg in childCanvasGroups)
        {
            if (cg != null)
            {
                cg.alpha = active ? 1 : 0;
            }
        }
    }

    // 🌟 淡入动画：从透明度0到1，包括所有子物体
    IEnumerator FadeIn(GameObject panel)
    {
        CanvasGroup cg = _canvasGroups[panel];
        CanvasGroup[] childCanvasGroups = panel.GetComponentsInChildren<CanvasGroup>(true);
        
        // 初始化为0
        cg.alpha = 0;
        foreach (var childCg in childCanvasGroups)
        {
            if (childCg != null && childCg != cg)
            {
                childCg.alpha = 0;
            }
        }

        float t = 0;
        while (t < showDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / showDuration);
            cg.alpha = alpha;
            foreach (var childCg in childCanvasGroups)
            {
                if (childCg != null && childCg != cg)
                {
                    childCg.alpha = alpha;
                }
            }
            yield return null;
        }
        
        cg.alpha = 1;
        foreach (var childCg in childCanvasGroups)
        {
            if (childCg != null && childCg != cg)
            {
                childCg.alpha = 1;
            }
        }
    }

    // 🌟 淡出动画：从透明度1到0，完成后才SetActive(false)，包括所有子物体
    IEnumerator FadeOut(GameObject panel)
    {
        CanvasGroup cg = _canvasGroups[panel];
        CanvasGroup[] childCanvasGroups = panel.GetComponentsInChildren<CanvasGroup>(true);
        
        cg.alpha = 1;
        foreach (var childCg in childCanvasGroups)
        {
            if (childCg != null && childCg != cg)
            {
                childCg.alpha = 1;
            }
        }

        float t = 0;
        while (t < hideDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / hideDuration);
            cg.alpha = alpha;
            foreach (var childCg in childCanvasGroups)
            {
                if (childCg != null && childCg != cg)
                {
                    childCg.alpha = alpha;
                }
            }
            yield return null;
        }
        
        cg.alpha = 0;
        foreach (var childCg in childCanvasGroups)
        {
            if (childCg != null && childCg != cg)
            {
                childCg.alpha = 0;
            }
        }
        
        SetPanelActive(panel, false);
        
        // 如果隐藏完成后，恢复按钮交互（如果之前被禁用）
        if (_button != null && !panel.activeSelf)
        {
            _button.interactable = true;
        }
    }

    /// <summary>
    /// 切换单个面板的显隐状态
    /// </summary>
    public void TogglePanel(GameObject panel)
    {
        if (panel == null) return;

        if (panel.activeSelf)
        {
            // 检查是否要隐藏按钮本身所在的面板
            bool isHidingSelfPanel = (panel == gameObject || panel.transform.IsChildOf(panel.transform));
            if (isHidingSelfPanel && _button != null)
            {
                _button.interactable = false;
            }
            HidePanel(panel);
        }
        else
        {
            ShowPanel(panel);
        }
    }
}