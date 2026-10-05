using System;
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

    private Dictionary<DiceColor, Dictionary<int, ScoreSheetRow>> _ordereredRows = new();

    private bool _isActivePlayer = false;

    private int _lifeAmount = 0;

    private const int MAX_LIFE_AMOUNT = 5;

    // Options pairs combine the currently selected entry and a bool indicating whether the lock is selected
    private Tuple<ScoreSheetRow.ScoreSheetRowEntry, bool> _currentSelectedOptionNColored = new(ScoreSheetRow.DefaultEntry, false);

    private Tuple<ScoreSheetRow.ScoreSheetRowEntry, bool> _currentSelectedOptionColored = new(ScoreSheetRow.DefaultEntry, false);

    // Events
    public UnityEvent OnRollClickedEvent;

    public UnityEvent OnReadyClickedEvent;

    public UnityEvent<int> OnRowLockedEvent;

    public UnityEvent OnPlayerDiedEvent;


    private void OnDestroy()
    {
        if(_scoreSheetUI != null)
        {
            _scoreSheetUI.OnRollClickedEvent.RemoveAllListeners();
            _scoreSheetUI.OnReadyClickedEvent.RemoveAllListeners();
            _scoreSheetUI.OnOptionClickedEvent.RemoveAllListeners();
            Destroy(_scoreSheetUI.gameObject);
        }

    }

    public void Awake()
    {
        _lifeAmount = MAX_LIFE_AMOUNT;

        SortRows();
    }
    private void SortRows()
    {
        foreach(ScoreSheetRow row in _scoreSheetRows)
        {
            SortRow(row);
        }
    }

    private void SortRow(ScoreSheetRow row)
    {
        List<ScoreSheetRow.ScoreSheetRowEntry> entries = row.GetEntries();

        foreach(ScoreSheetRow.ScoreSheetRowEntry entry in entries)
        {
            if (EntryExists(entry))
            {
                Debug.LogError("A duplicate entry has been found, no entry can occurr twice!");
            }

            if(!_ordereredRows.ContainsKey(entry.Color))
            {
                _ordereredRows[entry.Color] = new();
            }

            _ordereredRows[entry.Color][entry.Value] = row;
        }
    }

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
                _scoreSheetUI.SetHearts(_lifeAmount);
                _scoreSheetUI.OnRollClickedEvent.AddListener(() => OnRollClickedEvent.Invoke());
                _scoreSheetUI.OnReadyClickedEvent.AddListener(ReadyClicked);
                _scoreSheetUI.OnOptionClickedEvent.AddListener(OptionClicked);
            }
        }
    }

    public void StartTurn(bool activePlayer)
    {
        _isActivePlayer = activePlayer;
        if (_scoreSheetUI != null)
        {
            _scoreSheetUI.StartTurn(activePlayer);
        }
    }

    public void SetRollData(DiceRoll.DiceRollData data, bool isActivePlayer)
    {
        _isActivePlayer = isActivePlayer;
        HandleData(data, out List<ScoreSheetRow.ScoreSheetRowEntry> nColoredOptions, out List<ScoreSheetRow.ScoreSheetRowEntry> coloredOptions, isActivePlayer);

        if (_scoreSheetUI != null)
        {
            _scoreSheetUI.SetRollData(nColoredOptions, coloredOptions);
        }
    }

    private void LockRow(ScoreSheetRow row)
    {
        for(int i = 0; i < _scoreSheetRows.Count; ++i)
        {
            if (_scoreSheetRows[i] == row)
            {
                OnRowLockedEvent.Invoke(i);
                return;
            }
        }

    }

    private void ReadyClicked()
    {
        if(_isActivePlayer)
        {
            if(!EntryExists(_currentSelectedOptionNColored.Item1) && !EntryExists(_currentSelectedOptionColored.Item1))
            {
                LoseLife();
            }
        }


        if (EntryExists(_currentSelectedOptionColored.Item1))
        {
            ScoreSheetRow currentRow = _ordereredRows[_currentSelectedOptionColored.Item1.Color][_currentSelectedOptionColored.Item1.Value];

            currentRow.CrossEntry(_currentSelectedOptionColored.Item1);
            if(_currentSelectedOptionColored.Item2)
            {
                currentRow.CrossEntry(currentRow.GetLockEntry());
                LockRow(currentRow);
            }
        }

        if (EntryExists(_currentSelectedOptionNColored.Item1))
        {
            ScoreSheetRow currentRow = _ordereredRows[_currentSelectedOptionNColored.Item1.Color][_currentSelectedOptionNColored.Item1.Value];

            currentRow.CrossEntry(_currentSelectedOptionNColored.Item1);
            if (_currentSelectedOptionNColored.Item2)
            {
                currentRow.CrossEntry(currentRow.GetLockEntry());
                LockRow(currentRow);
            }
        }


        if (_scoreSheetUI != null)
        {
            _scoreSheetUI.UpdateRows(_scoreSheetRows);
            _scoreSheetUI.SetPulsating(_currentSelectedOptionColored.Item1, false);

            if(_currentSelectedOptionColored.Item2)
            {
                _scoreSheetUI.SetPulsating(_ordereredRows[_currentSelectedOptionColored.Item1.Color][_currentSelectedOptionColored.Item1.Value].GetLockEntry(), false);
            }


            _scoreSheetUI.SetPulsating(_currentSelectedOptionNColored.Item1, false);

            if (_currentSelectedOptionNColored.Item2)
            {
                _scoreSheetUI.SetPulsating(_ordereredRows[_currentSelectedOptionNColored.Item1.Color][_currentSelectedOptionNColored.Item1.Value].GetLockEntry(), false);
            }
        }

        _currentSelectedOptionColored = new(ScoreSheetRow.DefaultEntry, false);
        _currentSelectedOptionNColored = new(ScoreSheetRow.DefaultEntry, false);


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

    private bool EntryExists(ScoreSheetRow.ScoreSheetRowEntry entry)
    {
        return _ordereredRows.ContainsKey(entry.Color) && _ordereredRows[entry.Color].ContainsKey(entry.Value);
    }

    private void OptionClicked(ScoreSheetRow.ScoreSheetRowEntry entry, bool isNColoredOption)
    {
        if (isNColoredOption)
        {
            // If new value is equal to colored value, unset colored value and set NColored
            if (entry.Value == _currentSelectedOptionColored.Item1.Value && entry.Color == _currentSelectedOptionColored.Item1.Color)
            {
                UnsetChoice(ref _currentSelectedOptionColored);
                SetChoice(ref _currentSelectedOptionNColored, entry);
            }
            // If new value is equal to thee Ncolored value, unset the choice
            else if (entry.Value == _currentSelectedOptionNColored.Item1.Value && entry.Color == _currentSelectedOptionNColored.Item1.Color)
            {
                UnsetChoice(ref _currentSelectedOptionNColored);
            }
            // Else, set new choice
            else
            {
                SetChoice(ref _currentSelectedOptionNColored, entry);
            }
        }
        else
        {
            // If new value is equal to nColored value, unset colored value and set colored
            if (entry.Value == _currentSelectedOptionNColored.Item1.Value && entry.Color == _currentSelectedOptionNColored.Item1.Color)
            {
                UnsetChoice(ref _currentSelectedOptionNColored);
                SetChoice(ref _currentSelectedOptionColored, entry);
            }
            // If new value is equal to thee colored value, unset the choice
            else if (entry.Value == _currentSelectedOptionColored.Item1.Value && entry.Color == _currentSelectedOptionColored.Item1.Color)
            {
                UnsetChoice(ref _currentSelectedOptionColored);
            }
            // Else, set new choice
            else
            {
                SetChoice(ref _currentSelectedOptionColored, entry);
            }
        }
    }

    private void UnsetChoice(ref Tuple<ScoreSheetRow.ScoreSheetRowEntry, bool> toUnset)
    {
        if(_scoreSheetUI != null)
        {
            if(toUnset.Item2 && EntryExists(toUnset.Item1))
            {
                _scoreSheetUI.SetPulsating(_ordereredRows[toUnset.Item1.Color][toUnset.Item1.Value].GetLockEntry(), false);
            }


            _scoreSheetUI.SetPulsating(toUnset.Item1, false);
        }

        toUnset = new(ScoreSheetRow.DefaultEntry, false);

    }

    private void SetChoice(ref Tuple<ScoreSheetRow.ScoreSheetRowEntry, bool> toSet, ScoreSheetRow.ScoreSheetRowEntry newValue)
    {
        UnsetChoice(ref toSet);

        bool lockSet = false;

        if(EntryExists(newValue))
        {
            lockSet = _ordereredRows[newValue.Color][newValue.Value].CanLockRow(newValue);
        }

        if (_scoreSheetUI != null)
        {
            _scoreSheetUI.SetPulsating(newValue, true);
            if(lockSet)
            {
                _scoreSheetUI.SetPulsating(_ordereredRows[newValue.Color][newValue.Value].GetLockEntry(), true);
            }
        }

        toSet = new(newValue, lockSet);
    }

    public void LockRow(int rowIndex)
    {
        if(rowIndex < 0 || rowIndex >= _scoreSheetRows.Count)
        {
            Debug.LogError("Row index is out of bounds");
            return;
        }

        ScoreSheetRow row = _scoreSheetRows[rowIndex];

        row.LockRow();

        if(_scoreSheetUI != null)
        {
            _scoreSheetUI.UpdateRows(_scoreSheetRows);
        }
    }

    private void LoseLife()
    {
        _lifeAmount--;

        if(_scoreSheetUI != null)
        {
            _scoreSheetUI.LoseHeart();
        }

        if(_lifeAmount <= 0)
        {
            OnPlayerDiedEvent.Invoke();
        }
    }
}
