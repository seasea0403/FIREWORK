using UnityEngine;
using System.Collections.Generic;

public class ContentSelector : MonoBehaviour
{
    // 单例实例
    public static ContentSelector Instance;

    // 私有变量（加m_前缀）
    private ContentItem m_CurrentSelectedItem;
    private List<FireworkComponent> m_SelectedFireworkComponents = new List<FireworkComponent>();

    // 公共属性
    public ContentItem CurrentSelectedItem
    {
        get { return m_CurrentSelectedItem; }
        set { m_CurrentSelectedItem = value; }
    }

    public List<FireworkComponent> SelectedFireworkComponents => new List<FireworkComponent>(m_SelectedFireworkComponents);

    void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 物品点击回调
    /// </summary>
    public void OnItemClicked(ContentItem clickedItem)
    {
        // 取消当前选中
        if (m_CurrentSelectedItem == clickedItem)
        {
            m_CurrentSelectedItem.Deselect();
            // 移除映射的组件
            FireworkComponent comp = FireworkComponentMapper.MapToFireworkComponent(clickedItem);
            if (m_SelectedFireworkComponents.Contains(comp))
            {
                m_SelectedFireworkComponents.Remove(comp);
            }
            m_CurrentSelectedItem = null;
        }
        // 选中新物品
        else
        {
            if (m_CurrentSelectedItem != null)
            {
                m_CurrentSelectedItem.Deselect();
                // 移除旧组件
                FireworkComponent oldComp = FireworkComponentMapper.MapToFireworkComponent(m_CurrentSelectedItem);
                if (m_SelectedFireworkComponents.Contains(oldComp))
                {
                    m_SelectedFireworkComponents.Remove(oldComp);
                }
            }
            m_CurrentSelectedItem = clickedItem;
            m_CurrentSelectedItem.Select();
            // 添加新组件
            FireworkComponent newComp = FireworkComponentMapper.MapToFireworkComponent(clickedItem);
            if (!m_SelectedFireworkComponents.Contains(newComp))
            {
                m_SelectedFireworkComponents.Add(newComp);
            }
        }

        Debug.Log("当前选中组件：" + string.Join(",", m_SelectedFireworkComponents));
    }

    /// <summary>
    /// 清空所有选择
    /// </summary>
    public void ClearAllSelection()
    {
        if (m_CurrentSelectedItem != null)
        {
            m_CurrentSelectedItem.Deselect();
        }
        m_CurrentSelectedItem = null;
        m_SelectedFireworkComponents.Clear();
    }
}