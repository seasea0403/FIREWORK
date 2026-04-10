using UnityEngine;
using DG.Tweening;

/// <summary>
/// 纸筒预制体管理器：负责纸筒预制体的生成、销毁、目标移动和下一天自动重置。
/// 定义了核心函数：SpawnNewPaperTube、MovePaperTubeToTarget、ResetAndMoveNewTube、ResetPaperTube。
/// </summary>
public class PaperTubeManager : MonoBehaviour
{
    [Header("=== 纸筒预制体配置 ===")]
    public GameObject paperTubePrefab;
    public Transform spawnPoint;
    public Transform targetPoint;
    public Vector3 targetScale = new Vector3(0.8f, 0.8f, 0.8f);
    public float targetAlpha = 1f;
    public float moveDuration = 0.5f;

    public static PaperTubeManager Instance { get; private set; }

    // 当前纸筒实例
    private GameObject m_CurrentPaperTube;
    private SpriteRenderer m_TubeSpriteRenderer;
    private Vector3 m_OriginalPos;
    private Vector3 m_OriginalScale;
    private Color m_OriginalColor;

    /// <summary>
    /// 初始化纸筒预制体管理器单例。
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 场景开始时生成第一个纸筒实例。
    /// </summary>
    void Start()
    {
        // 初始生成纸筒
        SpawnNewPaperTube();
    }

    /// <summary>
    /// 生成新的纸筒实例
    /// </summary>
    public void SpawnNewPaperTube()
    {
        // 销毁旧实例
        if (m_CurrentPaperTube != null)
        {
            Destroy(m_CurrentPaperTube);
            m_CurrentPaperTube = null;
        }

        if (paperTubePrefab == null)
        {
            Debug.LogWarning("未配置纸筒预制体！");
            return;
        }

        // 生成新实例
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        m_CurrentPaperTube = Instantiate(paperTubePrefab, spawnPos, spawnRot);
        m_CurrentPaperTube.name = paperTubePrefab.name + "_Instance";

        // 记录原始状态
        m_OriginalPos = m_CurrentPaperTube.transform.position;
        m_OriginalScale = m_CurrentPaperTube.transform.localScale;

        // 获取渲染器
        m_TubeSpriteRenderer = m_CurrentPaperTube.GetComponent<SpriteRenderer>();
        if (m_TubeSpriteRenderer == null)
        {
            m_TubeSpriteRenderer = m_CurrentPaperTube.GetComponentInChildren<SpriteRenderer>();
        }

        if (m_TubeSpriteRenderer != null)
        {
            m_OriginalColor = m_TubeSpriteRenderer.color;
        }

        Debug.Log("新纸筒已生成");
    }

    /// <summary>
    /// 将纸筒移动到目标位置
    /// </summary>
    public void MovePaperTubeToTarget()
    {
        if (m_CurrentPaperTube == null)
        {
            SpawnNewPaperTube();
        }

        if (m_CurrentPaperTube == null || targetPoint == null)
        {
            Debug.LogWarning("纸筒实例或目标点不存在！");
            return;
        }

        // 设置父物体
        m_CurrentPaperTube.transform.parent = transform;

        // 移动动画
        m_CurrentPaperTube.transform.DOMove(targetPoint.position, moveDuration)
            .SetEase(Ease.Linear);

        // 缩放动画
        m_CurrentPaperTube.transform.DOScale(targetScale, moveDuration)
            .SetEase(Ease.Linear);

        // 透明度动画
        if (m_TubeSpriteRenderer != null)
        {
            m_TubeSpriteRenderer.DOFade(targetAlpha, moveDuration)
                .SetEase(Ease.OutQuad);
        }

        Debug.Log("纸筒已移动到目标位置");
    }

    /// <summary>
    /// 重置并移动新纸筒（用于下一天）
    /// </summary>
    public void ResetAndMoveNewTube()
    {
        SpawnNewPaperTube();
        MovePaperTubeToTarget();
    }

    /// <summary>
    /// 重置纸筒到原始位置
    /// </summary>
    public void ResetPaperTube()
    {
        if (m_CurrentPaperTube == null) return;

        // 停止动画
        m_CurrentPaperTube.transform.DOKill();
        if (m_TubeSpriteRenderer != null)
        {
            m_TubeSpriteRenderer.DOKill();
        }

        // 复位
        m_CurrentPaperTube.transform.parent = null;
        m_CurrentPaperTube.transform.position = m_OriginalPos;
        m_CurrentPaperTube.transform.localScale = m_OriginalScale;

        if (m_TubeSpriteRenderer != null)
        {
            m_TubeSpriteRenderer.color = m_OriginalColor;
        }

        Debug.Log("纸筒已重置");
    }
}