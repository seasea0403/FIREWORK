using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 烟花纸筒核心逻辑：负责材料堆叠、组件记录、类别映射（供判定用）
/// </summary>
public class PaperTube : MonoBehaviour
{
    [Header("=== 火药类预制体 ===")]
    public GameObject powderIn1Prefab;   // 火药第一层预制体
    public GameObject powderIn2Prefab;   // 火药第二层预制体
    public GameObject powderIn3Prefab;   // 火药第三层预制体

    [Header("=== 彩珠类预制体 ===")]
    public GameObject beadInRedPrefab;    // 红彩珠预制体
    public GameObject beadInBluePrefab;   // 蓝彩珠预制体
    public GameObject beadInGreenPrefab;  // 绿彩珠预制体
    public GameObject beadInPurplePrefab; // 紫彩珠预制体
    public GameObject beadInGoldPrefab;   // 金彩珠预制体
    public GameObject beadInSilverPrefab; // 银彩珠预制体

    [Header("=== 引线类预制体 ===")]
    public GameObject fusePrefab;         // 引线通用预制体（短/长引线共用）

    [Header("=== 外壳类预制体 ===")]
    public GameObject shellCylinderPrefab; // 圆柱外壳预制体
    public GameObject shellPyramidPrefab;  // 圆锥外壳预制体
    public GameObject shellRoundPrefab;    // 圆形外壳预制体
    public GameObject shellHeartPrefab;    // 心形外壳预制体（Day4解锁）
    public GameObject shellEllipsePrefab;  // 椭圆外壳预制体（Day4解锁）

    [Header("=== 彩纸类预制体 ===")]
    public GameObject paperRedPrefab;      // 红色彩纸预制体
    public GameObject paperYellowPrefab;   // 黄色彩纸预制体
    public GameObject paperGreenPrefab;    // 绿色彩纸预制体

    [Header("=== 粘土类预制体 ===")]
    public GameObject clayPrefab1;      // 粘土预制体
    public GameObject clayPrefab2;

    [Header("=== 隔板类预制体 ===")]
    public GameObject partitionPrefab;     // 隔板预制体（Day3解锁）

    [Header("=== 堆叠设置 ===")]
    public Transform stackRoot;            // 堆叠根节点（无则自动生成）
    public int orderIncrement = 1;         // 层级递增步长

    // 私有变量（加m_前缀，符合命名规范）
    private List<GameObject> m_StackedItems = new List<GameObject>();       // 已堆叠的物品实例
    private GameObject m_CurrentShellObj;                                  // 当前外壳实例
    private SpriteRenderer m_TubeSpriteRenderer;
    private List<FireworkComponent> m_StackedFireworkComponents = new List<FireworkComponent>(); // 已堆叠的组件列表
    private Dictionary<ComponentCategory, FireworkComponent> m_CategoryComponentMap = new Dictionary<ComponentCategory, FireworkComponent>(); // 类别-组件映射（判定核心）

    // 公共属性（供外部判定调用）
    public List<FireworkComponent> StackedFireworkComponents => new List<FireworkComponent>(m_StackedFireworkComponents);
    public Dictionary<ComponentCategory, FireworkComponent> CategoryComponentMap => new Dictionary<ComponentCategory, FireworkComponent>(m_CategoryComponentMap);

    void Start()
    {
        // 获取纸筒自身的SpriteRenderer
        m_TubeSpriteRenderer = GetComponent<SpriteRenderer>();

        // 初始化堆叠根节点（防止空引用）
        if (stackRoot == null)
        {
            stackRoot = new GameObject("StackRoot").transform;
            stackRoot.parent = transform;
            stackRoot.localPosition = Vector3.zero;
        }

        // 初始化类别映射字典
        if (m_CategoryComponentMap == null)
        {
            m_CategoryComponentMap = new Dictionary<ComponentCategory, FireworkComponent>();
        }
    }

