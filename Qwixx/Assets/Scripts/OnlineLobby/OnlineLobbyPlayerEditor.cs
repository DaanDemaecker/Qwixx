using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using WebSocketSharp;

public class OnlineLobbyPlayerEditor : NetworkBehaviour
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

    [SerializeField]
    private OnlineLobbyUiManager _uiManager = null;

    private bool _isConnected = false;

    private bool _isTryingToConnect = false;

    private string _playerId = string.Empty;

    private void Start()
    {
        if(_uiManager == null)
        {
            Debug.LogError("No Ui manager was asigned");
        }

        _playerId = UserManager.Instance.GetId();
    }

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

    private void Update()
    {
        ConnectedPlayer();
    }

    private async void ConnectedPlayer()
    {
        if(_isConnected || _isTryingToConnect)
        {
            return;
        }

        _isTryingToConnect = true;

        string relayJoinKey = OnlineLobbyManager.Instance.GetLobbyData(OnlineLobbyManager.RELAY_JOIN_CODE_KEY);

        if(relayJoinKey.IsNullOrEmpty())
        {
            _isTryingToConnect = false;
            return;
        }

        await RelayManager.Instance.StartClient(relayJoinKey);

        _isConnected = true; ;
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

            _uiManager.SetPlayerNameClientRpc(_playerId, newValue);
        }
    }

    private void OnColorSelected(Color color)
    {
        OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PLAYER_COLOR_KEY, color.ToHexString());

        _uiManager.SetPlayerColorClientRpc(_playerId, color);
    }

    public void OnReadyClickced()
    {
        ReadyApproved();
    }

    public void ReadyApproved()
    {
        OnlineLobbyManager.Instance.UpdatePlayerData(OnlineLobbyManager.PLAYER_READY_KEY, true.ToString());

        _uiManager.SetPlayerReadyClientRpc(_playerId, true);

        if (_mainContainer != null)
        {
            _mainContainer.SetActive(false);
        }
    }
}
