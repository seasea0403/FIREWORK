using UnityEngine;

public class ContentItem : MonoBehaviour
{
    [Header("=== 烟花组件类型 ===")]
    public FireworkComponent fireworkComponent;

    [Header("=== 描边设置 ===")]
    [Tooltip("描边宽度（对应材质里的Width）")]
    public int outlineWidth = 3;
    [Tooltip("描边颜色（金色推荐：255,215,0）")]
    public Color outlineColor = new Color(1, 0.84f, 0); // 金色

    [Header("=== 勺子设置 ===")]
    [Tooltip("是否显示勺子（引线设为false）")]
    public bool showSpoon = true;
    [Tooltip("拖入你的勺子预制体")]
    public GameObject spoonPrefab;
    [Tooltip("勺子在物品上方的偏移量")]
    public Vector3 spoonOffset = new Vector3(0, 0.5f, 0);

    // 私有变量（加m_前缀）
    private Material m_InstanceMat; // 独立材质实例
    private GameObject m_SpoonInstance; // 勺子实例
    private bool m_IsUnlocked; // 解锁状态
    public bool isSelected = false; // 是否选中（保留原有命名，避免绑定问题）
    public bool IsUnlocked => m_IsUnlocked;

    void Start()
    {
        // 检查解锁状态
        m_IsUnlocked = FireworkComponentMapper.IsItemUnlocked(this);
        if (!m_IsUnlocked)
        {
            // 灰显+禁用点击
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            return;
        }

        // 初始化材质
        InitMaterialInstance();
        // 初始隐藏描边
        SetOutlineActive(false);
        // 初始化勺子
        if (showSpoon)
        {
            InitSpoon();
        }
    }

    /// <summary>
    /// 鼠标点击触发
    /// </summary>
    void OnMouseDown()
    {
        if (!m_IsUnlocked) return;

        Debug.Log("点击物品：" + gameObject.name, this);
        if (ContentSelector.Instance != null)
        {
            ContentSelector.Instance.OnItemClicked(this);
        }
    }

    #region 核心方法（保留原有逻辑）
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
            Debug.LogWarning("无SpriteRenderer或材质！", this);
        }
    }

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
            Debug.LogWarning("未设置勺子预制体！", this);
        }
    }

    public void SetOutlineActive(bool active)
    {
        if (m_InstanceMat == null) return;

        m_InstanceMat.SetInt("_OutlineEnabled", active ? 1 : 0);
        m_InstanceMat.SetFloat("_OutlineWidth", outlineWidth);
        m_InstanceMat.SetColor("_OutlineColorBase", outlineColor);
    }

    public void Select()
    {
        isSelected = true;
        SetOutlineActive(true);
        if (showSpoon && m_SpoonInstance != null)
        {
            m_SpoonInstance.SetActive(true);
        }
    }

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