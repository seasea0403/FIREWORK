using UnityEngine;

public class CameraDragMove : MonoBehaviour
{
    [Header("=== 拖拽配置 ===")]
    [Tooltip("拖拽灵敏度（值越大移动越快）")]
    public float dragSensitivity = 1f;
    [Tooltip("是否仅水平移动（制作台左右移动，必开）")]
    public bool onlyHorizontalMove = true;

    [Header("=== 移动边界限制 ===")]
    [Tooltip("相机X轴最小位置（制作台最左侧）")]
    public float minX = -5f;
    [Tooltip("相机X轴最大位置（制作台最右侧）")]
    public float maxX = 10f;
    [Tooltip("相机Y轴固定位置（防止上下移动）")]
    public float fixedY = 0f;

    [Header("=== 平滑配置 ===")]
    [Tooltip("移动平滑度（0=无平滑，1=最丝滑）")]
    public float smoothSpeed = 0.125f;

    // 私有变量
    private Vector3 _lastMousePos; // 上一帧鼠标位置
    private Vector3 _targetCamPos; // 相机目标位置

    void Start()
    {
        // 初始化目标位置为相机当前位置（锁定Y轴）
        _targetCamPos = transform.position;
        _targetCamPos.y = fixedY;
    }

    void Update()
    {
        // 仅在按住鼠标左键时响应拖拽
        if (Input.GetMouseButtonDown(0))
        {
            // 记录按下瞬间的鼠标位置（转世界坐标）
            _lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            // 计算拖拽偏移
            DragCamera();
        }

        // 平滑移动相机到目标位置（核心：避免瞬移）
        SmoothMoveCamera();
    }

    // 计算拖拽偏移，更新相机目标位置
    private void DragCamera()
    {
        // 获取当前鼠标世界坐标
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 计算鼠标移动偏移（反向：鼠标左拖=相机右移，符合直觉）
        Vector3 offset = _lastMousePos - currentMousePos;

        // 仅水平移动（锁定Y轴）
        if (onlyHorizontalMove)
        {
            offset.y = 0;
        }

        // 更新相机目标位置
        _targetCamPos += offset * dragSensitivity;
        // 限制X轴在制作台范围内（防止拖出界）
        _targetCamPos.x = Mathf.Clamp(_targetCamPos.x, minX, maxX);
        // 锁定Y轴位置
        _targetCamPos.y = fixedY;

        // 更新上一帧鼠标位置
        _lastMousePos = currentMousePos;
    }

    // 平滑移动相机（核心：丝滑效果）
    private void SmoothMoveCamera()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            _targetCamPos,
            smoothSpeed
        );
    }

    // 可选：重置相机到初始位置
    public void ResetCamera()
    {
        _targetCamPos = new Vector3(0, fixedY, transform.position.z);
    }
}