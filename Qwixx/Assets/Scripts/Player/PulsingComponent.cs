using UnityEngine;

public class PulsingComponent : MonoBehaviour
{
    [SerializeField]
    private float _maxScale = 1.5f;

    [SerializeField]
    private float _minScale = 0.75f;

    private float _pulsingTimer = 0.0f;

    private float _pulsingTime = 0.25f;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _pulsingTimer = 0;
    }

    private void OnDisable()
    {
        if(_rectTransform != null)
        {
            _rectTransform.localScale = Vector3.one;
        }
    }

    public void Update()
    {
        if (_rectTransform == null)
        {
            return;
        }

        _pulsingTimer += Time.deltaTime;

        float factor = Mathf.Sin(_pulsingTimer * (1/_pulsingTime));

        float scaleMiddle = (_maxScale + _minScale) / 2;

        float scaleMagnitude = (_maxScale - _minScale) / 2;

        float scale = scaleMiddle + factor * scaleMagnitude;

        _rectTransform.localScale = Vector3.one * scale;
    }
}
