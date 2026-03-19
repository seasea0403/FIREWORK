using UnityEngine;
using System.Collections.Generic;

public class PaperTube : MonoBehaviour
{
    [Header("=== 堆叠素材预制体 ===")]
    public GameObject powderIn1Prefab;
    public GameObject powderIn2Prefab;
    public GameObject powderIn3Prefab;
    public GameObject beadInRedPrefab;
    public GameObject beadInBluePrefab;
    public GameObject beadInGoldPrefab;
    public GameObject beadInGreenPrefab;
    public GameObject beadInLightBluePrefab;
    public GameObject beadInPurplePrefab;
    public GameObject clay1Prefab;
    public GameObject clay2Prefab;
    public GameObject fusePrefab;
    public GameObject shellCylinderPrefab;
    public GameObject shellPyramidPrefab;
    public GameObject shellRoundPrefab;

    [Header("=== 堆叠设置 ===")]
    public Transform stackRoot;
    public int orderIncrement = 1;

    private List<GameObject> stackedItems = new List<GameObject>();
    private GameObject currentShellObj; // 记录当前外壳
    private Transform originalShellParent; // 记录外壳原本的父级（用于重置）

    void Start()
    {
        if (stackRoot == null)
        {
            stackRoot = new GameObject("StackRoot").transform;
            stackRoot.parent = transform;
            stackRoot.localPosition = Vector3.zero;
        }
    }

    void OnMouseDown()
    {
        ContentItem selectedItem = ContentSelector.Instance.currentSelectedItem;
        if (selectedItem == null)
        {
            Debug.Log("请先选中物品！");
            return;
        }

        switch (selectedItem.itemType)
        {
            case ItemType.Powder_In:
                SpawnPowderInStack();
                break;
            case ItemType.Bead_In:
                SpawnBeadInStack(selectedItem.beadColor);
                break;
            case ItemType.Package_Clay:
                SpawnClayInStack();
                break;
            case ItemType.Package_Fuse:
                SpawnFuseInStack();
                break;
            case ItemType.Shell:
                SpawnShellStack(selectedItem.shellShape);
                break;
            default:
                Debug.Log("不支持的物品类型！");
                break;
        }

        selectedItem.Deselect();
        ContentSelector.Instance.currentSelectedItem = null;
    }

    // 生成火药（原有逻辑不变）
    private void SpawnPowderInStack()
    {
        int currentMaxOrder = GetCurrentMaxOrder();

        if (powderIn1Prefab != null)
        {
            GameObject frame1 = Instantiate(powderIn1Prefab, stackRoot);
            frame1.transform.localPosition = Vector3.zero;
            SpriteRenderer sr1 = frame1.GetComponent<SpriteRenderer>();
            if (sr1 != null) sr1.sortingOrder = currentMaxOrder + 1;
            stackedItems.Add(frame1);
        }

        if (powderIn2Prefab != null)
        {
            GameObject frame2 = Instantiate(powderIn2Prefab, stackRoot);
            frame2.transform.localPosition = Vector3.zero;
            SpriteRenderer sr2 = frame2.GetComponent<SpriteRenderer>();
            if (sr2 != null) sr2.sortingOrder = currentMaxOrder + 2;
            stackedItems.Add(frame2);
        }

        if (powderIn3Prefab != null)
        {
            GameObject frame3 = Instantiate(powderIn3Prefab, stackRoot);
            frame3.transform.localPosition = Vector3.zero;
            SpriteRenderer sr3 = frame3.GetComponent<SpriteRenderer>();
            if (sr3 != null) sr3.sortingOrder = currentMaxOrder + 3;
            stackedItems.Add(frame3);
        }

        Debug.Log("火药三帧已堆叠到纸筒，共 " + stackedItems.Count + " 个素材");
    }

    // 生成彩珠（原有逻辑不变）
    private void SpawnBeadInStack(BeadInColor color)
    {
        GameObject targetPrefab = null;
        switch (color)
        {
            case BeadInColor.Red: targetPrefab = beadInRedPrefab; break;
            case BeadInColor.Blue: targetPrefab = beadInBluePrefab; break;
            case BeadInColor.Gold: targetPrefab = beadInGoldPrefab; break;
            case BeadInColor.Green: targetPrefab = beadInGreenPrefab; break;
            case BeadInColor.LightBlue: targetPrefab = beadInLightBluePrefab; break;
            case BeadInColor.Purple: targetPrefab = beadInPurplePrefab; break;
            default: Debug.LogError("未配置该颜色彩珠的预制体！"); return;
        }

        if (targetPrefab == null)
        {
            Debug.LogError($"未设置 {color} 彩珠的堆叠预制体！");
            return;
        }

        int currentMaxOrder = GetCurrentMaxOrder();
        GameObject bead = Instantiate(targetPrefab, stackRoot);
        bead.transform.localPosition = Vector3.zero;
        SpriteRenderer sr = bead.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = currentMaxOrder + 1;
        stackedItems.Add(bead);

        Debug.Log($"{color} 彩珠已堆叠到纸筒，共 " + stackedItems.Count + " 个素材");
    }

