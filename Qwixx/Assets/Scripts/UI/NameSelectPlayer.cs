using UnityEngine;

public class NameSelectPlayer : MonoBehaviour
{
    [SerializeField]
    private int _playerNumber = 0;

    public int PlayerNumber
    {
        get
        {
            return _playerNumber;
        }
    }

    [SerializeField]
    private TMPro.TextMeshProUGUI _nameText = null;

    [SerializeField]
    private GameObject _readyImage = null;

    [SerializeField]
    private GameObject _mainContainer = null;

    public void SetActive(bool active)
    {
        if(_mainContainer != null)
        {
            _mainContainer.SetActive(active);
        }
    }

    public void SetInfo(PlayerInitData data)
    {

    }

    public void UpdateName(string name)
    {
        if(_nameText != null)
        {
            _nameText.text = name;
        }
    }

    public void SetReady(bool ready)
    {
        if(_readyImage != null)
        {
            _readyImage.SetActive(ready);
        }
    }

    public void UpdateColor(Color color)
    {
        if(_nameText != null)
        {
            _nameText.color = color;
        }
    }
}
