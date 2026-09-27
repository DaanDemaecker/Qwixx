using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreSheetUi : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _nameText = null;

    [SerializeField]
    private Image _background = null;

    public UnityEvent OnRollClickedEvent;

    public void SetInfo(LobbyPlayerData playerData)
    {
        if (_nameText != null)
        {
            _nameText.text = playerData.Name.ToString();
            _nameText.color = playerData.Color;
        }

        if (_background != null)
        {
            _background.color = playerData.Color;
        }
    }

    public void OnRollClicked()
    {
        OnRollClickedEvent.Invoke();
    }
}