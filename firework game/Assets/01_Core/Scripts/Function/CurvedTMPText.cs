using UnityEngine;
using TMPro;

/// <summary>
/// 文本弯曲特效组件：为 TMP_Text 应用圆弧变形，使其呈现装饰性弯曲效果。
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class CurvedTMPText : MonoBehaviour
{
    [Header("弯曲设置")]
    [Tooltip("圆弧半径：值越小弯曲越厉害")]
    public float radius = 100f;

    [Tooltip("是否启用弧形效果")]
    public bool enableCurve = true;

    private TMP_Text _tmpText;
    private Vector3[] _vertices;

    /// <summary>
    /// 组件启用时订阅 TMP 文本更改事件，并缓存文本组件引用。
    /// </summary>
    void OnEnable()
    {
        _tmpText = GetComponent<TMP_Text>();
        if (_tmpText != null)
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }
    }

    /// <summary>
    /// 禁用时取消订阅文本更改事件，以避免内存泄漏。
    /// </summary>
    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    /// <summary>
    /// 当 TMP 文本内容发生变化时重新应用弯曲效果。
    /// </summary>
    void OnTextChanged(Object obj)
    {
        if ((TMP_Text)obj == _tmpText)
        {
            UpdateCurvedText();
        }
    }

    /// <summary>
    /// 每帧末执行文本更新，保持弧形效果与文本内容一致。
    /// </summary>
    void LateUpdate()
    {
        if (enableCurve && _tmpText != null)
        {
            UpdateCurvedText();
        }
    }

    /// <summary>
    /// 计算并应用文字顶点变形，使文本呈圆弧形排列。
    /// </summary>
    void UpdateCurvedText()
    {
        if (_tmpText == null || !enableCurve) return;

        _tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = _tmpText.textInfo;

        if (textInfo.characterCount == 0) return;

        // 获取网格顶点
        TMP_MeshInfo meshInfo = textInfo.meshInfo[0];
        _vertices = meshInfo.vertices;

        // 计算文本中心（字符总数）
        int characterCount = textInfo.characterCount;
        
        // 每个字符占据的弧度
        float anglePerChar = radius > 0 ? 1f / radius : 0.1f;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            
            if (!charInfo.isVisible)
                continue;

            // 计算该字符应该处于的角度（以中心为基准）
            float charAngle = (i - characterCount / 2f) * anglePerChar;

            // 计算圆弧上的位置
            float posX = Mathf.Sin(charAngle) * radius;
            float posY = -Mathf.Cos(charAngle) * radius + radius; // 上移radius使底部居中

            int vertexIndex = charInfo.vertexIndex;

            // 移动字符的4个顶点
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = _vertices[vertexIndex + j];

                // 计算相对于字符中心的偏移
                float charCenterX = (charInfo.bottomLeft.x + charInfo.bottomRight.x) / 2f;
                float charCenterY = (charInfo.bottomLeft.y + charInfo.topLeft.y) / 2f;

                float relX = v.x - charCenterX;
                float relY = v.y - charCenterY;

                // 旋转顶点，让文字沿着圆弧方向
                float cos = Mathf.Cos(charAngle);
                float sin = Mathf.Sin(charAngle);
                
                float rotatedX = relX * cos - relY * sin;
                float rotatedY = relX * sin + relY * cos;

                // 设置最终位置
                _vertices[vertexIndex + j] = new Vector3(
                    posX + rotatedX,
                    posY + rotatedY,
                    v.z
                );
            }
        }

        // 应用修改
        meshInfo.vertices = _vertices;
        _tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}