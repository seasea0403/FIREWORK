using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PaperTubeCompress : MonoBehaviour
{
    [Header("=== 压缩开关设置 ===")]
    public GameObject compressSwitch;

    [Header("=== 滑杆设置 ===")]
    public Transform slideBarTop;
    public Transform slideBarBottom;
    [Tooltip("上滑杆压缩后的目标Y坐标")]
    public float topBarTargetY = -0.3f;
    [Tooltip("下滑杆压缩后的目标Y坐标")]
    public float bottomBarTargetY = 0.3f;
    public float compressDuration = 0.5f;

    [Header("=== 内容物设置 ===")]
    public Transform stackRoot;
    public GameObject compressedContent;
    public PaperTube paperTube; // 关联的纸筒

    [Header("=== 移动设置 ===")]
    public float packageAreaX = 5f;
    public float moveDuration = 1f;

    // 私有变量（加m_前缀）
    private GameObject m_SpawnedCompressedContent;
    private bool m_IsCompressed = false;

    void Start()
    {
        // 初始化开关碰撞体
        if (compressSwitch != null && !compressSwitch.GetComponent<BoxCollider2D>())
        {
            compressSwitch.AddComponent<BoxCollider2D>();
            Debug.Log("已为开关添加BoxCollider2D");
        }

        // 挂载开关点击检测
        if (compressSwitch != null && !compressSwitch.GetComponent<SwitchClickDetector>())
        {
            SwitchClickDetector detector = compressSwitch.AddComponent<SwitchClickDetector>();
            detector.CompressScript = this;
            Debug.Log("已为开关挂载点击检测脚本");
        }

        // 初始化压缩效果图
        if (compressedContent != null && stackRoot != null)
        {
            m_SpawnedCompressedContent = Instantiate(compressedContent, stackRoot);
            m_SpawnedCompressedContent.transform.localPosition = Vector3.zero;
            m_SpawnedCompressedContent.SetActive(false);
        }
    }

    /// <summary>
    /// 触发压缩逻辑
    /// </summary>
    public void TriggerCompress()
    {
        if (m_IsCompressed || paperTube == null) return;
        m_IsCompressed = true;

        Debug.Log("开始压缩纸筒...");

        // 步骤1：压缩滑杆
        CompressSlideBars();
        // 步骤2：替换内容物
        DOVirtual.DelayedCall(compressDuration, ReplaceContent);
        // 步骤3：移动到包装区 + 执行判定
        DOVirtual.DelayedCall(compressDuration + 0.2f, () =>
        {
            MoveToPackageArea();
        });
    }

    #region 核心逻辑（保留原有动画）
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

    private void ReplaceContent()
    {
        // 隐藏原有内容
        foreach (Transform child in stackRoot)
        {
            if (child.gameObject != m_SpawnedCompressedContent)
            {
                child.gameObject.SetActive(false);
            }
        }
        // 显示压缩效果图
        if (m_SpawnedCompressedContent != null)
        {
            m_SpawnedCompressedContent.SetActive(true);
        }
        Debug.Log("内容物已替换为压缩效果");
    }

    private void MoveToPackageArea()
    {
        transform.DOMoveX(packageAreaX, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Debug.Log("已移动到包装区！");
            });
    }
    #endregion

    /// <summary>
    /// 重置压缩状态
    /// </summary>
    public void ResetState()
    {
        m_IsCompressed = false;

        // 滑杆复位
        if (slideBarTop != null) slideBarTop.DOLocalMoveY(1f, 0.5f);
        if (slideBarBottom != null) slideBarBottom.DOLocalMoveY(-1f, 0.5f);

        // 恢复内容物
        foreach (Transform child in stackRoot)
        {
            child.gameObject.SetActive(true);
        }
        if (m_SpawnedCompressedContent != null)
        {
            m_SpawnedCompressedContent.SetActive(false);
        }

        // 位置复位
        transform.DOMoveX(0f, 0.5f);
    }
}

/// <summary>
/// 开关点击检测
/// </summary>
public class SwitchClickDetector : MonoBehaviour
{
    public PaperTubeCompress CompressScript;

    void OnMouseDown()
    {
        if (CompressScript != null)
        {
            CompressScript.TriggerCompress();
        }
        else
        {
            Debug.LogError("开关未关联压缩脚本！");
        }
    }
}