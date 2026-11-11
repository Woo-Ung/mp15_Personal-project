using UnityEngine;

public class LineRendererPolygon : MonoBehaviour
{
    [SerializeField][Range(3, 100)] private int _polygonPoints = 3;
    [field: SerializeField][Min(0.1f)] public float _radius { get; private set; }

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.loop = true;
    }

    private void Update()
    {
        Play();
    }

    public void SetRadius(float radius)
    {
        _radius = Mathf.Max(0.1f, radius);
    }

    private void Play()
    {
        _lineRenderer.positionCount = _polygonPoints;
        float anglePerStep = 2 * Mathf.PI * ((float)1 / _polygonPoints);

        for(int i = 0; i< _polygonPoints; ++i)
        {
            Vector2 point = Vector2.zero;
            float angle = anglePerStep * i;

            point.x = Mathf.Cos(angle) * _radius;
            point.y = Mathf.Sin(angle) * _radius;

            _lineRenderer.SetPosition(i, point);
        }
    }
}
