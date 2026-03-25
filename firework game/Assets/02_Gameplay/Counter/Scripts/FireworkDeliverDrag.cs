using UnityEngine;

public class FireworkDeliverDrag : MonoBehaviour
{
    private Vector3 _originPos;
    private Camera _mainCam;
    private bool _isDragging = false;

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

    void OnMouseDown()
    {
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        Vector3 mouseWorldPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;
    }

    void OnMouseUp()
    {
        _isDragging = false;

        // --------------------------
        // 🔥 关键修复：忽略自己，只检测别人
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
                Debug.Log("✅ 成功交付给客户！");

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