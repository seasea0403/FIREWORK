using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PackageTableLogic : MonoBehaviour
{
    [Header("=== 纸筒配置 ===")]
    public GameObject existingPaperTube; // 场景中已有的纸筒物体（不再是预制体）
    public Transform tubeTargetPoint; // 纸筒要移动到的桌子目标位置
    public Vector3 tubeTargetScale = new Vector3(0.8f, 0.8f, 0.8f); // 纸筒目标缩放
    public float tubeTargetAlpha = 1f; // 纸筒最终透明度（不透明）
    public float tubeMoveDuration = 0.5f; // 纸筒移动+缩放+透明度动画时长

    // 私有变量
    private bool isTubeMoved = false; // 纸筒是否已移动到桌子（防重复操作）
    private SpriteRenderer tubeSpriteRenderer; // 纸筒的精灵渲染器
    private Vector3 tubeOriginalPos; // 纸筒初始位置（用于重置）
    private Vector3 tubeOriginalScale; // 纸筒初始缩放（用于重置）
    private Color tubeOriginalColor; // 纸筒初始颜色（用于重置）

    void Start()
    {
        // 初始化：获取纸筒的精灵渲染器（提前缓存）
        if (existingPaperTube != null)
        {
            // 缓存纸筒初始状态（用于重置）
            tubeOriginalPos = existingPaperTube.transform.position;
            tubeOriginalScale = existingPaperTube.transform.localScale;

            // 获取精灵渲染器
            tubeSpriteRenderer = existingPaperTube.GetComponent<SpriteRenderer>();
            if (tubeSpriteRenderer == null)
            {
                tubeSpriteRenderer = existingPaperTube.GetComponentInChildren<SpriteRenderer>();
            }

            // 缓存初始颜色（含透明度）
            if (tubeSpriteRenderer != null)
            {
                tubeOriginalColor = tubeSpriteRenderer.color;
            }

        }
    }

    #region 1. 点击桌子移动纸筒到目标位置
    void OnMouseDown()
    {
        // 防止重复移动纸筒
        if (isTubeMoved || existingPaperTube == null) return;

        // 移动纸筒到桌子位置并调整状态
        MovePaperTubeToTable();
    }

    // 核心逻辑：移动已有纸筒 + 缩放 + 不透明化
    public void MovePaperTubeToTable()
    {

        // 2. 设置父物体（可选：让纸筒跟随桌子移动）
        existingPaperTube.transform.parent = transform;

        // 3. 动画：移动到桌子目标位置（移除弹跳，改为线性无抖动）
        existingPaperTube.transform.DOMove(tubeTargetPoint.position, tubeMoveDuration)
            .SetEase(Ease.Linear); // ✅ 关键修改：去掉OutBounce，改为Linear

        // 4. 动画：缩放到目标大小（移除弹跳，改为线性无抖动）
        existingPaperTube.transform.DOScale(tubeTargetScale, tubeMoveDuration)
            .SetEase(Ease.Linear); // ✅ 关键修改：去掉OutBounce，改为Linear

        // 5. 动画：从不透明到透明（不透明化）
        if (tubeSpriteRenderer != null)
        {
            tubeSpriteRenderer.DOFade(tubeTargetAlpha, tubeMoveDuration)
                .SetEase(Ease.OutQuad);
        }

        // 标记为已移动，防止重复操作
        isTubeMoved = true;

        Debug.Log("纸筒已移动到包装区桌子！");
    }
    #endregion


    // 重置包装区（纸筒回到初始状态）
    public void ResetPackageTable()
    {
        if (existingPaperTube == null) return;

        // 停止所有动画（防止重置时动画还在运行）
        existingPaperTube.transform.DOKill();
        if (tubeSpriteRenderer != null)
        {
            tubeSpriteRenderer.DOKill();
        }

        // 重置位置、缩放、父物体
        existingPaperTube.transform.parent = null; // 取消父物体
        existingPaperTube.transform.position = tubeOriginalPos; // 回到初始位置
        existingPaperTube.transform.localScale = tubeOriginalScale; // 回到初始缩放

        // 重置颜色（含透明度）
        if (tubeSpriteRenderer != null)
        {
            tubeSpriteRenderer.color = tubeOriginalColor;
        }

        // 重置标记
        isTubeMoved = false;

        Debug.Log("纸筒已重置到初始状态！");
    }
}