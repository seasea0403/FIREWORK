using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class move : MonoBehaviour
{
    [Header("=== 相机移动配置 ===")]
    [Tooltip("要控制的主相机（默认取MainCamera）")]
    public Camera mainCamera;
    [Tooltip("相机跳转的目标位置（世界坐标）")]
    public Vector3 cameraTargetPos;
    [Tooltip("相机跳转后的旋转（2D场景设为(0,0,0)）")]
    public Vector3 cameraTargetRot = Vector3.zero;
    [Tooltip("移动时长（秒），0=瞬间跳转")]
    public float moveDuration = 0.5f;
    [Tooltip("缓动曲线（平滑移动效果）")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("=== 可选：返回初始位置 ===")]
    [Tooltip("点击自身是否返回初始位置（相机已移动时）")]
    public bool clickToReturn = true;
    [Tooltip("返回时长（秒）")]
    public float returnDuration = 0.5f;

    private Vector3 _originalCamPos;    // 相机初始位置
    private Quaternion _originalCamRot; // 相机初始旋转
    private bool _isCameraMoved = false;// 相机是否已跳转
    private Collider2D _collider;       // 自身Collider
    private SpriteRenderer _spriteRenderer; // 自身Sprite

    void Awake()
    {
        // 自动获取组件
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        // 自动配置Collider（确保可点击）
        if (_collider is Collider2D coll2D)
        {
            coll2D.isTrigger = true; // 2D Collider设为触发（避免物理碰撞）
        }

        // 自动获取主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 记录相机初始状态（仅运行时执行）
        if (mainCamera != null)
        {
            _originalCamPos = mainCamera.transform.position;
            _originalCamRot = mainCamera.transform.rotation;
        }
    }

    /// <summary>
    /// 2D场景点击检测（挂载在自身，通过OnMouseDown触发）
    /// </summary>
    void OnMouseDown()
    {
        // 运行时才执行，且校验相机不为空
        if (mainCamera == null)
        {
            Debug.LogError("主相机未配置！请在Inspector中指定mainCamera");
            return;
        }

        if (_isCameraMoved && clickToReturn)
        {
            // 相机已移动，点击返回初始位置
            MoveCamera(_originalCamPos, _originalCamRot, returnDuration);
            _isCameraMoved = false;
        }
        else
        {
            // 相机未移动，点击跳转到目标位置
            MoveCamera(cameraTargetPos, Quaternion.Euler(cameraTargetRot), moveDuration);
            _isCameraMoved = true;
        }
    }

    /// <summary>
    /// 平滑移动相机到指定位置/旋转
    /// </summary>
    private void MoveCamera(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        if (mainCamera == null) return;

        // 停止当前移动（避免重复触发）
        StopAllCoroutines();
        StartCoroutine(SmoothMoveCoroutine(targetPos, targetRot, duration));
    }

    /// <summary>
    /// 相机平滑移动协程
    /// </summary>
    private System.Collections.IEnumerator SmoothMoveCoroutine(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        // 时长为0则瞬间跳转
        if (duration <= 0)
        {
            mainCamera.transform.position = targetPos;
            mainCamera.transform.rotation = targetRot;
            yield break;
        }

        // 平滑插值移动
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveT = moveCurve.Evaluate(t); // 缓动曲线优化

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, curveT);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetRot, curveT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置精准
        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }

    // 外部调用：手动触发相机跳转（可选）
    public void TriggerCameraMove()
    {
        if (mainCamera == null) return;
        MoveCamera(cameraTargetPos, Quaternion.Euler(cameraTargetRot), moveDuration);
        _isCameraMoved = true;
    }

    // 外部调用：手动返回初始位置（可选）
    public void TriggerCameraReturn()
    {
        if (mainCamera == null) return;
        MoveCamera(_originalCamPos, _originalCamRot, returnDuration);
        _isCameraMoved = false;
    }

    // Gizmos可视化目标位置（修复空引用问题）
    void OnDrawGizmosSelected()
    {
        // ========== 核心修复：先校验mainCamera是否为空 ==========
        if (mainCamera == null)
        {
            // 编辑模式下自动查找主相机（避免空引用）
            mainCamera = Camera.main;
            // 如果仍为空，直接返回，不绘制Gizmos
            if (mainCamera == null) return;
        }

        // 绘制目标位置标记（红色球体）
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cameraTargetPos, 0.2f);
        // 绘制相机移动路径（仅当相机存在时）
        Gizmos.DrawLine(mainCamera.transform.position, cameraTargetPos);
    }
}