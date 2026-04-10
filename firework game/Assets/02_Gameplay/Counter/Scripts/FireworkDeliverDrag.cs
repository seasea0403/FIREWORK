using UnityEngine;

/// <summary>
/// 拖拽物体交付逻辑：玩家拖动烟花并将其投递给客户。
/// </summary>
public class FireworkDeliverDrag : MonoBehaviour
{
    private Vector3 _originPos;
    private Camera _mainCam;
    private bool _isDragging = false;

    /// <summary>
    /// 初始化相机引用、记录原始位置，并确保对象具备刚体组件。
    /// </summary>
    void Awake()
    {
        _mainCam = Camera.main;
        _originPos = transform.position;

        // 自动加刚体
        if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
    }

    /// <summary>
    /// 鼠标按下时开始拖拽对象。
    /// </summary>
    void OnMouseDown()
    {
        _isDragging = true;
    }

    /// <summary>
    /// 拖拽过程中将对象位置跟随鼠标世界坐标。
    /// </summary>
    void OnMouseDrag()
    {
        if (!_isDragging) return;
        Vector3 mouseWorldPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;
    }

    /// <summary>
    /// 松开鼠标时检测是否成功交付到客户，否则返回原位。
    /// </summary>
    void OnMouseUp()
    {
        _isDragging = false;

        // --------------------------
        //忽略自己，只检测别人
        // --------------------------
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (var hit in hits)
        {
            // 跳过自己！只找客户
            if (hit.gameObject == gameObject)
                continue;

            Debug.Log("检测到：" + hit.gameObject.name + " 标签：" + hit.tag);

            if (hit.CompareTag("Customer"))
            {
                Debug.Log("成功交付给客户！");

                Customer customer = hit.GetComponent<Customer>();
                if (customer != null)
                    customer.OnReceiveFirework();

                Destroy(gameObject);
                return;
            }
        }

        // 没找到就回去
        transform.position = _originPos;
    }
}