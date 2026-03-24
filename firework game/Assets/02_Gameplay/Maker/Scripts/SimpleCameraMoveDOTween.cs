using UnityEngine;
using DG.Tweening;

public class SimpleCameraMoveDOTween : MonoBehaviour
{
    [Header("相机配置")]
    public Camera mainCamera;
    public Vector3 CounterPosition;//柜台位置
    public Vector3 CraftPosition;//制作台位置
    public float moveTime = 0.5f;
    public Ease moveEase = Ease.OutQuad;

    // 引用拖拽脚本
    private CameraDragMove _dragMove;

    void Awake()
    {
        // 获取相机上的拖拽脚本
        _dragMove = mainCamera.GetComponent<CameraDragMove>();
    }

    //镜头移动至柜台
    public void MoveCameraToCounter()
    {
        //镜头一跳转 → 立刻禁用拖拽
        if (_dragMove != null)
        {
            _dragMove.canDrag = false;
        }
        //跳转镜头
        Vector3 safeTarget = CounterPosition;

        mainCamera.transform.DOKill(); // 停止相机当前所有正在运行的DOTween动画
        mainCamera.transform.DOMove(safeTarget, moveTime)// 参数1：目标位置  参数2：移动耗时(秒)
            .SetEase(moveEase) // 设置移动的缓动效果（让镜头移动更顺滑，不是机械匀速）
            .SetUpdate(true); // 强制动画正常运行，不受游戏暂停/时间缩放影响
            
    }

    //返回制作台时 → 开启拖拽
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
            .OnComplete(() => {
                 if (_dragMove != null) _dragMove.EnableDrag();
             });
    }

}