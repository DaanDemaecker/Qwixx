using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _scoreSheetPrefab = null;


    public void StartGame()
    {
        if(_scoreSheetPrefab!= null)
        {
            Instantiate(_scoreSheetPrefab).transform.parent = Camera.main.transform;
        }
    }
}
