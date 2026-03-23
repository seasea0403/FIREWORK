using UnityEngine;
using DG.Tweening; // 必须导入DOTween命名空间

/// <summary>
/// 极简版相机移动（DOTween）：点击区域后相机移动到指定位置
/// </summary>
public class SimpleCameraMoveDOTween : MonoBehaviour
{
    [Header("相机配置")]
    [Tooltip("要控制的主相机（默认取MainCamera）")]
    public Camera mainCamera;
    [Tooltip("相机移动的目标位置（世界坐标）")]
    public Vector3 targetPosition;
    [Tooltip("移动时长（秒）")]
    public float moveTime = 0.5f;
    [Tooltip("缓动效果（DOTween内置，可选）")]
    public Ease moveEase = Ease.OutQuad; // 顺滑的出缓动，适合相机移动

    void Awake()
    {
        // 自动获取主相机，无需手动绑定
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    /// <summary>
    /// 核心方法：触发相机移动（绑定到点击区域/按钮）
    /// </summary>
    public void MoveCameraToTarget()
    {
        if (mainCamera == null)
        {
            Debug.LogError("未找到主相机！");
            return;
        }

        // 停止当前所有相机动画，避免重复触发
        mainCamera.transform.DOKill();

        // DOTween核心：平滑移动相机到目标位置
        mainCamera.transform.DOMove(targetPosition, moveTime)
            .SetEase(moveEase) // 应用缓动效果
            .SetUpdate(true);  // 不受Time.timeScale影响（比如暂停时也能移动）
    }

    // 方式1：场景内2D碰撞区域点击触发（比如透明按钮）
    void OnMouseDown()
    {
        MoveCameraToTarget();
    }

    // 可选：Gizmos可视化目标位置（调试用）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.2f); // 红色小球标记目标位置
    }
}