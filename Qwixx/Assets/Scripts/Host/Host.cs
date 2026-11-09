using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using static ScoreSheetRow;

public class Host : NetworkBehaviour
{
    [SerializeField]
    private DiceManager _diceManager;

    [SerializeField]
    private ScoreSheetTracker _scoreSheetTracker;

    public UnityEvent<DiceRoll.DiceRollData, FixedString32Bytes> OnRollCompleteEvent;

    public void SpawnDice()
    {
        if (_diceManager == null)
        {
            Debug.LogError("Dice Manager is not assigned in Host.");
        }

        if(_scoreSheetTracker == null)
        {
            Debug.LogError("Score Sheet Tracker is not assigned in Host.");
        }


        List<DieSpawner> dieSpawners = new List<DieSpawner>(FindObjectsByType<DieSpawner>());

        foreach (DieSpawner spawner in dieSpawners)
        {
            Die die = spawner.SpawnDie();

            if (die != null)
            {
                _diceManager.AddDie(die);
            }
        }
    }


    public void RollDice(FixedString32Bytes activePlayerId)
    {
        _diceManager.Roll(activePlayerId);
    }

    public void RollComplete(DiceRoll.DiceRollData data, FixedString32Bytes activePlayerId)
    {
        OnRollCompleteEvent.Invoke(data, activePlayerId);
    }

    public void RegisterScoreSheetRow(ulong playerId, DiceColor color, int value)
    {
        _scoreSheetTracker.RegisterScoreSheetRow(playerId, color, value);
    }
}
