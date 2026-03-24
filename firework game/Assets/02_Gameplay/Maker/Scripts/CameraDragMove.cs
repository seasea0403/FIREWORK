using UnityEngine;
using UnityEngine.EventSystems;

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

    // 🔥 核心：动画完成后，调用这个方法开启拖拽
    public void EnableDrag()
    {
        canDrag = true;
    }

    // 关闭拖拽（跳柜台时调用）
    public void DisableDrag()
    {
        canDrag = false;
    }

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

    private void SmoothMoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, _targetCamPos, smoothSpeed);
    }

    public void ResetCamera()
    {
        _targetCamPos = new Vector3(0, fixedY, transform.position.z);
    }
}