using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreSheetUiOld : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _nameText = null;

    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private GameObject _rollButton = null;

    [SerializeField]
    private GameObject _confirmButton = null;

    public UnityEvent OnRollClickedEvent;
    public UnityEvent OnConfirmTurnEvent;

    public void Awake()
    {
        if(_rollButton != null)
        {
            _rollButton.SetActive(false);
        }

        if(_confirmButton != null)
        {
            _confirmButton.SetActive(true);
        }
    }

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

    public void StartTurn(bool activePlayer)
    {
        if (_rollButton != null)
        {
            _rollButton.SetActive(activePlayer);
        }
    }

    public void OnRollClicked()
    {
        if(_rollButton != null)
        {
            _rollButton.SetActive(false);
        }

        OnRollClickedEvent.Invoke();
    }

    public void OnConfirmTurn()
    {
        if (_confirmButton != null)
        {
            _confirmButton.SetActive(false);
        }

        OnConfirmTurnEvent.Invoke();
    }

    public void SetRollData()
    {
        if(_confirmButton != null)
        {
            _confirmButton.SetActive(true);
        }
    }
}