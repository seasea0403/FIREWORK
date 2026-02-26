using UnityEngine;

public class ContentItem : MonoBehaviour
{
    [Header("=== 描边设置 ===")]
    [Tooltip("描边宽度（对应材质里的Width）")]
    public int outlineWidth = 3;
    [Tooltip("描边颜色（金色推荐：255,215,0）")]
    public Color outlineColor = new Color(1, 0.84f, 0); // 金色

    [Header("=== 勺子设置 ===")]
    [Tooltip("拖入你的勺子预制体")]
    public GameObject spoonPrefab;
    [Tooltip("勺子在物品上方的偏移量，可实时调整")]
    public Vector3 spoonOffset = new Vector3(0, 0.5f, 0);

    // 私有变量
    private Material _instanceMat; // 每个物品独立的材质实例
    private GameObject _spoonInstance; // 生成的勺子实例
    public bool isSelected = false; // 是否被选中

    void Start()
    {
        // 1. 初始化材质实例（避免多个物品共享一个材质状态）
        InitMaterialInstance();

        // 2. 初始隐藏描边
        SetOutlineActive(false);

        // 3. 初始化勺子（生成+隐藏）
        InitSpoon();
    }

    // 鼠标点击物品时触发
    void OnMouseDown()
    {
        Debug.Log("我被点击了！物体名：" + gameObject.name, this);
        // 告诉管理器：我被点击了
        if (ContentSelector.Instance != null)
        {
            ContentSelector.Instance.OnItemClicked(this);
        }
    }

    #region 核心方法
    // 初始化材质实例
    private void InitMaterialInstance()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.material != null)
        {
            // 创建材质副本（每个物品独立）
            _instanceMat = new Material(sr.material);
            // 替换物品的材质为副本
            sr.material = _instanceMat;
        }
        else
        {
            Debug.LogWarning("当前物体没有SpriteRenderer或材质！", this);
        }
    }

    // 初始化勺子
    private void InitSpoon()
    {
        if (spoonPrefab != null)
        {
            // 生成勺子，父物体设为当前物品（跟随物品移动）
            _spoonInstance = Instantiate(spoonPrefab, transform);
            // 设置勺子位置偏移
            _spoonInstance.transform.localPosition = spoonOffset;
            // 初始隐藏勺子
            _spoonInstance.SetActive(false);
        }
        else
        {
            Debug.LogWarning("未设置勺子预制体！", this);
        }
    }

    // 控制描边显示/隐藏
    public void SetOutlineActive(bool active)
    {
        if (_instanceMat == null) return;

        // 开关描边（_OutlineEnabled是Sprites/Outline材质的核心参数）
        _instanceMat.SetInt("_OutlineEnabled", active ? 1 : 0);
        // 同步描边宽度和颜色
        _instanceMat.SetFloat("_OutlineWidth", outlineWidth);
        _instanceMat.SetColor("_OutlineColorBase", outlineColor);
    }

    // 选中物品（外部调用）
    public void Select()
    {
        isSelected = true;
        SetOutlineActive(true); // 显示描边
        if (_spoonInstance != null) _spoonInstance.SetActive(true); // 显示勺子
    }

    // 取消选中（外部调用）
    public void Deselect()
    {
        isSelected = false;
        SetOutlineActive(false); // 隐藏描边
        if (_spoonInstance != null) _spoonInstance.SetActive(false); // 隐藏勺子
    }
    #endregion
}