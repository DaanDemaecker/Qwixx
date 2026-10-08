using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ScoreSheetRow : MonoBehaviour
{
    [Serializable]
    public struct ScoreSheetRowEntry
    {
        public int Value;
        public bool IsCrossed;
        public bool IsLocked;
        public bool IsLock;
        public DiceColor Color;
    }

    [SerializeField]
    private List<ScoreSheetRowEntry> _entries = new();

    private bool _rowLocked = false;

    private int _crossedCount = 0;

    public int CrossedCount
    {
        get
        {
            return _crossedCount;
        }
    }

    private static ScoreSheetRowEntry _defaultEntry = new ScoreSheetRowEntry
    {
        Value = -1,
        IsCrossed = false,
        IsLocked = true,
        IsLock = false,
        Color = DiceColor.Color0
    };

    public static ScoreSheetRowEntry DefaultEntry
    {
        get
        {
            return _defaultEntry;
        }
    }


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

    public void CrossEntry(ScoreSheetRowEntry entry)
    {
        if (entry.Value < 0)
        {
            return;
        }

        bool crossedOff = false;

        for (int i = _entries.Count - 1; i >= 0; --i)
        {
            if (_entries[i].Color == entry.Color && _entries[i].Value == entry.Value)
            {
                ScoreSheetRowEntry currentEntry = _entries[i];

                currentEntry.IsCrossed = true;

                _entries[i] = currentEntry;

                crossedOff = true;

                _crossedCount++;
            }

            if (crossedOff)
            {
                ScoreSheetRowEntry currentEntry = _entries[i];

                if (currentEntry.IsLocked)
                {
                    break;
                }

                currentEntry.IsLocked = true;

                _entries[i] = currentEntry;
            }
        }
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
        if (entry.Value > 0)
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

    public ScoreSheetRowEntry GetLockEntry()
    {
        if (_entries.Count > 1 && _entries[_entries.Count - 1].IsLock)
        {
            return _entries[_entries.Count - 1];
        }

        return _defaultEntry;
    }

    public bool CanLockRow(ScoreSheetRowEntry entry)
    {
        if (_rowLocked || _crossedCount < GameRuleManager.Instance.MinCrossesForLock)
        {
            return false;
        }

        ScoreSheetRowEntry finalEntry = _defaultEntry;

        for (int i = _entries.Count - 1; i >= 0; --i)
        {
            if (_entries[i].IsLock)
            {
                continue;
            }

            finalEntry = _entries[i];
            break;
        }

        if (entry.Color == finalEntry.Color && entry.Value == finalEntry.Value)
        {
            return true;
        }


        return false;
    }

    public void LockRow()
    {
        for (int i = _entries.Count - 1; i >= 0; --i)
        {
            ScoreSheetRowEntry currentEntry = _entries[i];

            if(currentEntry.IsLocked)
            {
                return;
            }

            currentEntry.IsLocked = true;

            _entries[i] = currentEntry;
        }
    }
}
