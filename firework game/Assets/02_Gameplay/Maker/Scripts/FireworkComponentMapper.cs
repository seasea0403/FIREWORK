using UnityEngine;

/// <summary>
/// ContentItem与FireworkComponent的映射工具（适配最终版GameEnums）
/// </summary>
public static class FireworkComponentMapper
{
    /// <summary>
    /// 直接返回ContentItem绑定的FireworkComponent（无需映射，因为已直接绑定）
    /// </summary>
    public static FireworkComponent MapToFireworkComponent(ContentItem item)
    {
        if (item == null) return FireworkComponent.NormalPowder; // 兜底

        // 无需复杂映射，直接返回绑定的组件类型（因为ContentItem已直接绑定GameEnums的FireworkComponent）
        return item.fireworkComponent;
    }

    /// <summary>
    /// 检查物品是否已解锁
    /// </summary>
    public static bool IsItemUnlocked(ContentItem item)
    {
        if (item == null || GameManager.Instance == null) return true;

        FireworkComponent comp = MapToFireworkComponent(item);
        return GameManager.Instance.gameState.IsComponentUnlocked(comp);
    }
}