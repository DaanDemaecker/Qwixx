using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ScoreSheet : MonoBehaviour
{
    [SerializeField]
    private GameObject _scoreSheetUiPrefab = null;

    private ScoreSheetUi _scoreSheetUI = null;

    public UnityEvent OnRollClickedEvent;

    public UnityEvent OnConfirmTurnEvent;

    public void SetInfo(LobbyPlayerData data)
    {
        if (_scoreSheetUiPrefab != null)
        {
            var scoreSheet = Instantiate(_scoreSheetUiPrefab);

            scoreSheet.transform.SetParent(Camera.main.transform, false);


            if (scoreSheet.TryGetComponent<ScoreSheetUi>(out _scoreSheetUI))
            {
                _scoreSheetUI.SetInfo(data);
                _scoreSheetUI.OnConfirmTurnEvent.AddListener(ScoreSheet_OnConfirmTurn);
                _scoreSheetUI.OnRollClickedEvent.AddListener(ScoreSheet_OnRollClicked);
            }
        }
    }

    private void ScoreSheet_OnRollClicked()
    {
        OnRollClickedEvent.Invoke();
    }

    private void ScoreSheet_OnConfirmTurn()
    {
        OnConfirmTurnEvent.Invoke();
    }

    public void StartTurn(bool activePlayer)
    {
        if(_scoreSheetUI != null)
        {
            _scoreSheetUI.StartTurn(activePlayer);
        }
    }

    public void SetRollData(DiceRoll.DiceRollData data)
    {
        HandleData(data);

        if(_scoreSheetUI != null)
        {
            _scoreSheetUI.SetRollData();
        }
    }

    private void HandleData(DiceRoll.DiceRollData data)
    {

    }
}
