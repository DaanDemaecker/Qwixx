using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string _playerName = "Player";
    private Color _playerColor = Color.white;
    private int _playerNumber = -1;
    private ulong _clientId = ulong.MaxValue;

    private ScoreSheet _scoreSheet = null;

    public void InitializeValues(LobbyPlayerData data)
    {
        _playerName = data.Name.ToString();
        _playerColor = data.Color;
        _playerNumber = data.PlayerNumber;
        _clientId = data.ClientId;

        if(TryGetComponent<ScoreSheet>(out _scoreSheet))
        {
            _scoreSheet.SetInfo(data);
        }
    }

    public void SetRollData(DiceRoll.DiceRollData data)
    {
        Debug.LogError($"Roll complete: {data.Color0_1}, {data.Color0_2}, {data.Color1}, {data.Color2}, {data.Color3}, {data.Color4}");
    }
}
