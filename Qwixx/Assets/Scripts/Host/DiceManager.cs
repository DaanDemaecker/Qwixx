using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField]
    private Host _host = null;

    private DiceRoll _rollInfo = new DiceRoll();

    private List<Die> _dice = new();

    private bool _initialRollDone = false;

    private ulong _activePlayerId = 0;

    private void Awake()
    {
        _rollInfo.OnRollCompleteEvent.AddListener(DiceManager_OnRollComplete);
        _rollInfo.Reset();
    }

    private void DiceManager_OnRollComplete(DiceRoll.DiceRollData data)
    {
        if (_initialRollDone)
        {
            if (_host != null)
            {
                _host.RollComplete(data, _activePlayerId);
            }
        }
        else
        {
            _initialRollDone = true;
        }
    }

    public void AddDie(Die die)
    {
        _dice.Add(die);
        die.OnRollCompleteEvent.AddListener(DiceManager_OnSingleRollComplete);
    }

    public void Roll(ulong activePlayerId)
    {
        _activePlayerId = activePlayerId;
        _rollInfo.Reset();

        foreach (Die die in _dice)
        {
            die.MoveToStartPosition();
            die.Roll();
        }
    }

    private void DiceManager_OnSingleRollComplete(DiceColor color, int value)
    {
        _rollInfo.SetValue(color, value);
    }
}
