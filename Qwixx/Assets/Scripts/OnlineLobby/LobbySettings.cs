using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbySettings : MonoBehaviour
{
    [SerializeField]
    private GameObject _settingsContainer = null;

    [Header("Button")]
    [SerializeField]
    private Button _settingsButton = null;

    [SerializeField]
    private Image _settingsButtonImage = null;

    [SerializeField]
    private Sprite _openSettingsSprite = null;

    [SerializeField]
    private Sprite _closeSettingsSprite = null;


    private void Start()
    {
        if(_settingsContainer == null)
        {
            Debug.LogError("No settings container was asigned");
        }

        if(_settingsButton == null)
        {
            Debug.LogError("No settings button was asigned");
        }

        if(_settingsButtonImage == null)
        {
            Debug.LogError("No settings button image was asigned");
        }

        if (_openSettingsSprite == null)
        {
            Debug.LogError("No open settings sprite was asigned");
        }

        if(_closeSettingsSprite == null)
        {
            Debug.LogError("No close settings sprite was asigned");
        }

        CloseSettings();
    }

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(SettingsButtonClicked);
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveListener(SettingsButtonClicked);
    }



    private void SettingsButtonClicked()
    {
        if(_settingsContainer.activeInHierarchy)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    private void OpenSettings()
    {
        _settingsContainer.SetActive(true);
        _settingsButtonImage.sprite = _closeSettingsSprite;
    }

    private void CloseSettings()
    {
        _settingsContainer.SetActive(false);
        _settingsButtonImage.sprite = _openSettingsSprite;
    }
}
