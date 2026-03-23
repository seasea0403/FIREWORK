using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PackageTableLogic : MonoBehaviour
{
    [Header("=== 纸筒配置 ===")]
    public GameObject existingPaperTube;
    public Transform tubeTargetPoint;
    public Vector3 tubeTargetScale = new Vector3(0.8f, 0.8f, 0.8f);
    public float tubeTargetAlpha = 1f;
    public float tubeMoveDuration = 0.5f;


    // 私有变量（加m_前缀）
    private bool m_IsTubeMoved = false;
    private SpriteRenderer m_TubeSpriteRenderer;
    private Vector3 m_TubeOriginalPos;
    private Vector3 m_TubeOriginalScale;
    private Color m_TubeOriginalColor;

    void Start()
    {
        // 初始化纸筒参数
        if (existingPaperTube != null)
        {
            m_TubeOriginalPos = existingPaperTube.transform.position;
            m_TubeOriginalScale = existingPaperTube.transform.localScale;

            // 获取精灵渲染器
            m_TubeSpriteRenderer = existingPaperTube.GetComponent<SpriteRenderer>();
            if (m_TubeSpriteRenderer == null)
            {
                m_TubeSpriteRenderer = existingPaperTube.GetComponentInChildren<SpriteRenderer>();
            }

            // 缓存初始颜色
            if (m_TubeSpriteRenderer != null)
            {
                m_TubeOriginalColor = m_TubeSpriteRenderer.color;
            }
        }

    }

    /// <summary>
    /// 点击工作台移动纸筒
    /// </summary>
    void OnMouseDown()
    {
        if (m_IsTubeMoved || existingPaperTube == null) return;
        MovePaperTubeToTable();
    }

    /// <summary>
    /// 移动纸筒到工作台
    /// </summary>
    public void MovePaperTubeToTable()
    {
        // 设置父物体
        existingPaperTube.transform.parent = transform;

        // 移动动画
        existingPaperTube.transform.DOMove(tubeTargetPoint.position, tubeMoveDuration)
            .SetEase(Ease.Linear);

        // 缩放动画
        existingPaperTube.transform.DOScale(tubeTargetScale, tubeMoveDuration)
            .SetEase(Ease.Linear);

        // 透明度动画
        if (m_TubeSpriteRenderer != null)
        {
            m_TubeSpriteRenderer.DOFade(tubeTargetAlpha, tubeMoveDuration)
                .SetEase(Ease.OutQuad);
        }

        m_IsTubeMoved = true;
        Debug.Log("纸筒已移动");
    }

    /// <summary>
    /// 重置工作台
    /// </summary>
    public void ResetPackageTable()
    {
        if (existingPaperTube == null) return;

        // 停止所有动画
        existingPaperTube.transform.DOKill();
        if (m_TubeSpriteRenderer != null)
        {
            m_TubeSpriteRenderer.DOKill();
        }

        // 复位纸筒
        existingPaperTube.transform.parent = null;
        existingPaperTube.transform.position = m_TubeOriginalPos;
        existingPaperTube.transform.localScale = m_TubeOriginalScale;

        // 复位颜色
        if (m_TubeSpriteRenderer != null)
        {
            m_TubeSpriteRenderer.color = m_TubeOriginalColor;
        }

        // 重置标记
        m_IsTubeMoved = false;


        Debug.Log("工作台已重置！");
    }
}