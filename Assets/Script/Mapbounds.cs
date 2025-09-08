using UnityEngine;

public class MapBounds : MonoBehaviour
{
    public static MapBounds Instance;

    private float minX, maxX, minY, maxY;

    public float MinY => minY;   // thêm property public để Egg.cs gọi
    public float MaxY => maxY;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CalculateBounds();
    }

    void CalculateBounds()
    {
        Camera cam = Camera.main;
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;
        Vector3 camPos = cam.transform.position;

        minX = camPos.x - horzExtent;
        maxX = camPos.x + horzExtent;
        minY = camPos.y - vertExtent;
        maxY = camPos.y + vertExtent;
    }

    public Vector2 ClampPosition(Vector2 position)
    {
        float x = Mathf.Clamp(position.x, minX, maxX);
        float y = Mathf.Clamp(position.y, minY, maxY);
        return new Vector2(x, y);
    }
}
