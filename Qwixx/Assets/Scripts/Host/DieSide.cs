using UnityEngine;

public class DieSide : MonoBehaviour
{
    [SerializeField]
    private int _value = 0;

    public int Value
    {
        get
        {
            return _value;
        }
    }
}
