using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class OnlineTurnManager : NetworkBehaviour
{
    private List<FixedString32Bytes> _playerIds = new();

    private Dictionary<FixedString32Bytes, bool> _playersReady = new();

    private int _currentPlayerIndex = -1;

    private OnlinePlayerManager _playerManager;

    private List<int> _tempLockedRows = new();

    private List<int> _lockedRows = new();

    private bool _playerDied = false;

    private void Start()
    {
        if (!TryGetComponent<OnlinePlayerManager>(out _playerManager))
        {
            Debug.LogError("OnlinePlayerManager not found");
        }
    }

    [Rpc(SendTo.Server)]
    public void AddPlayerRpc(FixedString32Bytes playerId)
    {
        if (!_playerIds.Contains(playerId))
        {
            _playerIds.Add(playerId);
        }

        _playersReady[playerId] = false;
    }


    private void ResetPlayersReady()
    {
        foreach (var playerName in _playersReady.Keys.ToList())
        {
            _playersReady[playerName] = false;
        }
    }

    public void OnTurnConfirmed(FixedString32Bytes playerId)
    {
        OnTurnConfirmedServerRpc(playerId);
    }

    [Rpc(SendTo.Server)]
    private void OnTurnConfirmedServerRpc(FixedString32Bytes playerId)
    {
        _playersReady[playerId.ToString()] = true;

        if (AllPlayersReady())
        {
            NextTurn();
        }
    }

    private bool AllPlayersReady()
    {
        foreach (var isReady in _playersReady.Values)
        {
            if (!isReady)
            {
                return false;
            }
        }

        return true;
    }

    private void NextTurn()
    {
        ResetPlayersReady();

        HandleLockedRows();

        if (!ShouldGameEnd())
        {
            _playerManager.StartTurnClientRpc(GetNextTurnId());
        }
        else
        {
            _playerManager.EndGame();
        }
    }

    private bool ShouldGameEnd()
    {
        if (_lockedRows.Count >= GameRuleManager.Instance.MinRowsLocked)
        {
            return true;
        }

        if (_playerDied)
        {
            return true;
        }

        return false;
    }

    private void HandleLockedRows()
    {
        foreach (int rowIndex in _tempLockedRows)
        {
            _lockedRows.Add(rowIndex);
            _playerManager.LockRowClientRpc(rowIndex);
        }


        _tempLockedRows.Clear();
    }

    public FixedString32Bytes GetNextTurnId()
    {
        if (_playerIds.Count < 1)
        {
            return string.Empty;
        }

        _currentPlayerIndex++;

        if (_currentPlayerIndex >= _playerIds.Count)
        {
            _currentPlayerIndex = 0;
        }

        return _playerIds[_currentPlayerIndex];
    }

    public void OnPlayerDied()
    {
        OnPlayerDiedServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OnPlayerDiedServerRpc()
    {
        _playerDied = true;
    }

    public void OnRowLocked(int rowIndex)
    {
        LockRowServerRPc(rowIndex);
    }

    [Rpc(SendTo.Server)]
    private void LockRowServerRPc(int rowIndex)
    {
        if (!_lockedRows.Contains(rowIndex) && !_tempLockedRows.Contains(rowIndex))
        {
            _tempLockedRows.Add(rowIndex);
        }
    }

    public void ResetGame()
    {
        _playerIds.Clear();

        _playersReady.Clear();

        _currentPlayerIndex = -1;

        _tempLockedRows.Clear();

        _lockedRows.Clear();

        _playerDied = false;
    }
}
