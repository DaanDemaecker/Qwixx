using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSheetRow : MonoBehaviour
{
    [Serializable]
    public struct ScoreSheetRowEntry
    {
        public int Value;
        public bool IsCrossed;
        public bool IsLocked;
        public DiceColor Color;
    }

    [SerializeField]
    private List<ScoreSheetRowEntry> _entries = new();

    private bool _rowLocked = false;

    private static ScoreSheetRowEntry _defaultEntry = new ScoreSheetRowEntry
    {
        Value = -1,
        IsCrossed = false,
        IsLocked = true,
        Color = DiceColor.Color0
    };

    public List<ScoreSheetRowEntry> GetEntries()
    {
        return _entries;
    }


    private ScoreSheetRowEntry GetAvailableEntry(int value, DiceColor color)
    {
        foreach (ScoreSheetRowEntry entry in _entries)
        {
            if (entry.Value == value && (entry.Color == color || color == DiceColor.Color0))
            {
                return entry;
            }
        }

        return _defaultEntry;
    }

    public ScoreSheetRowEntry GetAvailableEntryNColored(DiceRoll.DiceRollData data)
    {
        return GetAvailableEntry(data.Color0_1 + data.Color0_2, DiceColor.Color0);
    }

    public List<ScoreSheetRowEntry> GetAvailableEntriesColored(DiceRoll.DiceRollData data)
    {
        List<ScoreSheetRowEntry> list = new();

        ScoreSheetRowEntry entry;

        // Color1
        entry = GetAvailableEntry(data.Color0_1 + data.Color1, DiceColor.Color1);
        if(entry.Value > 0)
        {
            list.Add(entry);
        }

        entry = GetAvailableEntry(data.Color0_2 + data.Color1, DiceColor.Color1);
        if (entry.Value > 0)
        {
            list.Add(entry);
        }

        // Color2
        entry = GetAvailableEntry(data.Color0_1 + data.Color2, DiceColor.Color2);
        if (entry.Value > 0)
        {
            list.Add(entry);
        }

        entry = GetAvailableEntry(data.Color0_2 + data.Color2, DiceColor.Color2);
        if (entry.Value > 0)
        {
            list.Add(entry);
        }

        // Color3
        entry = GetAvailableEntry(data.Color0_1 + data.Color3, DiceColor.Color3);
        if (entry.Value > 0)
        {
            list.Add(entry);
        }

        entry = GetAvailableEntry(data.Color0_2 + data.Color3, DiceColor.Color3);
        if (entry.Value > 0)
        {
            list.Add(entry);
        }

        // Color4
        entry = GetAvailableEntry(data.Color0_1 + data.Color4, DiceColor.Color4);
        if (entry.Value > 0)
        {
            list.Add(entry);
        }

        entry = GetAvailableEntry(data.Color0_2 + data.Color4, DiceColor.Color4);
        if (entry.Value > 0)
        {
            list.Add(entry);
        }


        return list;
    }
}
