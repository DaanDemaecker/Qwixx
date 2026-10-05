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

    private List<int> _tempLockedRows = new();

    private List<int> _lockedRows = new();

    private const int MAX_LOCKED_ROWS = 1;

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

        if (AllPlayersReady())
        {
            NextTurn();
        }
    }

    private void NextTurn()
    {
        ResetReadyPlayers();

        HandleLockedRows();

        if (!ShouldGameEnd())
        {
            if (_playerManager != null)
            {
                _playerManager.StartTurnClientRpc(GetNextTurnId());
            }
        }
        else
        {
            EndGame();
        }
    }

    private bool ShouldGameEnd()
    {
        if(_lockedRows.Count >= MAX_LOCKED_ROWS)
        {
            return true;
        }

        return false;
    }

    private void EndGame()
    {
        if(_playerManager != null)
        {
            _playerManager.EndGame();
        }
    }

    private void HandleLockedRows()
    {
        foreach(int rowIndex in _tempLockedRows)
        {
            _lockedRows.Add(rowIndex);

            if(_playerManager != null)
            {
                _playerManager.LockRowClientRpc(rowIndex);
            }
        }

        _tempLockedRows.Clear();
    }

    public ulong GetNextTurnId()
    {
        if (_playerIds.Count < 1)
        {
            return ulong.MaxValue;
        }

        _currentTurnIndex++;

        if (_currentTurnIndex >= _playerIds.Count)
        {
            _currentTurnIndex = 0;
        }

        return _playerIds[_currentTurnIndex];
    }

    private bool AllPlayersReady()
    {
        foreach (KeyValuePair<ulong, bool> pair in _playersReady)
        {
            if (!pair.Value)
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

    public void LockRow(int rowIndex)
    {
        if (!_lockedRows.Contains(rowIndex) && !_tempLockedRows.Contains(rowIndex))
        {
            _tempLockedRows.Add(rowIndex);
        }
    }
}
