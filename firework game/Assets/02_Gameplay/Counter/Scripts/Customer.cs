using UnityEngine;

/// <summary>
/// 客户端脚本：负责接收玩家出的烟花纸筒并触发评分流程。
/// </summary>
public class Customer : MonoBehaviour
{
    public PaperTube paperTube;

    /// <summary>
    /// 客户收到纸筒后调用，执行判定、显示结果并清空纸筒。
    /// </summary>
    public void OnReceiveFirework()
    {
        Debug.Log("客户收到烟花纸筒");

        if (paperTube == null)
        {
            Debug.LogError("未绑定 PaperTube");
            return;
        }

        // 1. 获取纸筒上的组件列表
        var components = paperTube.StackedFireworkComponents;
        // 2. 获取当前客人
        var guest = GuestManager.Instance.currentGuest;
        // 3. 执行评分逻辑
        JudgeResult result = JudgeManager.Instance.JudgeFirework(components, guest);
        // 4. 显示评分结果、奖励和评价
        GuestManager.Instance.ShowJudgeResult(result);
        // 5. 清空纸筒
        paperTube.ClearStack();
    }
}