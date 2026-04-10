using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 道具选中管理器，负责跟踪当前选中项和选中组件列表。
/// </summary>
public class ContentSelector : MonoBehaviour
{
    // 单例实例
    public static ContentSelector Instance;

    // 私有成员变量
    private ContentItem m_CurrentSelectedItem;
    private List<FireworkComponent> m_SelectedFireworkComponents = new List<FireworkComponent>();

    public ContentItem CurrentSelectedItem
    {
        get { return m_CurrentSelectedItem; }
        set { m_CurrentSelectedItem = value; }
    }

    public List<FireworkComponent> SelectedFireworkComponents => new List<FireworkComponent>(m_SelectedFireworkComponents);

    /// <summary>
    /// 初始化 ContentSelector 单例，确保场景中只保留一个选中管理器实例。
    /// </summary>
    void Awake()
    {
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
    /// 处理道具被点击后更新选中状态。
    /// </summary>
    public void OnItemClicked(ContentItem clickedItem)
    {
        if (m_CurrentSelectedItem == clickedItem)
        {
            m_CurrentSelectedItem.Deselect();
            FireworkComponent comp = FireworkComponentMapper.MapToFireworkComponent(clickedItem);
            if (m_SelectedFireworkComponents.Contains(comp))
            {
                m_SelectedFireworkComponents.Remove(comp);
            }
            m_CurrentSelectedItem = null;
        }
        else
        {
            if (m_CurrentSelectedItem != null)
            {
                m_CurrentSelectedItem.Deselect();
                FireworkComponent oldComp = FireworkComponentMapper.MapToFireworkComponent(m_CurrentSelectedItem);
                if (m_SelectedFireworkComponents.Contains(oldComp))
                {
                    m_SelectedFireworkComponents.Remove(oldComp);
                }
            }
            m_CurrentSelectedItem = clickedItem;
            m_CurrentSelectedItem.Select();
            FireworkComponent newComp = FireworkComponentMapper.MapToFireworkComponent(clickedItem);
            if (!m_SelectedFireworkComponents.Contains(newComp))
            {
                m_SelectedFireworkComponents.Add(newComp);
            }
        }

        Debug.Log("当前选中组件：" + string.Join(",", m_SelectedFireworkComponents));
    }

    /// <summary>
    /// 清空当前选中项和选中组件列表。
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