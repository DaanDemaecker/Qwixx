using UnityEngine;

public class LobbyPlayerUi : MonoBehaviour
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
    private GameObject _mainContainer = null;

    [SerializeField]
    private TMPro.TMP_Text _playerNameText = null;

    [SerializeField]
    private GameObject _playerReadyImage = null;

    public void SetInfo(LobbyPlayerData data)
    {
        SetActive(true);

        if(_playerNameText != null)
        {
            _playerNameText.text = data.Name.ToString();
            _playerNameText.color = data.Color;
        }

        if(_playerReadyImage != null)
        {
            _playerReadyImage.SetActive(data.IsReady);
        }
    }

    public void SetActive(bool active)
    {
        if(_mainContainer != null)
        {
            _mainContainer.SetActive(active);
        }
    }
}
