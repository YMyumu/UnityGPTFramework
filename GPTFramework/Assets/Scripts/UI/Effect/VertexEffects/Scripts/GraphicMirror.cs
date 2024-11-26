using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

// Graphic(文本，图片)镜像生成组件
// 作者：安建宇
[RequireComponent(typeof(Graphic)), AddComponentMenu("UI/Ext/GraphicMirror")]
public class GraphicMirror : BaseMeshEffect
{
    public enum MirrorType
    {
        /// <summary>
        /// 水平
        /// </summary>
        Horizontal,

        /// <summary>
        /// 垂直
        /// </summary>
        Vertical,

        /// <summary>
        /// 四分之一
        /// 相当于水平，然后再垂直
        /// </summary>
        Quarter,
    }

    /// <summary>
    /// 镜像类型
    /// </summary>
    [SerializeField]
    MirrorType m_MirrorType = MirrorType.Horizontal;

    [SerializeField]
    bool isRight = true;    // 是否向右翻转
    [SerializeField]
    bool isDown = false;    // 是否向下翻转
    [SerializeField]
    [Range(-10f, 10f)]
    float offset = 0;   // 像素偏移，解决镜像缝隙

    public MirrorType mirrorType
    {
        get { return m_MirrorType; }
        set
        {
            if (m_MirrorType != value)
            {
                m_MirrorType = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }
    }

    RectTransform m_RectTransform;

    public RectTransform rectTransform
    {
        get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
    }

    float offsetX
    {
        get
        {
            return isRight ? -offset / 2 : (offset / 2);
        }
    }

    float offsetY
    {
        get
        {
            return isDown ? offset / 2 : -(offset / 2);
        }
    }

    /// <summary>
    /// 设置原始尺寸
    /// </summary>
    public void SetNativeSize()
    {
        if (graphic != null && graphic is Image)
        {
            Sprite overrideSprite = (graphic as Image).overrideSprite;

            if (overrideSprite != null)
            {
                float w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
                float h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;
                rectTransform.anchorMax = rectTransform.anchorMin;

                switch (m_MirrorType)
                {
                    case MirrorType.Horizontal:
                        rectTransform.sizeDelta = new Vector2(w * 2 - offset, h);
                        break;
                    case MirrorType.Vertical:
                        rectTransform.sizeDelta = new Vector2(w, h * 2 - offset);
                        break;
                    case MirrorType.Quarter:
                        rectTransform.sizeDelta = new Vector2(w * 2 - offset, h * 2 - offset);
                        break;
                }

                graphic.SetVerticesDirty();
            }
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        var output = new List<UIVertex>();
        vh.GetUIVertexStream(output);

        int count = output.Count;

        if (graphic is Image)
        {
            Image.Type type = (graphic as Image).type;

            switch (type)
            {
                case Image.Type.Simple:
                    DrawSimple(output, count);
                    break;
                case Image.Type.Sliced:
                    DrawSliced(output, count);
                    break;
                case Image.Type.Tiled:
                case Image.Type.Filled:
                    break;
            }
        }
        else
        {
            DrawSimple(output, count);
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(output);
    }

    /// <summary>
    /// 绘制Simple版
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    protected void DrawSimple(List<UIVertex> output, int count)
    {
        Rect rect = graphic.GetPixelAdjustedRect();

        SimpleScale(rect, output, count);

        switch (m_MirrorType)
        {
            case MirrorType.Horizontal:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, true);
                break;
            case MirrorType.Vertical:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, false);
                break;
            case MirrorType.Quarter:
                ExtendCapacity(output, count * 3);
                MirrorVerts(rect, output, count, true);
                MirrorVerts(rect, output, count * 2, false);
                break;
        }
    }

    /// <summary>
    /// 绘制Sliced版
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    protected void DrawSliced(List<UIVertex> output, int count)
    {
        if (!(graphic as Image).hasBorder)
        {
            DrawSimple(output, count);

            return;
        }

        Rect rect = graphic.GetPixelAdjustedRect();

        SlicedScale(rect, output, count);

        count = SliceExcludeVerts(output, count);

        switch (m_MirrorType)
        {
            case MirrorType.Horizontal:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, true);
                break;
            case MirrorType.Vertical:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, false);
                break;
            case MirrorType.Quarter:
                ExtendCapacity(output, count * 3);
                MirrorVerts(rect, output, count, true);
                MirrorVerts(rect, output, count * 2, false);
                break;
        }
    }

    /// <summary>
    /// 扩展容量
    /// </summary>
    /// <param name="verts"></param>
    /// <param name="addCount"></param>
    protected void ExtendCapacity(List<UIVertex> verts, int addCount)
    {
        var neededCapacity = verts.Count + addCount;
        if (verts.Capacity < neededCapacity)
        {
            verts.Capacity = neededCapacity;
        }
    }

    /// <summary>
    /// Simple缩放位移顶点（减半）
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    protected void SimpleScale(Rect rect, List<UIVertex> verts, int count)
    {
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];

            Vector3 position = vertex.position;

            if (m_MirrorType == MirrorType.Horizontal || m_MirrorType == MirrorType.Quarter)
            {
                if (isRight)
                    position.x = (position.x + rect.x) * 0.5f - offsetX;
                else
                    position.x = (position.x - rect.x) * 0.5f - offsetX;
            }

            if (m_MirrorType == MirrorType.Vertical || m_MirrorType == MirrorType.Quarter)
            {
                if (isDown)
                    position.y = (position.y - rect.y) * 0.5f - offsetY;
                else
                    position.y = (position.y + rect.y) * 0.5f - offsetY;
            }

            vertex.position = position;

            verts[i] = vertex;
        }
    }

    /// <summary>
    /// Sliced缩放位移顶点（减半）
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    protected void SlicedScale(Rect rect, List<UIVertex> verts, int count)
    {
        Vector4 border = GetAdjustedBorders(rect);

        float halfWidth = -rect.x;

        float halfHeight = -rect.y;

        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];

            Vector3 position = vertex.position;

            if (m_MirrorType == MirrorType.Horizontal || m_MirrorType == MirrorType.Quarter)
            {
                if (isRight)
                {
                    // 不够九宫格border大小，顶点缩放到中心位置
                    if (halfWidth < border.x && position.x >= rect.center.x)
                    {
                        position.x = rect.center.x - offsetX;
                    }
                    else if (halfWidth < border.z && position.x <= rect.center.x)
                    {
                        position.x = rect.center.x - offsetX;
                    }
                    else if (position.x >= (halfWidth - border.z))
                    {
                        position.x = (position.x + rect.x) - offsetX;
                    }
                    else if (position.x >= border.x)
                    {
                        position.x = (position.x + rect.x) * 0.5f - offsetX;
                    }
                }
                else
                {
                    // 不够九宫格border大小，顶点缩放到中心位置
                    if (halfWidth < border.x && position.x >= rect.center.x)
                    {
                        position.x = rect.center.x - offsetX;
                    }
                    else if (halfWidth < border.z && position.x <= rect.center.x)
                    {
                        position.x = rect.center.x - offsetX;
                    }
                    else if (position.x <= -(halfWidth - border.x))
                    {
                        position.x = (position.x - rect.x) - offsetX;
                    }
                    else if (position.x <= -border.z)
                    {
                        position.x = (position.x - rect.x) * 0.5f - offsetX;
                    }
                }
            }

            if (m_MirrorType == MirrorType.Vertical || m_MirrorType == MirrorType.Quarter)
            {
                if (isDown)
                {
                    // 不够九宫格border大小，顶点缩放到中心位置
                    if (halfHeight < border.w && position.y <= rect.center.y)
                    {
                        position.y = rect.center.y - offsetY;
                    }
                    else if (halfHeight < border.y && position.y >= rect.center.y)
                    {
                        position.y = rect.center.y - offsetY;
                    }
                    else if (position.y <= -(halfHeight - border.y))
                    {
                        position.y = (position.y - rect.y) - offsetY;
                    }
                    else if (position.y <= -border.w)
                    {
                        position.y = (position.y - rect.y) * 0.5f - offsetY;
                    }
                }
                else
                {
                    // 不够九宫格border大小，顶点缩放到中心位置
                    if (halfHeight < border.w && position.y <= rect.center.y)
                    {
                        position.y = rect.center.y - offsetY;
                    }
                    else if (halfHeight < border.y && position.y >= rect.center.y)
                    {
                        position.y = rect.center.y - offsetY;
                    }
                    else if (position.y >= (halfHeight - border.w))
                    {
                        position.y = (position.y + rect.y) - offsetY;
                    }
                    else if (position.y >= border.y)
                    {
                        position.y = (position.y + rect.y) * 0.5f - offsetY;
                    }
                }
            }

            vertex.position = position;

            verts[i] = vertex;
        }
    }

    /// <summary>
    /// 镜像顶点
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    /// <param name="isHorizontal"></param>
    protected void MirrorVerts(Rect rect, List<UIVertex> verts, int count, bool isHorizontal = true)
    {
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];

            Vector3 position = vertex.position;

            if (isHorizontal)
            {
                position.x = rect.center.x * 2 - position.x + offsetX;
            }
            else
            {
                position.y = rect.center.y * 2 - position.y + offsetY;
            }

            vertex.position = position;

            verts.Add(vertex);
        }
    }

    /// <summary>
    /// 清理掉不能成三角面的顶点
    /// </summary>
    /// <param name="verts"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    protected int SliceExcludeVerts(List<UIVertex> verts, int count)
    {
        int realCount = count;

        int i = 0;

        while (i < realCount)
        {
            UIVertex v1 = verts[i];
            UIVertex v2 = verts[i + 1];
            UIVertex v3 = verts[i + 2];

            if (v1.position == v2.position || v2.position == v3.position || v3.position == v1.position)
            {
                verts[i] = verts[realCount - 3];
                verts[i + 1] = verts[realCount - 2];
                verts[i + 2] = verts[realCount - 1];

                realCount -= 3;
                continue;
            }

            i += 3;
        }

        if (realCount < count)
        {
            verts.RemoveRange(realCount, count - realCount);
        }

        return realCount;
    }

    /// <summary>
    /// 返回矫正过的范围
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    protected Vector4 GetAdjustedBorders(Rect rect)
    {
        Sprite overrideSprite = (graphic as Image).overrideSprite;

        Vector4 border = overrideSprite.border;

        border = border / (graphic as Image).pixelsPerUnit;

        for (int axis = 0; axis <= 1; axis++)
        {
            float combinedBorders = border[axis] + border[axis + 2];
            if (rect.size[axis] < combinedBorders && combinedBorders != 0)
            {
                float borderScaleRatio = rect.size[axis] / combinedBorders;
                border[axis] *= borderScaleRatio;
                border[axis + 2] *= borderScaleRatio;
            }
        }

        return border;
    }
}
