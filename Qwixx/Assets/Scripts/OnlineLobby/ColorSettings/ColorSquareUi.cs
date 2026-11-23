using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorSquareUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    [SerializeField]
    private RectTransform _rectTransform = null;

    [SerializeField]
    private RectTransform _indicatorTransform = null;

    [SerializeField]
    private Image _squareImage = null;

    private const string HUE_PROPERTY_NAME = "_Hue";

    private float _value = 0;

    private float _saturation = 0;

    private bool _isDragging = false;

    public UnityEvent<float> OnValueChangedEvent = new UnityEvent<float>();

    public UnityEvent<float> OnSaturationChangedEvent = new UnityEvent<float>();

    private void Start()
    {
        if(_rectTransform == null)
        {
            Debug.LogError("No rect transform was set");
        }

        if(_indicatorTransform == null)
        {
            Debug.LogError("No indicator transform was set");
        }

        if(_squareImage == null)
        {
            Debug.LogError("No square image was set");
        }
    }

    public void SetHue(float hue)
    {
        _squareImage.material.SetFloat(HUE_PROPERTY_NAME, hue);
    }

    private Vector2 GetRelativePos(Vector2 worldPos)
    {
        Vector2 pos = Vector2.zero;

        Vector3[] corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);

        Vector3 topLeftCorner = corners[0];
        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;

        Vector2 distanceToCorner = worldPos - new Vector2(topLeftCorner.x, topLeftCorner.y);

        pos.x = distanceToCorner.x / width;
        pos.y = distanceToCorner.y / height;

        return pos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
        UpdatePointerPos(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_isDragging)
            return;


        UpdatePointerPos(eventData.position);
    }

    private void UpdatePointerPos(Vector2 absolutePos)
    {
        Vector3[] corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);

        Vector3 topLeftCorner = corners[0];

        if (absolutePos.x < topLeftCorner.x)
        {
            absolutePos.x = topLeftCorner.x;
        }
        else if (absolutePos.x > topLeftCorner.x + _rectTransform.rect.width)
        {
            absolutePos.x = topLeftCorner.x + _rectTransform.rect.width;
        }

        if (absolutePos.y < topLeftCorner.y)
        {
            absolutePos.y = topLeftCorner.y;
        }
        else if (absolutePos.y > topLeftCorner.y + _rectTransform.rect.height )
        {
            absolutePos.y = topLeftCorner.y + _rectTransform.rect.height;
        }


        Vector2 relativePos = GetRelativePos(absolutePos);

        if (_value != relativePos.x)
        {
            _value = relativePos.x;
            OnValueChangedEvent.Invoke(_value);
        }

        if (_saturation != relativePos.y)
        {
            _saturation = relativePos.y;
            OnSaturationChangedEvent.Invoke(_saturation);
        }

        _indicatorTransform.position = absolutePos;
    }
}
