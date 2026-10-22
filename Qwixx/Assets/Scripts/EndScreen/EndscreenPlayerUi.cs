using UnityEngine;

public class EndscreenPlayerUi : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainContainer = null;

    [SerializeField]
    private TMPro.TMP_Text _playerNameText = null;

    [SerializeField]
    private TMPro.TMP_Text _playerScoreText = null;

    public void SetInfo(ScoreManager.PlayerScoreData data)
    {
        SetActive(true);

        if (_playerNameText != null)
        {
            _playerNameText.text = data.PlayerName.ToString();
            _playerNameText.color = data.Color;
        }

        if (_playerScoreText != null)
        {
            _playerScoreText.text = data.Score.ToString();
            _playerScoreText.color = data.Color;
        }
    }

    public void SetActive(bool active)
    {
        if (_mainContainer != null)
        {
            _mainContainer.SetActive(active);
        }
    }
}
