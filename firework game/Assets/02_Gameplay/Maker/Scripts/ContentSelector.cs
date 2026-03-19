using UnityEngine;

public class ContentSelector : MonoBehaviour
{
    // 全局唯一实例，让其他脚本能访问
    public static ContentSelector Instance;

    // 关键修改：把 private 改成 public，或新增公共属性（推荐后者）
    private ContentItem _currentSelectedItem;
    // 新增公共属性，外部脚本通过这个属性访问
    public ContentItem currentSelectedItem
    {
        get { return _currentSelectedItem; }
        set { _currentSelectedItem = value; }
    }

    void Awake()
    {
        // 确保场景里只有一个ContentSelector
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换丢失
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 外部调用：物品被点击时执行
    public void OnItemClicked(ContentItem clickedItem)
    {
        // 情况1：点击的是已选中的物品 → 取消选中
        if (_currentSelectedItem == clickedItem)
        {
            _currentSelectedItem.Deselect();
            _currentSelectedItem = null;
        }
        // 情况2：点击新物品 → 取消旧的，选中新的
        else
        {
            if (_currentSelectedItem != null)
            {
                _currentSelectedItem.Deselect();
            }
            _currentSelectedItem = clickedItem;
            _currentSelectedItem.Select();
        }
    }
}