using Unity.Netcode;
using UnityEngine;

public class ScoreSheet : MonoBehaviour
{
    [SerializeField]
    private GameObject _scoreSheetUiPrefab = null;

    public void SetInfo(LobbyPlayerData data)
    {
        if (_scoreSheetUiPrefab != null)
        {
            var scoreSheet = Instantiate(_scoreSheetUiPrefab);

            scoreSheet.transform.SetParent(Camera.main.transform, false);

            ScoreSheetUi scoreSheetUi = null;

            if(scoreSheet.TryGetComponent<ScoreSheetUi>(out scoreSheetUi))
            {
                scoreSheetUi.SetInfo(data);
            }
        }
    }
}
