using System.Collections.Generic;
using UnityEngine;

public class ScoreSheetUiRow : MonoBehaviour
{
    [SerializeField]
    private GameObject _entryPrefab = null;

    private List<ScoreSheetUiRowEntry> _entries = new();

    public void SetEntries(List<ScoreSheetRow.ScoreSheetRowEntry> entries)
    {
        if(_entryPrefab == null)
        {
            return;
        }

        foreach(ScoreSheetRow.ScoreSheetRowEntry entry in entries)
        {
            GameObject entryObject = Instantiate(_entryPrefab, transform);

            ScoreSheetUiRowEntry entryComponent = entryObject.GetComponent<ScoreSheetUiRowEntry>();

            if(entryComponent != null)
            {
                entryComponent.SetEntry(entry);
            }

            _entries.Add(entryComponent);
        }
    }
}