    // 添加粘土（原有逻辑不变）
    private void SpawnClayInStack()
    {
        int currentMaxOrder = GetCurrentMaxOrder();

        if (clay1Prefab != null)
        {
            GameObject frame1 = Instantiate(clay1Prefab, stackRoot);
            frame1.transform.localPosition = Vector3.zero;
            SpriteRenderer sr1 = frame1.GetComponent<SpriteRenderer>();
            if (sr1 != null) sr1.sortingOrder = currentMaxOrder + 1;
            stackedItems.Add(frame1);
        }

        if (clay2Prefab != null)
        {
            GameObject frame2 = Instantiate(clay2Prefab, stackRoot);
            frame2.transform.localPosition = Vector3.zero;
            SpriteRenderer sr2 = frame2.GetComponent<SpriteRenderer>();
            if (sr2 != null) sr2.sortingOrder = currentMaxOrder + 2;
            stackedItems.Add(frame2);
        }

        Debug.Log($"粘土已堆叠到纸筒，共 " + stackedItems.Count + " 个素材");
    }

    // 添加引线（原有逻辑不变）
    private void SpawnFuseInStack()
    {
        int currentMaxOrder = GetCurrentMaxOrder();
        GameObject fuse = Instantiate(fusePrefab, stackRoot);
        fuse.transform.localPosition = Vector3.zero;
        SpriteRenderer sr = fuse.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = currentMaxOrder + 1;
        stackedItems.Add(fuse);

        Debug.Log($"引线已堆叠到纸筒，共 " + stackedItems.Count + " 个素材");
    }

    // 生成外壳（核心修改：父物体隐藏+子物体显示）
    private void SpawnShellStack(ShellShape shape)
    {
        // 1. 销毁旧外壳（互斥逻辑）
        if (currentShellObj != null)
        {
            // 恢复旧外壳的父级（如果需要）
            if (originalShellParent != null)
                currentShellObj.transform.parent = originalShellParent;
            Destroy(currentShellObj);
            stackedItems.Remove(currentShellObj);
        }

        // 2. 匹配外壳预制体
        GameObject targetPrefab = null;
        switch (shape)
        {
            case ShellShape.Cylinder: targetPrefab = shellCylinderPrefab; break;
            case ShellShape.Pyramid: targetPrefab = shellPyramidPrefab; break;
            case ShellShape.Round: targetPrefab = shellRoundPrefab; break;
            default: Debug.LogError("未配置该包装！"); return;
        }

        if (targetPrefab == null)
        {
            Debug.LogError($"未设置 {shape} 外壳的堆叠预制体！");
            return;
        }

        // 3. 生成新外壳（先挂载到stackRoot）
        int currentMaxOrder = GetCurrentMaxOrder();
        currentShellObj = Instantiate(targetPrefab, stackRoot);
        currentShellObj.transform.localPosition = Vector3.zero;
        SpriteRenderer sr = currentShellObj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = currentMaxOrder + 10; // 确保层级最高
        stackedItems.Add(currentShellObj);

        // 4. 关键：记录外壳原本的父级，然后解除父子关系
        originalShellParent = currentShellObj.transform.parent;
        currentShellObj.transform.parent = null; // 脱离父物体，不受父物体激活状态影响

        // 5. 隐藏绑定脚本的父物体（纸筒）
        gameObject.SetActive(false);

        // 6. 强制显示外壳（确保子物体显示）
        currentShellObj.SetActive(true);

        Debug.Log($"{shape} 包装已生成，纸筒（父物体）已隐藏，外壳（子物体）保持显示");
    }

    // 获取当前最大sortingOrder（原有逻辑不变）
    private int GetCurrentMaxOrder()
    {
        int maxOrder = 0;
        foreach (var item in stackedItems)
        {
            if (item.TryGetComponent<SpriteRenderer>(out var sr) && sr.sortingOrder > maxOrder)
                maxOrder = sr.sortingOrder;
        }
        return maxOrder;
    }

    // 清空纸筒（重置父物体+子物体状态）
    public void ClearStack()
    {
        // 1. 恢复父物体（纸筒）显示
        gameObject.SetActive(true);

        // 2. 销毁外壳并恢复父子关系
        if (currentShellObj != null)
        {
            if (originalShellParent != null)
                currentShellObj.transform.parent = originalShellParent;
            Destroy(currentShellObj);
            currentShellObj = null;
            originalShellParent = null;
        }

        // 3. 销毁所有堆叠物品
        foreach (var item in stackedItems)
            Destroy(item);
        stackedItems.Clear();
    }
}