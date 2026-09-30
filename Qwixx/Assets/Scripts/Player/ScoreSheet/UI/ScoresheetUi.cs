using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public void InitInfo(LobbyPlayerData playerData)
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
        while(_nColoredOptions.Count < options.Count)
        {
            GameObject newOption = Instantiate(_optionPrefab, _nColoredOptionsParent.transform);
            ScoreSheetUiOption optionComponent = newOption.GetComponent<ScoreSheetUiOption>();
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
        while (_coloredOptions.Count < options.Count)
        {
            GameObject newOption = Instantiate(_optionPrefab, _coloredOptionsParent.transform);
            ScoreSheetUiOption optionComponent = newOption.GetComponent<ScoreSheetUiOption>();
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
}
