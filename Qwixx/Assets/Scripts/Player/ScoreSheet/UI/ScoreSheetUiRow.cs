using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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

        while(_entries.Count < entries.Count)
        {
            GameObject entryObject = Instantiate(_entryPrefab, transform);

            ScoreSheetUiRowEntry entryComponent = entryObject.GetComponent<ScoreSheetUiRowEntry>();

            if (entryComponent != null)
            {
                _entries.Add(entryComponent);
            }
        }


        for(int i = 0; i < _entries.Count; i++)
        {
            if(i < entries.Count)
            {
                _entries[i].SetEntry(entries[i]);
                _entries[i].gameObject.SetActive(true);
            }
            else
            {
                _entries[i].gameObject.SetActive(false);
            }
        }
    }
}
