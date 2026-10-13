using UnityEngine;

public class OnlineLobbyUiManager : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _lobbyCodeText = null;

    private void Start()
    {
        if(_lobbyCodeText != null)
        {
            _lobbyCodeText.text = OnlineLobbyManager.Instance.GetLobbyCode();
        }
    }
}
