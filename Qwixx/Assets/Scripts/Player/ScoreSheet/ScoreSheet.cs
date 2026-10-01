using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreSheet : MonoBehaviour
{
    [SerializeField]
    private GameObject _scoreSheetUiPrefab = null;

    private ScoreSheetUi _scoreSheetUI = null;

    [SerializeField]
    private List<ScoreSheetRow> _scoreSheetRows = new();

    // Options
    private ScoreSheetRow.ScoreSheetRowEntry _currentSelectedOptionNColored = ScoreSheetRow.DefaultEntry;

    private ScoreSheetRow.ScoreSheetRowEntry _currentSelectedOptionColored = ScoreSheetRow.DefaultEntry;

    // Events
    public UnityEvent OnRollClickedEvent;

    public UnityEvent OnReadyClickedEvent;

    public void SetInfo(LobbyPlayerData data)
    {
        if (_scoreSheetUiPrefab != null)
        {
            var scoreSheet = Instantiate(_scoreSheetUiPrefab);

            scoreSheet.transform.SetParent(Camera.main.transform, false);


            if (scoreSheet.TryGetComponent<ScoreSheetUi>(out _scoreSheetUI))
            {
                _scoreSheetUI.InitInfo(data);
                _scoreSheetUI.SetRows(_scoreSheetRows);
                _scoreSheetUI.OnRollClickedEvent.AddListener(() => OnRollClickedEvent.Invoke());
                _scoreSheetUI.OnReadyClickedEvent.AddListener(ReadyClicked);
                _scoreSheetUI.OnOptionClickedEvent.AddListener(OptionClicked);
            }
        }
    }

    public void StartTurn(bool activePlayer)
    {
        if (_scoreSheetUI != null)
        {
            _scoreSheetUI.StartTurn(activePlayer);
        }
    }

    public void SetRollData(DiceRoll.DiceRollData data, bool isActivePlayer)
    {
        HandleData(data, out List<ScoreSheetRow.ScoreSheetRowEntry> nColoredOptions, out List<ScoreSheetRow.ScoreSheetRowEntry> coloredOptions, isActivePlayer);

        if (_scoreSheetUI != null)
        {
            _scoreSheetUI.SetRollData(nColoredOptions, coloredOptions);
        }
    }

    private void ReadyClicked()
    {
        foreach(ScoreSheetRow row in _scoreSheetRows)
        {
            row.CrossEntry(_currentSelectedOptionColored);
            row.CrossEntry(_currentSelectedOptionNColored);
        }

        _currentSelectedOptionColored = ScoreSheetRow.DefaultEntry;
        _currentSelectedOptionNColored = ScoreSheetRow.DefaultEntry;

        if (_scoreSheetUI != null)
        {
            _scoreSheetUI.SetRows(_scoreSheetRows);
        }

        OnReadyClickedEvent.Invoke();
    }

    private void HandleData(DiceRoll.DiceRollData data, out List<ScoreSheetRow.ScoreSheetRowEntry> nColoredOptions, out List<ScoreSheetRow.ScoreSheetRowEntry> coloredOptions, bool isActivePlayer)
    {
        nColoredOptions = new();
        coloredOptions = new();

        foreach (var row in _scoreSheetRows)
        {
            nColoredOptions.Add(row.GetAvailableEntryNColored(data));
            if (isActivePlayer)
            {
                coloredOptions.AddRange(row.GetAvailableEntriesColored(data));
            }
        }
    }

    private void OptionClicked(ScoreSheetRow.ScoreSheetRowEntry entry, bool isNColoredOption)
    {
        if (isNColoredOption)
        {
            if(entry.Value == _currentSelectedOptionColored.Value && entry.Color == _currentSelectedOptionColored.Color)
            {
                return;
            }    

            _currentSelectedOptionNColored = entry;

        }
        else
        {
            if (entry.Value == _currentSelectedOptionNColored.Value && entry.Color == _currentSelectedOptionNColored.Color)
            {
                return;
            }

            _currentSelectedOptionColored = entry;
        }
    }
}
