using UnityEngine;

public class ScoreSheetUiHeart : MonoBehaviour
{
    [SerializeField]
    private GameObject _heartIcon = null;

    private bool _isAlive = true;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
    }

    public void DisableHeart()
    {
        _isAlive = false;

        if(_heartIcon != null)
        {
            _heartIcon.SetActive(false);
        }
    }
}
