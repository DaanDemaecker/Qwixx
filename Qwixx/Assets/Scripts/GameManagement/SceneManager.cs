using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _sInstance;

    public static SceneManager Instance
    {
        get
        {
            return _sInstance;
        }
    }

    private void Awake()
    {
        if (_sInstance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }
        _sInstance = this;
    }

    public void LoadScene(string sceneName, bool additive)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }

    public void LoadSceneNetwork(string sceneName, bool additive)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }
}
