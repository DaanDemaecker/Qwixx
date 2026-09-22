using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Colors", menuName = "Scriptable Objects/Colors")]
public class Colors : ScriptableObject
{
    public Color Color0;
    public Color Color1;
    public Color Color2;
    public Color Color3;
    public Color Color4;
    public Color DisabledColor;

    public UnityEvent ColorChangedEvent = new UnityEvent();

    public void OnValidate()
    {
        ColorChangedEvent.Invoke();
    }
}
