using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField]
    private Host _host = null;

    private DiceRoll _rollInfo = new DiceRoll();

    private List<Die> _dice = new();

    private void Awake()
    {
        _rollInfo.OnRollCompleteEvent.AddListener(DiceManager_OnRollComplete);
    }

    private void DiceManager_OnRollComplete(DiceRoll.DiceRollData data)
    {
        if(_host != null)
        {
            _host.RollComplete(data);
        }
    }

    public void AddDie(Die die)
    {
        _dice.Add(die);
    }

    public void Roll()
    {
        _rollInfo.Reset();

        foreach (Die die in _dice)
        {
            die.OnRollCompleteEvent.AddListener(DiceManager_OnSingleRollComplete);
            die.MoveToStartPosition();
            die.Roll();
        }
    }

    private void DiceManager_OnSingleRollComplete(DiceColor color, int value)
    {
        _rollInfo.SetValue(color, value);
    }
}
