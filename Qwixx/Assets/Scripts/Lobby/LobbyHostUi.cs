using UnityEngine;
using UnityEngine.Events;

public class LobbyHostUi : MonoBehaviour
{
    public UnityEvent onStartGameClicked;
    public void OnStartGameClicked()
    {
        onStartGameClicked.Invoke();
    }
}
