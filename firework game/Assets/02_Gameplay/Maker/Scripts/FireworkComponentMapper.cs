using UnityEngine;

/// <summary>
/// 负责将 ContentItem 映射到 FireworkComponent 的辅助类。
/// </summary>
public static class FireworkComponentMapper
{
    /// <summary>
    /// 从 ContentItem 获取对应的 FireworkComponent 枚举。
    /// </summary>
    public static FireworkComponent MapToFireworkComponent(ContentItem item)
    {
        if (item == null) return FireworkComponent.NormalPowder;
        return item.fireworkComponent;
    }

    /// <summary>
    /// 检查该道具对应的组件是否已解锁。
    /// </summary>
    public static bool IsItemUnlocked(ContentItem item)
    {
        // 安全检查：如果管理器未初始化，默认不解锁（防止时序问题导致全部可用）
        if (item == null) return false;
        if (GameManager.Instance == null || GameManager.Instance.gameState == null) return false;

        FireworkComponent comp = MapToFireworkComponent(item);
        return GameManager.Instance.gameState.IsComponentUnlocked(comp);
    }
}