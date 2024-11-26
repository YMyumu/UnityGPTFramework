using UnityEngine;
using UnityEngine.UI;

// Graphic(文本，图片)翻转控件
[RequireComponent(typeof(RectTransform), typeof(Graphic)), DisallowMultipleComponent]
[AddComponentMenu("UI/Ext/GraphicFlip")]
public class GraphicFlip : BaseMeshEffect
{
    [SerializeField] bool m_Horizontal = false;
    [SerializeField] bool m_Veritical = false;

    public bool horizontal
    {
        get { return m_Horizontal; }
        set { m_Horizontal = value; }
    }

    public bool vertical
    {
        get { return m_Veritical; }
        set { m_Veritical = value; }
    }

    RectTransform m_RectTransform;

    public RectTransform rectTransform
    {
        get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        RectTransform rt = rectTransform;

        for (int i = 0; i < vh.currentVertCount; ++i)
        {
            UIVertex uiVertex = new UIVertex();
            vh.PopulateUIVertex(ref uiVertex, i);

            uiVertex.position = new Vector3(
                (m_Horizontal ? (uiVertex.position.x + (rt.rect.center.x - uiVertex.position.x) * 2) : uiVertex.position.x),
                (m_Veritical ? (uiVertex.position.y + (rt.rect.center.y - uiVertex.position.y) * 2) : uiVertex.position.y),
                uiVertex.position.z
            );

            vh.SetUIVertex(uiVertex, i);
        }
    }
}
