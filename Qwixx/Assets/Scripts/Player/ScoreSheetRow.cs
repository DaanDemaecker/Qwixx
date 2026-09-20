using System.Collections.Generic;
using UnityEngine;

public class ScoreSheetRow : MonoBehaviour
{
    [SerializeField]
    private DiceColor _color;

    [SerializeField]
    private List<ScoreSheetEntry> _entries = new();

    public int _highestValueIndex = -1;

    private int _crossedOffAmount = 0;

    public bool CanBeCrossed(int value)
    {
        for(int i = _highestValueIndex + 1; i < _entries.Count; ++i)
        {
            if(value == _entries[i].Value)
            {
                return true;
            }
        }


        return false;
    }

    public void Select(int value, bool selected)
    {
        for (int i = 0; i < _entries.Count; ++i)
        {
            if (value == _entries[i].Value)
            {
                _entries[i].Select(selected);
            }
        }
    }

    public void Cross(int value)
    {
        for (int i = 0; i < _entries.Count; ++i)
        {
            _entries[i].Disable();
            if (value == _entries[i].Value)
            {
                if (!_entries[i].CrossedOff)
                {
                    _crossedOffAmount++;
                    _entries[i].CrossOff();
                    _highestValueIndex = Mathf.Max(_highestValueIndex, i);
                }

                break;
            }
        }

        foreach(ScoreSheetEntry entry in _entries)
        {
            entry.Select(false);
        }
    }
}
