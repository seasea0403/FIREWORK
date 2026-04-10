using UnityEngine;
using DG.Tweening;

/// <summary>
/// 简单摄像机移动控制器，支持切换到柜台和工作台视角。
/// </summary>
public class SimpleCameraMoveDOTween : MonoBehaviour
{
    [Header("摄像机设置")]
    public Camera mainCamera;
    public Vector3 CounterPosition; // 柜台位置
    public Vector3 CraftPosition; // 工作台位置
    public float moveTime = 0.5f;
    public Ease moveEase = Ease.OutQuad;

    // 相机拖动脚本引用
    private CameraDragMove _dragMove;

    void Awake()
    {
        _dragMove = mainCamera.GetComponent<CameraDragMove>();
    }

    /// <summary>
    /// 将摄像机移动到柜台位置。
    /// </summary>
    public void MoveCameraToCounter()
    {
        if (_dragMove != null)
        {
            _dragMove.canDrag = false;
        }

        Vector3 safeTarget = CounterPosition;

        mainCamera.transform.DOKill();
        mainCamera.transform.DOMove(safeTarget, moveTime)
            .SetEase(moveEase)
            .SetUpdate(true);
    }

    /// <summary>
    /// 将摄像机移动回工作台位置，并重新启用拖动。
    /// </summary>
    public void MoveBackToCraft()
    {
        if (_dragMove != null)
        {
            _dragMove.canDrag = true;
        }

        Vector3 safeTarget = CraftPosition;
        safeTarget.z = mainCamera.transform.position.z;

        mainCamera.transform.DOMove(safeTarget, moveTime)
            .SetEase(moveEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (_dragMove != null) _dragMove.EnableDrag();
            });
    }
}