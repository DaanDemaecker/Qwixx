using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string _playerName = "Player";
    private Color _playerColor = Color.white;
    private int _playerNumber = -1;
    private ulong _clientId = ulong.MaxValue;

    [SerializeField]
    private ScoreSheet _scoreSheet = null;

    public ScoreSheet ScoreSheet
    {
        get
        {
            return _scoreSheet;
        }
    }

    public void InitializeValues(LobbyPlayerData data)
    {
        _playerName = data.Name.ToString();
        _playerColor = data.Color;
        _playerNumber = data.PlayerNumber;
        _clientId = data.ClientId;

        if(_scoreSheet != null)
        {
            _scoreSheet.SetInfo(data);
        }
    }

    public void StartTurn(bool activePlayer)
    {
        if(_scoreSheet != null)
        {
            _scoreSheet.StartTurn(activePlayer);
        }
    }

    public void LockRow(int rowIndex)
    {
        if(_scoreSheet != null)
        {
            _scoreSheet.LockRow(rowIndex);
        }
    }

    public void SetRollData(DiceRoll.DiceRollData data, bool isActivePlayer)
    {
        if(_scoreSheet != null)
        {
            _scoreSheet.SetRollData(data, isActivePlayer);
        }
    }
}
