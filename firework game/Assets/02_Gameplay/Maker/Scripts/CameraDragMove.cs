using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 相机拖动控制器：支持水平拖拽、边界限制和平滑跟随，适用于工作台视角切换场景。
/// </summary>
public class CameraDragMove : MonoBehaviour
{
    [Header("=== 拖拽配置 ===")]
    public float dragSensitivity = 1f;
    public bool onlyHorizontalMove = true;

    [Header("=== 移动边界限制 ===")]
    public float minX = -5f;
    public float maxX = 10f;
    public float fixedY = 0f;

    [Header("=== 平滑配置 ===")]
    public float smoothSpeed = 0.125f;

    //外部可开关
    public bool canDrag = false;
    // 内部开关，不手动修改
    private bool isDragInitialized = false;
    private Vector3 _lastMousePos;
    private Vector3 _targetCamPos;

    /// <summary>
    /// 每帧处理鼠标拖拽逻辑，并平滑移动摄像机位置。
    /// </summary>
    void Update()
    {
        // 禁止条件：开关关闭 / 点击UI
        if (!canDrag || EventSystem.current.IsPointerOverGameObject())
        {
            isDragInitialized = false;
            return;
        }

        // 仅在拖拽开启后，用【相机最终位置】初始化
        if (!isDragInitialized)
        {
            _targetCamPos = transform.position;
            _targetCamPos.y = fixedY;
            isDragInitialized = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            DragCamera();
        }

        SmoothMoveCamera();
    }

    /// <summary>
    /// 启用摄像机拖拽功能，通常在过渡动画结束后调用。
    /// </summary>
    public void EnableDrag()
    {
        canDrag = true;
    }

    /// <summary>
    /// 关闭摄像机拖拽功能，常用于切换视角或进入UI界面时。
    /// </summary>
    public void DisableDrag()
    {
        canDrag = false;
    }

    /// <summary>
    /// 处理拖拽时的鼠标偏移，并计算目标摄像机位置。
    /// </summary>
    private void DragCamera()
    {
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 offset = _lastMousePos - currentMousePos;

        if (onlyHorizontalMove) offset.y = 0;

        _targetCamPos += offset * dragSensitivity;
        _targetCamPos.x = Mathf.Clamp(_targetCamPos.x, minX, maxX);
        _targetCamPos.y = fixedY;

        _lastMousePos = currentMousePos;
    }

    /// <summary>
    /// 将摄像机位置平滑插值到目标位置，避免生硬移动。
    /// </summary>
    private void SmoothMoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, _targetCamPos, smoothSpeed);
    }

    /// <summary>
    /// 重置摄像机到默认位置，通常用于场景重置或退出拖拽模式。
    /// </summary>
    public void ResetCamera()
    {
        _targetCamPos = new Vector3(0, fixedY, transform.position.z);
    }
}