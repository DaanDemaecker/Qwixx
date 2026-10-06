using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName, bool additive)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }

    public void LoadSceneNetwork(string sceneName, bool additive)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }
}
