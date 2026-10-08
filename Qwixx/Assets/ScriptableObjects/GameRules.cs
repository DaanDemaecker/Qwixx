using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "Scriptable Objects/GameRules")]
public class GameRules : ScriptableObject
{
    public int MaxLives = 4;
    public int PenaltyPerLife = 5;
    public int MinCrossedForLock = 5;
    public int MinRowsLocked = 0;

    public int MinPlayers = 1;
    public int MaxPlayers = 4;

    public List<int> ScoreConverter = new List<int>();
}
