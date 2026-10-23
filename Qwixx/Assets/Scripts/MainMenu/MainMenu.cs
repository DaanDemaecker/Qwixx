using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button _hostButton = null;

    [SerializeField]
    private Button _joinButton = null;

    [SerializeField]
    private Button _quitButton = null;


    private void Awake()
    {
        if (_hostButton != null)
        {
            _hostButton.onClick.AddListener(OnHostButtonClicked);
        }
        if (_joinButton != null)
        {
            _joinButton.onClick.AddListener(OnJoinButtonClicked);
        }
        if (_quitButton != null)
        {
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        SetupHostButton();
        SetupQuitButton();
    }

    private void SetupQuitButton()
    {
        if(_quitButton != null)
        {
#if UNITY_ANDROID 
            _quitButton.gameObject.SetActive(false);   
#elif UNITY_WEBGL
            _quitButton.gameObject.SetActive(false);
#endif
        }
    }

    private void SetupHostButton()
    {
        if(_hostButton != null)
        {
#if UNITY_ANDROID
            _quitButton.gameObject.SetActive(false);   
#endif
        }
    }

    private void OnHostButtonClicked()
    {
        UserManager.Instance.IsHostDevice = true;
        GameManager.Instance.LoadLobbyHandlerScene();
    }

    private void OnJoinButtonClicked()
    {
        UserManager.Instance.IsHostDevice = false;
        GameManager.Instance.LoadLobbyHandlerScene();
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }
}
