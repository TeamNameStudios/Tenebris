
    using UnityEngine;

public class OverlapChecker : MonoBehaviour
{
    public Rect rect1;
    public Rect rect2;

    private Texture2D texture1;
    private Texture2D texture2;

    private void Awake()
    {
        // Load your textures here if needed
        // Example: texture1 = Resources.Load<Texture2D>("Texture1");
        // Example: texture2 = Resources.Load<Texture2D>("Texture2");
    }

    private void Update()
    {
        if (CheckOverlap(rect1, rect2))
        {
            Debug.Log("Rectangles overlap!");

            // Perform actions when overlap occurs
        }
    }

    private bool CheckOverlap(Rect rectA, Rect rectB)
    {
        // Check if the rectangles intersect
        if (rectA.xMax < rectB.xMin || rectB.xMax < rectA.xMin ||
            rectA.yMax < rectB.yMin || rectB.yMax < rectA.yMin)
        {
            return false;
        }

        // Check if using textures
        if (texture1 != null && texture2 != null)
        {
            // Convert rect coordinates to texture space
            Rect textureRectA = new Rect(rectA.x / texture1.width, rectA.y / texture1.height, rectA.width / texture1.width, rectA.height / texture1.height);
            Rect textureRectB = new Rect(rectB.x / texture2.width, rectB.y / texture2.height, rectB.width / texture2.width, rectB.height / texture2.height);

            // Get the intersection between the texture rects
            Rect intersection = RectIntersection(textureRectA, textureRectB);

            // Check if the intersection area is non-zero
            return intersection.width > 0 && intersection.height > 0;
        }

        // Rectangles overlap
        return true;
    }

    private Rect RectIntersection(Rect rectA, Rect rectB)
    {
        float xMin = Mathf.Max(rectA.xMin, rectB.xMin);
        float xMax = Mathf.Min(rectA.xMax, rectB.xMax);
        float yMin = Mathf.Max(rectA.yMin, rectB.yMin);
        float yMax = Mathf.Min(rectA.yMax, rectB.yMax);

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}
