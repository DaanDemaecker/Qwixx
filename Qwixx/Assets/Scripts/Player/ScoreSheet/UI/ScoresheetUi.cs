using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class ScoreSheetUi : MonoBehaviour
{
    [SerializeField]
    private Image _background = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI _name = null;

    [SerializeField]
    private Button _readyButton = null;

    [SerializeField]
    private Button _rollButton = null;

    // Hears
    [Header("Hearts")]
    [SerializeField]
    private GameObject _heartPrefab = null;

    [SerializeField]
    private GameObject _heartParent = null;

    private List<ScoreSheetUiHeart> _hearts = new();

    // Rows
    [Header("Rows")]
    [SerializeField]
    private GameObject _rowParent = null;

    [SerializeField]
    private GameObject _rowPrefab = null;

    private List<ScoreSheetUiRow> _rows = new();

    [Header("Options")]
    // Options
    [SerializeField]
    private GameObject _nColoredOptionsParent = null;

    [SerializeField]
    private GameObject _coloredOptionsParent = null;

    [SerializeField]
    private GameObject _optionPrefab = null;

    private List<ScoreSheetUiOption> _nColoredOptions = new();

    private List<ScoreSheetUiOption> _coloredOptions = new();

    // Events
    public UnityEvent OnReadyClickedEvent;

    public UnityEvent OnRollClickedEvent;

    public UnityEvent<ScoreSheetRow.ScoreSheetRowEntry, bool> OnOptionClickedEvent;

    private void Awake()
    {
        if(_readyButton != null)
        {
            _readyButton.onClick.AddListener(ReadyClicked);
        }

        if(_rollButton != null)
        {
            _rollButton.onClick.AddListener(RollClicked);
            _rollButton.gameObject.SetActive(false);
        }
    }

    public void SetPulsating(ScoreSheetRow.ScoreSheetRowEntry entry, bool pulsating)
    {
        foreach(ScoreSheetUiRow row in _rows)
        {
            row.SetPulsating(entry, pulsating);
        }
    }

    public void UpdateRows(List<ScoreSheetRow> rows)
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            if (i < rows.Count)
            {
                _rows[i].gameObject.SetActive(true);
                _rows[i].UpdateEntries(rows[i].GetEntries());
            }
            else
            {
                _rows[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetRows(List<ScoreSheetRow> rows)
    {
        if(_rowPrefab == null)
        {
            return;
        }

        foreach(ScoreSheetRow row in rows)
        {
            GameObject rowObject = Instantiate(_rowPrefab, _rowParent.transform);

            ScoreSheetUiRow rowComponent = rowObject.GetComponent<ScoreSheetUiRow>();

            if (rowComponent != null)
            {
                _rows.Add(rowComponent);
                rowComponent.SetEntries(row.GetEntries());
            }
        }
    }

    public void InitInfo(PlayerData playerData)
    {
        if (_background != null)
        {
            _background.color = playerData.Color;
        }

        if (_name != null)
        {
            _name.color = playerData.Color;
            _name.text = playerData.Name.ToString();
        }
    }

    public void SetHearts(int amount)
    {
        if(_heartPrefab != null && _heartParent != null)
        {
            for(int i = 0; i < amount; ++i)
            {
                GameObject heartObject = Instantiate(_heartPrefab, _heartParent.transform);

                if(heartObject.TryGetComponent<ScoreSheetUiHeart>(out ScoreSheetUiHeart heartComponent))
                {
                    _hearts.Add(heartComponent);
                }
            }
        }
    }

    public void LoseHeart()
    {
        foreach(ScoreSheetUiHeart heart in _hearts)
        {
            if(heart.IsAlive)
            {
                heart.DisableHeart();
                break;
            }
        }
    }

    public void StartTurn(bool activePlayer)
    {
        if(_rollButton != null)
        {
            _rollButton.gameObject.SetActive(activePlayer);
        }
    }

    public void SetRollData(List<ScoreSheetRow.ScoreSheetRowEntry> nColoredOptions, List<ScoreSheetRow.ScoreSheetRowEntry> coloredOptions)
    {
        SetNColoredOptions(nColoredOptions);

        SetColoredOptions(coloredOptions);

        if (_readyButton != null)
        {
            _readyButton.gameObject.SetActive(true);
        }
    }

    private void SetNColoredOptions(List<ScoreSheetRow.ScoreSheetRowEntry> options)
    {
        if (_nColoredOptionsParent != null)
        {
            _nColoredOptionsParent.SetActive(true);
        }

        while (_nColoredOptions.Count < options.Count)
        {
            GameObject newOption = Instantiate(_optionPrefab, _nColoredOptionsParent.transform);
            ScoreSheetUiOption optionComponent = newOption.GetComponent<ScoreSheetUiOption>();
            optionComponent.OnOptionClickedEvent.AddListener(OptionClicked);
            optionComponent.IsNColoredOption = true;
            _nColoredOptions.Add(optionComponent);
        }

        for(int i = 0; i < _nColoredOptions.Count; i++)
        {
            if(i < options.Count)
            {
                _nColoredOptions[i].SetEntry(options[i]);
                _nColoredOptions[i].gameObject.SetActive(true);
            }
            else
            {
                _nColoredOptions[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetColoredOptions(List<ScoreSheetRow.ScoreSheetRowEntry> options)
    {
        if (_coloredOptionsParent != null)
        {
            _coloredOptionsParent.SetActive(true);
        }

        while (_coloredOptions.Count < options.Count)
        {
            GameObject newOption = Instantiate(_optionPrefab, _coloredOptionsParent.transform);
            ScoreSheetUiOption optionComponent = newOption.GetComponent<ScoreSheetUiOption>();
            optionComponent.IsNColoredOption = false;
            optionComponent.OnOptionClickedEvent.AddListener(OptionClicked);
            _coloredOptions.Add(optionComponent);
        }

        for (int i = 0; i < _coloredOptions.Count; i++)
        {
            if (i < options.Count)
            {
                _coloredOptions[i].SetEntry(options[i]);
                _coloredOptions[i].gameObject.SetActive(true);
            }
            else
            {
                _coloredOptions[i].gameObject.SetActive(false);
            }
        }
    }

    private void ReadyClicked()
    {
        if (_readyButton != null)
        {
            _readyButton.gameObject.SetActive(false);
        }

        if(_nColoredOptionsParent != null)
        {
            _nColoredOptionsParent.SetActive(false);
        }

        if(_coloredOptionsParent != null)
        {
            _coloredOptionsParent.SetActive(false);
        }


        OnReadyClickedEvent.Invoke();
    }

    private void RollClicked()
    {
        if (_rollButton != null)
        {
            _rollButton.gameObject.SetActive(false);
        }
        OnRollClickedEvent.Invoke();
    }

    private void OptionClicked(ScoreSheetRow.ScoreSheetRowEntry entry, bool isNColoredOption)
    {
        OnOptionClickedEvent.Invoke(entry, isNColoredOption);
    }
}
