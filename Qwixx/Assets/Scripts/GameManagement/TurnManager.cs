using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    private PlayerManager _playerManager = null;

    private int _currentTurnIndex = -1;

    private List<ulong> _playerIds = new();

    private Dictionary<ulong, bool> _playersReady = new();

    public void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    public void AddPlayerEntry(ulong clientId)
    {
        _playerIds.Add(clientId);
        _playersReady[clientId] = false;
    }

    public void TurnConfirmed(ulong playerId)
    {
        _playersReady[playerId] = true;

        if(AllPlayersReady())
        {
            ResetReadyPlayers();
            if(_playerManager != null)
            {
                _playerManager.StartTurn(GetNextTurnId());
            }
        }
    }

    public ulong GetNextTurnId()
    {
        if(_playerIds.Count < 1)
        {
            return ulong.MaxValue;
        }

        _currentTurnIndex++;

        if(_currentTurnIndex >= _playerIds.Count)
        {
            _currentTurnIndex = 0;
        }

        return _playerIds[_currentTurnIndex];
    }

    private bool AllPlayersReady()
    {
        foreach(KeyValuePair<ulong, bool> pair in _playersReady)
        {
            if(!pair.Value)
            {
                return false;
            }
        }

        return true;
    }

    private void ResetReadyPlayers()
    {
        foreach (ulong key in _playersReady.Keys.ToList<ulong>())
        {
            _playersReady[key] = false;
        }
    }
}
