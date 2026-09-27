using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class Host : NetworkBehaviour
{
    List<Die> _dice = new();

    public void SpawnDice()
    {
        List<DieSpawner> dieSpawners = new List<DieSpawner>(FindObjectsByType<DieSpawner>());

        foreach(DieSpawner spawner in dieSpawners)
        {
            Die die = spawner.SpawnDie();

            if(die != null)
            {
                _dice.Add(die);
            }
        }
    }


    public void RollDice()
    {
        foreach(Die die in _dice)
        {
            die.MoveToStartPosition();
            die.Roll();
        }
    }
}
