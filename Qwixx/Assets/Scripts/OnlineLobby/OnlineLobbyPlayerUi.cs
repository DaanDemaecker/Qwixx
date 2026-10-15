using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyPlayerUi : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _playerNameText = null;

    [SerializeField]
    private Image _backgroundImage = null;

    [SerializeField]
    private Color _notReadyColor = Color.white;

    [SerializeField]
    private Color _readyColor = Color.green;

    [SerializeField]
    [Range(0f, 1f)]
    private float _backgroundOpacity = 0.5f;

    private bool _isReady = false;

    private string _playerId = string.Empty;

    public string PlayerId
    {
        set
        {
            _playerId = value;
        }

        get
        {
            return _playerId;
        }
    }

    public void SetData(string playerName, string playerColor, string playerReady)
    {
        UnityEngine.ColorUtility.TryParseHtmlString($"#{playerColor}", out Color colorOutput);

        if(_playerNameText != null)
        {
            _playerNameText.text = playerName;
            _playerNameText.color = colorOutput;
        }

        if(bool.TryParse(playerReady, out bool isReadyOutput))
        {
            if(isReadyOutput != _isReady)
            {
                _isReady = isReadyOutput;

                Color newColor = _isReady ? _readyColor : _notReadyColor;
                newColor.a = _backgroundOpacity;

                if(_backgroundImage != null)
                {
                    _backgroundImage.color = newColor;
                }
            }
        }
    }
}
