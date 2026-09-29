using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreSheetUi : MonoBehaviour
{
    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _name = null;

    [SerializeField]
    private Button _readyButton = null;

    [SerializeField]
    private Button _rollButton = null;



    // Events
    public UnityEvent OnReadyClickedEvent;

    public UnityEvent OnRollClickedEvent;

    private void Awake()
    {
        if(_readyButton != null)
        {
            _readyButton.onClick.AddListener(ReadyClicked);
        }

        if(_rollButton != null)
        {
            _rollButton.onClick.AddListener(RollClicked);
            _rollButton.gameObject.SetActive(false);
        }
    }

    public void InitInfo(LobbyPlayerData playerData)
    {
        if (_background != null)
        {
            _background.color = playerData.Color;
        }

        if (_name != null)
        {
            _name.color = playerData.Color;
            _name.text = playerData.Name.ToString();
        }
    }

    public void StartTurn(bool activePlayer)
    {
        if(_rollButton != null)
        {
            _rollButton.gameObject.SetActive(activePlayer);
        }
    }

    public void SetRollData(DiceRoll.DiceRollData data)
    {
        if (_readyButton != null)
        {
            _readyButton.gameObject.SetActive(true);
        }
    }

    private void ReadyClicked()
    {
        if (_readyButton != null)
        {
            _readyButton.gameObject.SetActive(false);
        }
        OnReadyClickedEvent.Invoke();
    }

    private void RollClicked()
    {
        if (_rollButton != null)
        {
            _rollButton.gameObject.SetActive(false);
        }
        OnRollClickedEvent.Invoke();
    }
}