    /// <summary>
    /// 点击纸筒添加选中的材料（核心交互逻辑）
    /// </summary>
    void OnMouseDown()
    {
        // 获取当前选中的材料
        ContentItem selectedItem = ContentSelector.Instance.CurrentSelectedItem;
        if (selectedItem == null)
        {
            Debug.LogWarning("请先选中材料再点击纸筒！");
            return;
        }

        // 1. 获取组件类型+所属类别（核心：自动推导类别，无需手动绑定）
        FireworkComponent currentComp = FireworkComponentMapper.MapToFireworkComponent(selectedItem);
        ComponentCategory currentCategory = ComponentCategoryHelper.GetCategory(currentComp);

        // 2. 记录组件（去重：同一组件只存一次）
        if (!m_StackedFireworkComponents.Contains(currentComp))
        {
            m_StackedFireworkComponents.Add(currentComp);
        }

        // 3. 记录类别-组件映射（同一类别只保留最后选中的组件）
        if (currentCategory != ComponentCategory.None)
        {
            m_CategoryComponentMap[currentCategory] = currentComp;
        }

        // 4. 根据组件类型生成堆叠视觉效果
        SpawnComponentVisual(currentComp);

        // 5. 取消选中状态（交互闭环）
        selectedItem.Deselect();
        ContentSelector.Instance.ClearAllSelection();
    }

    /// <summary>
    /// 根据组件类型生成堆叠视觉效果（核心视觉逻辑）
    /// </summary>
    private void SpawnComponentVisual(FireworkComponent compType)
    {
        switch (compType)
        {
            // 🔥 火药类
            case FireworkComponent.NormalPowder:
            case FireworkComponent.PushPowder:
            case FireworkComponent.SmokePowder:
                SpawnPowderInStack();
                break;

            // 🌈 彩珠类
            case FireworkComponent.RedBead:
            case FireworkComponent.BlueBead:
            case FireworkComponent.GreenBead:
            case FireworkComponent.PurpleBead:
            case FireworkComponent.GoldBead:
            case FireworkComponent.SilverBead:
                SpawnBeadInStack(compType);
                break;

            // 🧵 引线类
            case FireworkComponent.ShortFuse:
            case FireworkComponent.LongFuse:
                SpawnFuseInStack();
                break;

            // 📦 外壳类
            case FireworkComponent.CylinderShell:
            case FireworkComponent.PyramidShell:
            case FireworkComponent.RoundShell:
            case FireworkComponent.HeartShell:
            case FireworkComponent.EllipseShell:
                SpawnShellStack(compType);
                break;

            // 🎨 彩纸类
            case FireworkComponent.RedPaper:
            case FireworkComponent.YellowPaper:
            case FireworkComponent.GreenPaper:
                SpawnPaperInStack(compType);
                break;

            // 🧩 隔板类
            case FireworkComponent.Partition:
                SpawnPartitionInStack();
                break;

            // 🧩 粘土类
            case FireworkComponent.Clay:
                SpawnClayInStack();
                break;

            // ⚠️ 未配置的组件
            default:
                Debug.LogWarning($"未配置组件{compType}的堆叠视觉效果！");
                break;
        }
    }

    #region 各组件堆叠生成方法（保留原有动画逻辑）
    /// <summary>
    /// 生成火药堆叠视觉效果
    /// </summary>
    private void SpawnPowderInStack()
    {
        int currentMaxOrder = GetCurrentMaxOrder();

        // 生成火药三层堆叠（模拟分层效果）
        if (powderIn1Prefab != null)
        {
            GameObject powder1 = Instantiate(powderIn1Prefab, stackRoot);
            powder1.transform.localPosition = Vector3.zero;
            if (powder1.TryGetComponent<SpriteRenderer>(out var sr1))
            {
                sr1.sortingOrder = currentMaxOrder + 1;
            }
            m_StackedItems.Add(powder1);
        }

        if (powderIn2Prefab != null)
        {
            GameObject powder2 = Instantiate(powderIn2Prefab, stackRoot);
            powder2.transform.localPosition = Vector3.zero;
            if (powder2.TryGetComponent<SpriteRenderer>(out var sr2))
            {
                sr2.sortingOrder = currentMaxOrder + 2;
            }
            m_StackedItems.Add(powder2);
        }

        if (powderIn3Prefab != null)
        {
            GameObject powder3 = Instantiate(powderIn3Prefab, stackRoot);
            powder3.transform.localPosition = Vector3.zero;
            if (powder3.TryGetComponent<SpriteRenderer>(out var sr3))
            {
                sr3.sortingOrder = currentMaxOrder + 3;
            }
            m_StackedItems.Add(powder3);
        }

        Debug.Log($"生成火药堆叠，当前堆叠数量：{m_StackedItems.Count}");
    }

