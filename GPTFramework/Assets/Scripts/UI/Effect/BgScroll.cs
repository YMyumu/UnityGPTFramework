using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BgScroll : MonoBehaviour
{
    public bool scrolling;
    public float speed_x;
    public float speed_y;

    RawImage rimg;
    Rect rect;
    float x, y;

    private void Awake()
    {
        rimg = GetComponent<RawImage>();
        rect = new Rect();
    }
    
    private void Update()
    {
        if (scrolling)
        {
            x = rimg.uvRect.x + Time.deltaTime * speed_x;
            y = rimg.uvRect.y + Time.deltaTime * speed_y;

            if (x > 1) x = x - 1;
            if (y > 1) y = y - 1;

            rect.Set(x, y, rimg.uvRect.width, rimg.uvRect.height);
            rimg.uvRect = rect;
        }
    }
}
