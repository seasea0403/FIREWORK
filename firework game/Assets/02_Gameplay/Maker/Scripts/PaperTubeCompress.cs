using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 纸筒压缩机器逻辑，负责压缩展示并移动到打包区。
/// </summary>
public class PaperTubeCompress : MonoBehaviour
{
    [Header("=== 压缩开关 ===")]
    public GameObject compressSwitch;

    [Header("=== 滑杆设置 ===")]
    public Transform slideBarTop;
    public Transform slideBarBottom;
    [Tooltip("压缩时上方滑杆目标 Y 位置")]
    public float topBarTargetY = -0.3f;
    [Tooltip("压缩时下方滑杆目标 Y 位置")]
    public float bottomBarTargetY = 0.3f;
    public float compressDuration = 0.5f;

    [Header("=== 压缩效果 ===")]
    public Transform stackRoot;
    public GameObject compressedContent;
    public PaperTube paperTube; // 目标纸筒

    [Header("=== 移动设置 ===")]
    public float packageAreaX = 5f;
    public float moveDuration = 1f;

    // 私有成员变量
    private GameObject m_SpawnedCompressedContent;
    private bool m_IsCompressed = false;

    /// <summary>
    /// 初始化压缩机状态，添加必要的碰撞检测组件并准备压缩显示对象。
    /// </summary>
    /// <summary>
    /// 初始化压缩机状态，添加必要的碰撞检测组件并准备压缩显示对象。
    /// </summary>
    void Start()
    {
        if (compressSwitch != null && !compressSwitch.GetComponent<BoxCollider2D>())
        {
            compressSwitch.AddComponent<BoxCollider2D>();
            Debug.Log("已为压缩开关添加 BoxCollider2D");
        }

        if (compressSwitch != null && !compressSwitch.GetComponent<SwitchClickDetector>())
        {
            SwitchClickDetector detector = compressSwitch.AddComponent<SwitchClickDetector>();
            detector.CompressScript = this;
            Debug.Log("已为压缩开关添加 SwitchClickDetector");
        }

        if (compressedContent != null && stackRoot != null)
        {
            m_SpawnedCompressedContent = Instantiate(compressedContent, stackRoot);
            m_SpawnedCompressedContent.transform.localPosition = Vector3.zero;
            m_SpawnedCompressedContent.SetActive(false);
        }
    }

    /// <summary>
    /// 触发压缩流程。
    /// </summary>
    public void TriggerCompress()
    {
        if (m_IsCompressed || paperTube == null) return;
        m_IsCompressed = true;

        Debug.Log("开始压缩纸筒...");

        CompressSlideBars();
        DOVirtual.DelayedCall(compressDuration, ReplaceContent);
        DOVirtual.DelayedCall(compressDuration + 0.2f, () =>
        {
            MoveToPackageArea();
        });
    }

    #region 压缩动画与效果实现
    /// <summary>
    /// 执行压缩机滑杆收缩动画，模拟纸筒被压缩过程。
    /// </summary>
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

    /// <summary>
    /// 替换压缩机内部内容显示，将原始堆叠替换为压缩后展示。
    /// </summary>
    private void ReplaceContent()
    {
        foreach (Transform child in stackRoot)
        {
            if (child.gameObject != m_SpawnedCompressedContent)
            {
                child.gameObject.SetActive(false);
            }
        }
        if (m_SpawnedCompressedContent != null)
        {
            m_SpawnedCompressedContent.SetActive(true);
        }
        Debug.Log("已将包装内容替换为压缩效果");
    }

    /// <summary>
    /// 将压缩后的纸筒移动到打包区，并播放移动动画。
    /// </summary>
    private void MoveToPackageArea()
    {
        transform.DOMoveX(packageAreaX, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Debug.Log("已将压缩纸筒移动到打包区");
            });
    }
    #endregion

    /// <summary>
    /// 重置压缩装置状态。
    /// </summary>
    public void ResetState()
    {
        m_IsCompressed = false;

        if (slideBarTop != null) slideBarTop.DOLocalMoveY(1f, 0.5f);
        if (slideBarBottom != null) slideBarBottom.DOLocalMoveY(-1f, 0.5f);

        foreach (Transform child in stackRoot)
        {
            child.gameObject.SetActive(true);
        }
        if (m_SpawnedCompressedContent != null)
        {
            m_SpawnedCompressedContent.SetActive(false);
        }

        transform.DOMoveX(0f, 0.5f);
    }
}

/// <summary>
/// 压缩开关点击检测器。
/// </summary>
public class SwitchClickDetector : MonoBehaviour
{
    public PaperTubeCompress CompressScript;

    /// <summary>
    /// 响应鼠标点击，触发压缩机的压缩流程。
    /// </summary>
    void OnMouseDown()
    {
        if (CompressScript != null)
        {
            CompressScript.TriggerCompress();
        }
        else
        {
            Debug.LogError("当前未绑定 PaperTubeCompress 脚本");
        }
    }
}