    /// <summary>
    /// 生成彩珠堆叠视觉效果
    /// </summary>
    private void SpawnBeadInStack(FireworkComponent beadType)
    {
        GameObject targetPrefab = null;

        // 匹配对应颜色彩珠预制体
        switch (beadType)
        {
            case FireworkComponent.RedBead: targetPrefab = beadInRedPrefab; break;
            case FireworkComponent.BlueBead: targetPrefab = beadInBluePrefab; break;
            case FireworkComponent.GreenBead: targetPrefab = beadInGreenPrefab; break;
            case FireworkComponent.PurpleBead: targetPrefab = beadInPurplePrefab; break;
            case FireworkComponent.GoldBead: targetPrefab = beadInGoldPrefab; break;
            case FireworkComponent.SilverBead: targetPrefab = beadInSilverPrefab; break;
            default:
                Debug.LogError($"无匹配的彩珠预制体：{beadType}");
                return;
        }

        // 生成彩珠实例
        if (targetPrefab != null)
        {
            int currentMaxOrder = GetCurrentMaxOrder();
            GameObject bead = Instantiate(targetPrefab, stackRoot);
            bead.transform.localPosition = Vector3.zero;
            if (bead.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.sortingOrder = currentMaxOrder + orderIncrement;
            }
            m_StackedItems.Add(bead);
            Debug.Log($"生成{beadType}彩珠堆叠，当前数量：{m_StackedItems.Count}");
        }
        else
        {
            Debug.LogError($"未绑定{beadType}彩珠预制体！");
        }
    }

    /// <summary>
    /// 生成引线堆叠视觉效果
    /// </summary>
    private void SpawnClayInStack()
    {
        if (clayPrefab1 == null)
        {
            Debug.LogError("未绑定粘土预制体！");
            return;
        }

        int currentMaxOrder = GetCurrentMaxOrder();
        GameObject clay1 = Instantiate(clayPrefab1, stackRoot);
        clay1.transform.localPosition = Vector3.zero;
        if (clay1.TryGetComponent<SpriteRenderer>(out var sr1))
        {
            sr1.sortingOrder = currentMaxOrder + orderIncrement;
        }
        m_StackedItems.Add(clay1);
        GameObject clay2 = Instantiate(clayPrefab2, stackRoot);
        clay2.transform.localPosition = Vector3.zero;
        if (clay2.TryGetComponent<SpriteRenderer>(out var sr2))
        {
            sr2.sortingOrder = currentMaxOrder + orderIncrement;
        }
        m_StackedItems.Add(clay2);
        Debug.Log($"生成粘土堆叠，当前数量：{m_StackedItems.Count}");
    }


    private void SpawnFuseInStack()
    {
        if (fusePrefab == null)
        {
            Debug.LogError("未绑定引线预制体！");
            return;
        }

        int currentMaxOrder = GetCurrentMaxOrder();
        GameObject fuse = Instantiate(fusePrefab, stackRoot);
        fuse.transform.localPosition = Vector3.zero;
        if (fuse.TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.sortingOrder = currentMaxOrder + orderIncrement;
        }
        m_StackedItems.Add(fuse);
        Debug.Log($"生成引线堆叠，当前数量：{m_StackedItems.Count}");
    }


    /// <summary>
    /// 生成外壳堆叠视觉效果（外壳特殊：覆盖整个纸筒，隐藏所有内部零件）
    /// </summary>
    private void SpawnShellStack(FireworkComponent shellType)
    {
        // 1. 销毁旧外壳（切换外壳用）
        if (m_CurrentShellObj != null)
        {
            Destroy(m_CurrentShellObj);
            m_StackedItems.Remove(m_CurrentShellObj);
            m_CurrentShellObj = null;
        }

        if (m_TubeSpriteRenderer != null)
        {
            m_TubeSpriteRenderer.enabled = false;
        }

        // 匹配对应形状外壳预制体
        GameObject targetPrefab = null;
        switch (shellType)
        {
            case FireworkComponent.CylinderShell: targetPrefab = shellCylinderPrefab; break;
            case FireworkComponent.PyramidShell: targetPrefab = shellPyramidPrefab; break;
            case FireworkComponent.RoundShell: targetPrefab = shellRoundPrefab; break;
            case FireworkComponent.HeartShell: targetPrefab = shellHeartPrefab; break;
            case FireworkComponent.EllipseShell: targetPrefab = shellEllipsePrefab; break;
            default:
                Debug.LogError($"无匹配的外壳预制体：{shellType}");
                return;
        }

        // 生成新外壳
        if (targetPrefab != null)
        {
            // 🔥 核心：隐藏 stackRoot → 所有内部零件（火药/彩珠/引线）全部隐藏！
            stackRoot.gameObject.SetActive(false);

            // 外壳生成在 纸筒根节点（和stackRoot同级，不会被隐藏）
            m_CurrentShellObj = Instantiate(targetPrefab, transform);
            m_CurrentShellObj.transform.localPosition = Vector3.zero;

            // 外壳层级最高
            if (m_CurrentShellObj.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.sortingOrder = 100; // 固定最高层级，确保覆盖
            }

            m_CurrentShellObj.SetActive(true);
            m_StackedItems.Add(m_CurrentShellObj);

            Debug.Log($"生成{shellType}外壳，内部零件已全部隐藏");
        }
        else
        {
            Debug.LogError($"未绑定{shellType}外壳预制体！");
        }
    }

