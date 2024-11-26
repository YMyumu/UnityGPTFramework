using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/Gradient Color"), RequireComponent(typeof(Graphic))]
    public class GradientColor : BaseMeshEffect
    {
        public enum DIRECTION
        {
            Vertical,
            Horizontal,
            Both,
        }

        public DIRECTION direction = DIRECTION.Vertical;
        public Color colorTop = Color.white;
        public Color colorBottom = Color.black;
        public Color colorLeft = Color.white;
        public Color colorRight = Color.white;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (IsActive() == false)
            {
                return;
            }

            var vList = UIListPool<UIVertex>.Get();
            vh.GetUIVertexStream(vList);

            ModifyVertices(vList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vList);

            UIListPool<UIVertex>.Release(vList);
        }

        public void ModifyVertices(List<UIVertex> vList)
        {
            if (IsActive() == false || vList == null || vList.Count == 0)
            {
                return;
            }

            float topX = 0f, topY = 0f, bottomX = 0f, bottomY = 0f;

            for (int i = 0; i < vList.Count; i++)
            {
                var vertex = vList[i];
                topX = Mathf.Max(topX, vertex.position.x);
                topY = Mathf.Max(topY, vertex.position.y);
                bottomX = Mathf.Min(bottomX, vertex.position.x);
                bottomY = Mathf.Min(bottomY, vertex.position.y);
            }

            float width = topX - bottomX;
            float height = topY - bottomY;

            for (int i = 0; i < vList.Count; i++)
            {
                var tempVertex = vList[i];
                byte orgAlpha = tempVertex.color.a;
                Color colorOrg = Color.white; // 保证算法和原算法一样
                Color colorV = Color.Lerp(colorBottom, colorTop, (tempVertex.position.y - bottomY) / height);
                Color colorH = Color.Lerp(colorLeft, colorRight, (tempVertex.position.x - bottomX) / width);
                switch (direction)
                {
                    case DIRECTION.Both:
                        tempVertex.color = colorOrg * colorV * colorH;
                        break;
                    case DIRECTION.Vertical:
                        tempVertex.color = colorOrg * colorV;
                        break;
                    case DIRECTION.Horizontal:
                        tempVertex.color = colorOrg * colorH;
                        break;
                }
                tempVertex.color.a = orgAlpha;
                vList[i] = tempVertex;
            }
        }

        /// <summary>
        /// Refresh Gradient Color on playing.
        /// </summary>
        public void Refresh()
        {
            if (graphic != null)
            {
                graphic.SetVerticesDirty();
            }
        }
    }
}
