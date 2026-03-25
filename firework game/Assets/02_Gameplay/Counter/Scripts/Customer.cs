using UnityEngine;

// 客户逻辑：收到烟花后做什么
public class Customer : MonoBehaviour
{
    public PaperTube paperTube;

    // 收到烟花后触发
    public void OnReceiveFirework()
    {
        Debug.Log("客户收到烟花啦！");

        if (paperTube == null)
        {
            Debug.LogError("未绑定PaperTube！");
            return;
        }

        // 1. 获取纸筒里的组件
        var components = paperTube.StackedFireworkComponents;
        // 2. 获取当前客人需求
        var guest = GuestManager.Instance.currentGuest;
        // 3. 执行判定
        JudgeResult result = JudgeManager.Instance.JudgeFirework(components, guest);
        // 4. 显示判定结果（表情+台词+给钱）
        GuestManager.Instance.ShowJudgeResult(result);
        // 5. 清空纸筒
        paperTube.ClearStack();
    }

    // 你可以自己加特效、动画等
}