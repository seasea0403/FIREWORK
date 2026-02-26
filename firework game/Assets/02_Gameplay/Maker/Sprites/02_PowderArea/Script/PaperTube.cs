using UnityEngine;
using System.Collections.Generic;

public class PaperTube : MonoBehaviour
{
    [Header("=== 堆叠素材预制体 ===")]
    // 火药倒入的三个帧
    public GameObject powderIn1Prefab;
    public GameObject powderIn2Prefab;
    public GameObject powderIn3Prefab;
    // 彩珠倒入预制体
    public GameObject beadInRedPrefab;
    // 后续可添加 beadInBluePrefab 等

    [Header("=== 堆叠设置 ===")]
    public Transform stackRoot; // 纸筒内的堆叠根节点（空物体）
    public int orderIncrement = 1; // 每次堆叠，Order in Layer 增加的值

    private List<GameObject> stackedItems = new List<GameObject>();

    void Start()
    {
        // 如果没设置 stackRoot，自动创建一个
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
            Debug.Log("请先选中火药或彩珠！");
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
            default:
                Debug.Log("不支持的物品类型！");
                break;
        }

        // 倒入后取消选中
        selectedItem.Deselect();
        ContentSelector.Instance.currentSelectedItem = null;
    }

    // 生成火药三帧堆叠（在同一平面，靠 Order in Layer 覆盖）
    private void SpawnPowderInStack()
    {
        // 当前最大的 Order in Layer 值
        int currentMaxOrder = GetCurrentMaxOrder();

        // 生成第1帧
        if (powderIn1Prefab != null)
        {
            GameObject frame1 = Instantiate(powderIn1Prefab, stackRoot);
            frame1.transform.localPosition = Vector3.zero; // 所有素材都在同一位置
            SpriteRenderer sr1 = frame1.GetComponent<SpriteRenderer>();
            if (sr1 != null) sr1.sortingOrder = currentMaxOrder + 1;
            stackedItems.Add(frame1);
        }

        // 生成第2帧，覆盖在第1帧上
        if (powderIn2Prefab != null)
        {
            GameObject frame2 = Instantiate(powderIn2Prefab, stackRoot);
            frame2.transform.localPosition = Vector3.zero;
            SpriteRenderer sr2 = frame2.GetComponent<SpriteRenderer>();
            if (sr2 != null) sr2.sortingOrder = currentMaxOrder + 2;
            stackedItems.Add(frame2);
        }

        // 生成第3帧，覆盖在第2帧上
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

    // 生成彩珠堆叠（在同一平面，靠 Order in Layer 覆盖）
    private void SpawnBeadInStack(BeadInColor color)
    {
        GameObject targetPrefab = null;
        switch (color)
        {
            case BeadInColor.Red:
                targetPrefab = beadInRedPrefab;
                break;
            // case BeadInColor.Blue: targetPrefab = beadInBluePrefab; break;
            default:
                Debug.LogError("未配置该颜色彩珠的预制体！");
                return;
        }

        if (targetPrefab == null)
        {
            Debug.LogError($"未设置 {color} 彩珠的堆叠预制体！");
            return;
        }

        int currentMaxOrder = GetCurrentMaxOrder();
        GameObject bead = Instantiate(targetPrefab, stackRoot);
        bead.transform.localPosition = Vector3.zero; // 所有素材都在同一位置
        SpriteRenderer sr = bead.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = currentMaxOrder + 1;
        stackedItems.Add(bead);

        Debug.Log($"{color} 彩珠已堆叠到纸筒，共 " + stackedItems.Count + " 个素材");
    }

    // 获取当前所有堆叠素材中最大的 sortingOrder
    private int GetCurrentMaxOrder()
    {
        int maxOrder = 0;
        foreach (var item in stackedItems)
        {
            if (item.TryGetComponent<SpriteRenderer>(out var sr))
            {
                if (sr.sortingOrder > maxOrder)
                    maxOrder = sr.sortingOrder;
            }
        }
        return maxOrder;
    }

    // 清空纸筒（后续压缩/重置时调用）
    public void ClearStack()
    {
        foreach (var item in stackedItems)
            Destroy(item);
        stackedItems.Clear();
    }
}