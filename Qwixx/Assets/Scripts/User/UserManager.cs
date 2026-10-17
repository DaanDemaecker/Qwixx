using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    private static UserManager _instance = null;

    public static UserManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private bool _isHostDevice = false;

    public bool IsHostDevice
    {
        get
        {
            return _isHostDevice;
        }
    }

    [SerializeField]
    private int _userId = -1;

    private bool _isSignedIn = false;

    public bool IsSignedIn
    {
        get
        {
            return _isSignedIn;
        }
    }

    private void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        InitializationOptions options = new InitializationOptions();
        options.SetProfile($"Player_{_userId}");


        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _isSignedIn = true;
    }

    public string GetId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
}