    /// <summary>
    /// 生成彩纸堆叠视觉效果
    /// </summary>
    private void SpawnPaperInStack(FireworkComponent paperType)
    {
        GameObject targetPrefab = null;

        // 匹配对应颜色彩纸预制体
        switch (paperType)
        {
            case FireworkComponent.RedPaper: targetPrefab = paperRedPrefab; break;
            case FireworkComponent.YellowPaper: targetPrefab = paperYellowPrefab; break;
            case FireworkComponent.GreenPaper: targetPrefab = paperGreenPrefab; break;
            default:
                Debug.LogError($"无匹配的彩纸预制体：{paperType}");
                return;
        }

        // 生成彩纸实例
        if (targetPrefab != null)
        {
            int currentMaxOrder = GetCurrentMaxOrder();
            GameObject paper = Instantiate(targetPrefab, stackRoot);
            paper.transform.localPosition = Vector3.zero;
            if (paper.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.sortingOrder = currentMaxOrder + orderIncrement;
            }
            m_StackedItems.Add(paper);
            Debug.Log($"生成{paperType}彩纸堆叠，当前数量：{m_StackedItems.Count}");
        }
        else
        {
            Debug.LogError($"未绑定{paperType}彩纸预制体！");
        }
    }

    /// <summary>
    /// 生成隔板堆叠视觉效果
    /// </summary>
    private void SpawnPartitionInStack()
    {
        if (partitionPrefab == null)
        {
            Debug.LogError("未绑定隔板预制体！");
            return;
        }

        int currentMaxOrder = GetCurrentMaxOrder();
        GameObject partition = Instantiate(partitionPrefab, stackRoot);
        partition.transform.localPosition = Vector3.zero;
        if (partition.TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.sortingOrder = currentMaxOrder + orderIncrement;
        }
        m_StackedItems.Add(partition);
        Debug.Log($"生成隔板堆叠，当前数量：{m_StackedItems.Count}");
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 获取当前堆叠的最大层级（避免组件遮挡）
    /// </summary>
    private int GetCurrentMaxOrder()
    {
        int maxOrder = 0;
        foreach (var item in m_StackedItems)
        {
            if (item.TryGetComponent<SpriteRenderer>(out var sr) && sr.sortingOrder > maxOrder)
            {
                maxOrder = sr.sortingOrder;
            }
        }
        return maxOrder;
    }

    /// <summary>
    /// 获取纸筒中某类别的组件（判定核心方法）
    /// </summary>
    public FireworkComponent GetComponentInCategory(ComponentCategory category)
    {
        m_CategoryComponentMap.TryGetValue(category, out FireworkComponent comp);
        return comp;
    }

    /// <summary>
    /// 检查纸筒是否包含某类别组件
    /// </summary>
    public bool HasCategoryComponent(ComponentCategory category)
    {
        return m_CategoryComponentMap.ContainsKey(category);
    }
    #endregion

    #region 重置方法
    /// <summary>
    /// 清空所有堆叠内容（提交后重置）
    /// </summary>
    public void ClearStack()
    {
        // 1. 恢复纸筒显示，销毁外壳
        gameObject.SetActive(true);
        if (m_CurrentShellObj != null)
        {
            Destroy(m_CurrentShellObj);
            m_CurrentShellObj = null;
        }

        // 2. 销毁所有堆叠物品实例
        foreach (var item in m_StackedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        m_StackedItems.Clear();

        // 3. 清空组件和类别记录
        m_StackedFireworkComponents.Clear();
        m_CategoryComponentMap.Clear();

        Debug.Log("纸筒已清空，重置完成");
    }
    #endregion
}