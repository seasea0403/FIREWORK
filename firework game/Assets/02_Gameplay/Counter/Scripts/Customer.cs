using UnityEngine;

// 客户逻辑：收到烟花后做什么
public class Customer : MonoBehaviour
{
    // 收到烟花后触发
    public void OnReceiveFirework()
    {
        Debug.Log("客户收到烟花啦！");

        // 你可以在这里写任意逻辑：
        // 1. 播放开心动画
        // 2. 显示对话气泡
        // 3. 播放音效
        // 4. 生成金币
        // 5. 客户消失/走掉
        // 6. 天数+1、解锁新材料等

        // 示例：让客户飘出爱心
        // SpawnHeartEffect();
    }

    // 你可以自己加特效、动画等
}