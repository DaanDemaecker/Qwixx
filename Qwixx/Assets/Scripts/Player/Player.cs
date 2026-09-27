using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _scoreSheetPrefab = null;

    private string _playerName = "Player";
    private Color _playerColor = Color.white;
    private int _playerNumber = 0;


    public void StartGame()
    {
        if(_scoreSheetPrefab!= null)
        {
            Instantiate(_scoreSheetPrefab).transform.parent = Camera.main.transform;
        }
    }

    public void InitializeValues(LobbyPlayerData data)
    {
        _playerName = data.Name.ToString();
        _playerColor = data.Color;
        _playerNumber = data.PlayerNumber;
    }
}
