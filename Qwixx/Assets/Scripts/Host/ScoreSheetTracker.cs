using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class ScoreSheetTracker : MonoBehaviour
{
    [SerializeField]
    private Host _host = null;


    // This dictionary holds the score sheet entries for all players, where the key is the player ID and the value is another dictionary that maps DiceColor to the corresponding score value.
    private Dictionary<ulong, Dictionary<DiceColor, List<int>>> _allPlayerEntries = new();

    private bool _hasListChanged = false;

    private List<DiceColor> _availableColors = new();

    private void Awake()
    {
        if(_host == null)
        {
            Debug.LogError("No host was registered");
        }
    }

    public void RegisterScoreSheetRow(ulong playerId, DiceColor color, int value)
    {
        if(value == 0)
        {
            return;
        }

        if(!_availableColors.Contains(color))
        {
            _availableColors.Add(color);
        }

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
        if (_hasListChanged)
        {
            if(_allPlayerEntries.Count != PlayerManager.Instance.GetPlayerCount())
            {
                return;
            }

            CheckColors();
            _hasListChanged = false;
        }
    }

    private void CheckColors()
    {
        List<DiceColor> toRemove = new();

        foreach(DiceColor color in _availableColors)
        {
            bool isColorStillAvailable = false;

            foreach(ulong playerId in _allPlayerEntries.Keys)
            {
                if(_allPlayerEntries[playerId].ContainsKey(color))
                {
                    isColorStillAvailable = true;
                    break;
                }
            }

            if(!isColorStillAvailable)
            {
                toRemove.Add(color);
            }
        }

        foreach(DiceColor color in toRemove)
        {
            RemoveColor(color);
        }
    }

    private void RemoveColor(DiceColor color)
    {
        _availableColors.Remove(color);
        _host.RemoveDie(color);
    }

    public void RemoveEntry(ulong playerId, DiceColor color, int value)
    {
        if (!_allPlayerEntries.ContainsKey(playerId))
        {
            return;
        }

        if (!_allPlayerEntries[playerId].ContainsKey(color))
        {
            return;
        }

        if (_allPlayerEntries[playerId][color].Contains(value))
        {
            _allPlayerEntries[playerId][color].Remove(value);
        }

        if (_allPlayerEntries[playerId][color].Count == 0)
        {
            _allPlayerEntries[playerId].Remove(color);
        }

        _hasListChanged = true;
    }
}
