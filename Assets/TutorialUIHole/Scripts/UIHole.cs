using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode()]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
public class UIHole : Graphic, ICanvasRaycastFilter
{
    [SerializeField] private Vector2 _center;
    [SerializeField, Range(0f, 2f)] private float _size;

    private static readonly int _HoleCenter = Shader.PropertyToID("_HoleData");
    private Material _material;
    private bool _needUpdate = false;

    public Vector2 Center
    {
        get => _center;
        set
        {
            _center = value;
            _needUpdate = true;
        }
    }

    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            _needUpdate = true;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _material = m_Material;
    }

    protected override void OnDisable()
    {
        _material = null;
        base.OnDisable();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        _needUpdate = true;
    }
#endif // UNITY_EDITOR

    private void Update()
    {
        if (_material == null)
        {
            _material = m_Material;
        }
        if (!_needUpdate) return;

        // xy: hole center, z: hole size, w: aspect
        Vector4 hole = new Vector4(
            _center.x,
            _center.y,
            Mathf.Max(_size * 0.5f, 0),
            (float)Screen.height / Screen.width // INVERTED aspect
        );
        _material.SetVector(_HoleCenter, hole);

        _needUpdate = false;
    }

    // Block raycast
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return !IsHole(sp);
    }

    private bool IsHole(Vector2 sp)
    {
        float width = Screen.width;
        float height = Screen.height;

        Vector2 viewport = new Vector2(sp.x / width, sp.y / height);
        Vector2 vec = viewport - _center;
        vec.y *= height / width;// aspect collection

        return vec.magnitude < (_size * 0.5f);
    }
}
