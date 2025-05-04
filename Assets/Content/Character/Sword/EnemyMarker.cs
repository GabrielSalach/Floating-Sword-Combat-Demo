using UnityEngine;

public class EnemyMarker : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Camera _camera;
    private Transform _target;
    
    public void SetTarget(Transform target, Camera cam)
    {
        _camera = cam;
        _target = target;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.position = _camera.WorldToScreenPoint(_target.position);
        if (_target == null || _camera == null) return;

        Renderer rdr = _target.GetComponent<Renderer>();
        if (rdr == null) return;

        // 1. Bounding box en world space
        Bounds bounds = rdr.bounds;

        // 2. Obtenir les 8 coins de la bounding box
        Vector3[] corners = new Vector3[8];
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        // Les 8 coins (signes combinés des extents)
        int i = 0;
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    corners[i++] = center + Vector3.Scale(extents, new Vector3(x, y, z));
                }
            }
        }

        // 3. Projeter en screen space et trouver les min/max
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        foreach (Vector3 worldPoint in corners)
        {
            Vector3 screenPoint = _camera.WorldToScreenPoint(worldPoint);

            // Ne pas dessiner si l'objet est derrière la caméra
            if (screenPoint.z < 0) return;

            Vector2 screenPos = new Vector2(screenPoint.x, Screen.height - screenPoint.y); // Y inversé pour OnGUI
            min = Vector2.Min(min, screenPos);
            max = Vector2.Max(max, screenPos);
        }

        Rect screenRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        
        _rectTransform.sizeDelta = screenRect.size;
    }
}
