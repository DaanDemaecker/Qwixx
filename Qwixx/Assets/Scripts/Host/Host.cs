using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Host : NetworkBehaviour
{
    [SerializeField]
    private DiceManager _diceManager;

    public UnityEvent<DiceRoll.DiceRollData, FixedString32Bytes> OnRollCompleteEvent;

    public void SpawnDice()
    {
        List<DieSpawner> dieSpawners = new List<DieSpawner>(FindObjectsByType<DieSpawner>());

        foreach(DieSpawner spawner in dieSpawners)
        {
            Die die = spawner.SpawnDie();

            if(die != null && _diceManager != null)
            {
                _diceManager.AddDie(die);
            }
        }
    }


    public void RollDice(FixedString32Bytes activePlayerId)
    {
        if(_diceManager != null)
        {
            _diceManager.Roll(activePlayerId);
        }
    }

    public void RollComplete(DiceRoll.DiceRollData data, FixedString32Bytes activePlayerId)
    {
        OnRollCompleteEvent.Invoke(data, activePlayerId);
    }
}
