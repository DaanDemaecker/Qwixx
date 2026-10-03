using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ScoreSheetUiRow : MonoBehaviour
{
    [SerializeField]
    private GameObject _entryPrefab = null;


    private Dictionary<DiceColor, Dictionary<int, ScoreSheetUiRowEntry>> _entries = new();

    public void SetPulsating(ScoreSheetRow.ScoreSheetRowEntry entry, bool pulsating)
    {
        if (_entries.ContainsKey(entry.Color) && _entries[entry.Color].ContainsKey(entry.Value))
        {
            _entries[entry.Color][entry.Value].SetPulsing(pulsating);
        }
    }

    public void UpdateEntries(List<ScoreSheetRow.ScoreSheetRowEntry> entries)
    {
        foreach(ScoreSheetRow.ScoreSheetRowEntry entry in entries)
        {
            if (_entries.ContainsKey(entry.Color) && _entries[entry.Color].ContainsKey(entry.Value))
            {
                _entries[entry.Color][entry.Value].SetEntry(entry);
            }
        }
    }

    public void SetEntries(List<ScoreSheetRow.ScoreSheetRowEntry> entries)
    {
        if(_entryPrefab == null)
        {
            return;
        }

        foreach (ScoreSheetRow.ScoreSheetRowEntry entry in entries)
        {
            GameObject entryObject = Instantiate(_entryPrefab, transform);

            ScoreSheetUiRowEntry entryComponent = entryObject.GetComponent<ScoreSheetUiRowEntry>();

            if (entryComponent != null)
            {
                if(!_entries.ContainsKey(entry.Color))
                {
                    _entries[entry.Color] = new Dictionary<int, ScoreSheetUiRowEntry>();
                }

                _entries[entry.Color][entry.Value] = entryComponent;
            }
        }

        UpdateEntries(entries);
    }
}
