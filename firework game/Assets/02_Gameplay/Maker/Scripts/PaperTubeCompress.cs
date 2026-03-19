using UnityEngine;
using DG.Tweening;

public class PaperTubeCompress : MonoBehaviour
{
    [Header("=== 压缩开关设置 ===")]
    public GameObject compressSwitch; // 控制压缩的独立开关物件（拖入你的开关）

    [Header("=== 滑杆设置 ===")]
    public Transform slideBarTop; // 上滑杆
    public Transform slideBarBottom; // 下滑杆
    [Tooltip("上滑杆压缩后的目标Y坐标")]
    public float topBarTargetY = -0.3f;
    [Tooltip("下滑杆压缩后的目标Y坐标")]
    public float bottomBarTargetY = 0.3f;
    public float compressDuration = 0.5f; // 压缩动画时长

    [Header("=== 内容物设置 ===")]
    public Transform stackRoot; // 纸筒内堆叠素材根节点
    public GameObject compressedContent; // 压缩后效果图预制体
    private GameObject spawnedCompressedContent; // 生成的压缩后效果图

    [Header("=== 移动设置 ===")]
    public float packageAreaX = 5f; // 包装区X坐标
    public float moveDuration = 1f; // 移动动画时长
    private bool isCompressed = false; // 是否已完成压缩

    void Start()
    {
        // 1. 给开关自动加碰撞体（确保能点击）
        if (compressSwitch != null && !compressSwitch.GetComponent<BoxCollider2D>())
        {
            compressSwitch.AddComponent<BoxCollider2D>();
            Debug.Log("已为开关自动添加BoxCollider2D");
        }
        // 2. 给开关挂载点击检测脚本（自动挂载，不用手动加）
        if (compressSwitch != null && !compressSwitch.GetComponent<SwitchClickDetector>())
        {
            SwitchClickDetector detector = compressSwitch.AddComponent<SwitchClickDetector>();
            detector.compressScript = this; // 关联当前压缩脚本
            Debug.Log("已为开关挂载点击检测脚本");
        }

        // 3. 初始化压缩后效果图
        if (compressedContent != null && stackRoot != null)
        {
            spawnedCompressedContent = Instantiate(compressedContent, stackRoot);
            spawnedCompressedContent.transform.localPosition = Vector3.zero;
            spawnedCompressedContent.SetActive(false);
        }
    }

    // 开关点击后执行的核心压缩逻辑（外部调用）
    public void TriggerCompress()
    {
        if (isCompressed) return; // 防止重复点击
        isCompressed = true;

        Debug.Log("开关点击生效！开始压缩滑杆...");

        // 第一步：压缩滑杆
        CompressSlideBars();
        // 第二步：替换内容物（压缩完成后）
        DOVirtual.DelayedCall(compressDuration, ReplaceContent);
        // 第三步：移动到包装区（替换内容后延迟0.2秒）
        DOVirtual.DelayedCall(compressDuration + 0.2f, MoveToPackageArea);
    }

    // 滑杆向中间压缩（仅控制Y轴）
    private void CompressSlideBars()
    {
        if (slideBarTop != null)
        {
            slideBarTop.DOLocalMoveY(topBarTargetY, compressDuration)
                .SetEase(Ease.OutQuad);
        }
        if (slideBarBottom != null)
        {
            slideBarBottom.DOLocalMoveY(bottomBarTargetY, compressDuration)
                .SetEase(Ease.OutQuad);
        }
    }

    // 替换内容物：隐藏原有素材，显示压缩图
    private void ReplaceContent()
    {
        foreach (Transform child in stackRoot)
        {
            if (child.gameObject != spawnedCompressedContent)
            {
                child.gameObject.SetActive(false);
            }
        }
        if (spawnedCompressedContent != null)
        {
            spawnedCompressedContent.SetActive(true);
        }
        Debug.Log("内容物已替换为压缩后效果");
    }

    // 整体沿X轴向右移动到包装区
    private void MoveToPackageArea()
    {
        transform.DOMoveX(packageAreaX, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                Debug.Log("已移动到包装区！");
            });
    }

    // 重置状态（可选，重新制作时调用）
    public void ResetState()
    {
        isCompressed = false;
        // 滑杆回初始位置（根据你的实际初始值调整）
        if (slideBarTop != null) slideBarTop.DOLocalMoveY(1f, 0.5f);
        if (slideBarBottom != null) slideBarBottom.DOLocalMoveY(-1f, 0.5f);
        // 恢复原有素材
        foreach (Transform child in stackRoot)
        {
            child.gameObject.SetActive(true);
        }
        if (spawnedCompressedContent != null)
        {
            spawnedCompressedContent.SetActive(false);
        }
        // 回到初始X位置
        transform.DOMoveX(0f, 0.5f);
    }
}

// 开关点击检测脚本（自动挂载到开关上）
public class SwitchClickDetector : MonoBehaviour
{
    public PaperTubeCompress compressScript; // 关联的压缩脚本

    // 鼠标点击开关时触发（无需EventSystem）
    void OnMouseDown()
    {
        if (compressScript != null)
        {
            compressScript.TriggerCompress();
        }
        else
        {
            Debug.LogError("开关未关联压缩脚本！");
        }
    }
}