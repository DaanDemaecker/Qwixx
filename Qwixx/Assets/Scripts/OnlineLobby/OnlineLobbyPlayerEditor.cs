using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class OnlineLobbyPlayerEditor : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainContainer = null;

    [SerializeField]
    private TMPro.TMP_InputField _playerNameInputField = null;

    [SerializeField]
    private int _colorOptionAmount = 9;

    [SerializeField]
    private GameObject _colorButtonPrefab = null;

    [SerializeField]
    private GameObject _colorButtonParent = null;

    [SerializeField]
    private UiGridScaler _gridScaler = null;

    private void OnEnable()
    {
        if(OnlineLobbyManager.Instance.IsHostDevice)
        {
            gameObject.SetActive(false);
            return;
        }

        if (_playerNameInputField != null)
        {
            _playerNameInputField.SetTextWithoutNotify(OnlineLobbyManager.Instance.GetPlayerData(OnlineLobbyManager.PLAYER_NAME_KEY));
            _playerNameInputField.onValueChanged.AddListener(OnValueChanged);
        }

        SetupButtons();
    }

    private void SetupButtons()
    {
        if(_colorButtonPrefab == null)
        {
            return;
        }
        
        if(_gridScaler != null)
        {
            float sqrt = Mathf.Sqrt(_colorOptionAmount);

            int sqrtInt = (int)sqrt;

            if(sqrtInt == sqrt)
            {
                _gridScaler.ColumnAmount = sqrtInt + 1;
            }
            else
            {
                _gridScaler.ColumnAmount = sqrtInt;
            }
            _gridScaler.RowAmount = sqrtInt;
        }

        for(float i = 1; i <= _colorOptionAmount; ++i)
        {
            GameObject buttonObject = Instantiate(_colorButtonPrefab, _colorButtonParent.transform);

            if(buttonObject.TryGetComponent<OnlineLobbyColorButton>(out OnlineLobbyColorButton buttonComponent))
            {
                float hue = Mathf.Lerp(0, 1, i/_colorOptionAmount);
                buttonComponent.SetColor(Color.HSVToRGB(hue, 1, 1));

                buttonComponent.onColorSelectedEvent.AddListener(OnColorSelected);
            }
        }
    }

    private void OnDisable()
    {
        if (_playerNameInputField != null)
        {
            _playerNameInputField.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    private void OnValueChanged(string newValue)
    {
        if(newValue.IsNullOrEmpty())
        {
            return;
        }
        else
        {
            OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PLAYER_NAME_KEY, newValue);
        }
    }

    private void OnColorSelected(Color color)
    {
        OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PLAYER_COLOR_KEY, color.ToHexString());
    }

    public void OnReadyClickced()
    {
        OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PLAYER_READY_KEY, true.ToString());

        if(_mainContainer != null)
        {
            _mainContainer.SetActive(false);
        }
    }
}
