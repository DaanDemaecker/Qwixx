using System.Collections.Generic;
using UnityEngine;
using static ScoreSheetRow;

public class ScoreSheetTracker : MonoBehaviour
{
    // This dictionary holds the score sheet entries for all players, where the key is the player ID and the value is another dictionary that maps DiceColor to the corresponding score value.
    private Dictionary<ulong, Dictionary<DiceColor, List<int>>> _allPlayerEntries = new();

    private bool _hasListChanged = false;

    public void RegisterScoreSheetRow(ulong playerId, DiceColor color, int value)
    {
        if (!_allPlayerEntries.ContainsKey(playerId))
        {
            _allPlayerEntries[playerId] = new();
        }

        if (!_allPlayerEntries[playerId].ContainsKey(color))
        {
            _allPlayerEntries[playerId][color] = new();
        }

        if (!_allPlayerEntries[playerId][color].Contains(value))
        {
            _allPlayerEntries[playerId][color].Add(value);
        }

        _hasListChanged = true;
    }

    private void LateUpdate()
    {
        if(_hasListChanged)
        {
            Debug.Log(_allPlayerEntries.Count);
            _hasListChanged = false;
        }
    }
}
