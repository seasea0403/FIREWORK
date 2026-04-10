using UnityEngine;

/// <summary>
/// 道具项组件：表示玩家可选材料，并管理选中、描边和勺子显示逻辑。
/// </summary>
public class ContentItem : MonoBehaviour
{
    [Header("=== 爆竹组件 ===")]
    public FireworkComponent fireworkComponent;

    [Header("=== 描边设置 ===")]
    [Tooltip("描边宽度，对应材质的 OutlineWidth")]
    public int outlineWidth = 3;
    [Tooltip("描边颜色，例如金黄色")]
    public Color outlineColor = new Color(1, 0.84f, 0); // 颜色

    [Header("=== 勺子显示 ===")]
    [Tooltip("是否显示勺子")]
    public bool showSpoon = true;
    [Tooltip("勺子预制体")]
    public GameObject spoonPrefab;
    [Tooltip("勺子相对偏移位置")]
    public Vector3 spoonOffset = new Vector3(0, 0.5f, 0);

    // 私有成员变量，m_前缀表示实例字段
    private Material m_InstanceMat; // 材质实例
    private GameObject m_SpoonInstance; // 勺子实例
    private bool m_IsUnlocked; // 解锁状态
    public bool isSelected = false; // 是否选中，用于视觉表现
    public bool IsUnlocked => m_IsUnlocked;

    /// <summary>
    /// 初始化道具项状态，设置材质、描边和勺子提示。
    /// </summary>
    void Start()
    {
        // 初始化解锁状态
        m_IsUnlocked = FireworkComponentMapper.IsItemUnlocked(this);
        if (!m_IsUnlocked)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            return;
        }

        // 初始化材质实例
        InitMaterialInstance();
        // 关闭描边
        SetOutlineActive(false);
        // 初始化勺子
        if (showSpoon)
        {
            InitSpoon();
        }
    }

    /// <summary>
    /// 响应鼠标点击：如果材料可用则通知 ContentSelector 更新选中状态。
    /// </summary>
    void OnMouseDown()
    {
        if (!m_IsUnlocked) return;

        Debug.Log("点击道具：" + gameObject.name, this);
        if (ContentSelector.Instance != null)
        {
            ContentSelector.Instance.OnItemClicked(this);
        }
    }

    #region 内部渲染与选择逻辑
    /// <summary>
    /// 创建 SpriteRenderer 材质实例，避免多个道具共用材质导致描边显示异常。
    /// </summary>
    private void InitMaterialInstance()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.material != null)
        {
            m_InstanceMat = new Material(sr.material);
            sr.material = m_InstanceMat;
        }
        else
        {
            Debug.LogWarning("未找到 SpriteRenderer 或材质为空", this);
        }
    }

    /// <summary>
    /// 初始化勺子预制体，并将其挂载在道具项下用于选中提示。
    /// </summary>
    private void InitSpoon()
    {
        if (spoonPrefab != null)
        {
            m_SpoonInstance = Instantiate(spoonPrefab, transform);
            m_SpoonInstance.transform.localPosition = spoonOffset;
            m_SpoonInstance.SetActive(false);
        }
        else if (showSpoon)
        {
            Debug.LogWarning("未设置勺子预制体", this);
        }
    }

    /// <summary>
    /// 控制描边高亮状态，用于选中和取消选中视觉反馈。
    /// </summary>
    public void SetOutlineActive(bool active)
    {
        if (m_InstanceMat == null) return;

        m_InstanceMat.SetInt("_OutlineEnabled", active ? 1 : 0);
        m_InstanceMat.SetFloat("_OutlineWidth", outlineWidth);
        m_InstanceMat.SetColor("_OutlineColorBase", outlineColor);
    }

    /// <summary>
    /// 标记当前道具为已选中，并显示描边/勺子提示。
    /// </summary>
    public void Select()
    {
        isSelected = true;
        SetOutlineActive(true);
        if (showSpoon && m_SpoonInstance != null)
        {
            m_SpoonInstance.SetActive(true);
        }
    }

    /// <summary>
    /// 取消当前道具选中状态，并隐藏描边/勺子提示。
    /// </summary>
    public void Deselect()
    {
        isSelected = false;
        SetOutlineActive(false);
        if (showSpoon && m_SpoonInstance != null)
        {
            m_SpoonInstance.SetActive(false);
        }
    }
    #endregion
}