using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EndscreenManager : MonoBehaviour
{
    [SerializeField]
    private List<EndscreenPlayerUi> _players = null;

    [SerializeField]
    private Button _mainMenuButton = null;

    public const string MAIN_MENU_SCENE_NAME = "StartMenuScene";

    private void Awake()
    {
        var playerDatas = OnlineScoreManager.Instance.PlayerScores;

        List<OnlineScoreManager.PlayerScoreData> sortedDatas = SortList(playerDatas);

        SetPlayerUis(sortedDatas);

        if(_mainMenuButton != null)
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
    }


    private void SetPlayerUis(List<OnlineScoreManager.PlayerScoreData> playerDatas)
    {
        for(int i = 0; i < _players.Count; ++i)
        {
            if(i < playerDatas.Count)
            {
                _players[i].SetInfo(playerDatas[i]);
            }
            else
            {
                _players[i].SetActive(false);
            }
        }
    }

    private List<OnlineScoreManager.PlayerScoreData> SortList(NetworkList<OnlineScoreManager.PlayerScoreData> playerDatas)
    {
        List<OnlineScoreManager.PlayerScoreData> sortedDatas = new();

        List<int> toIgnoreIndices = new();

        while(playerDatas.Count > toIgnoreIndices.Count)
        {
            int index = GetMaxScoreIndex(playerDatas, toIgnoreIndices);
            sortedDatas.Add(playerDatas[index]);

            toIgnoreIndices.Add(index);
        }
        

        return sortedDatas;
    }

    private int GetMaxScoreIndex(NetworkList<OnlineScoreManager.PlayerScoreData> playerDatas, List<int> toIgnoreIndices)
    {
        int index = -1;
        int maxScore = int.MinValue;

        for(int i = 0; i < playerDatas.Count; ++i)
        {
            if(toIgnoreIndices.Contains(i))
            {
                continue;
            }

            if (playerDatas[i].Score > maxScore)
            {
                maxScore = playerDatas[i].Score;
                index = i;
            }
        }

        return index;
    }

    public void OnMainMenuButtonClicked()
    {
        RelayManager.Instance.Disconnect();
        OnlineGameManager.Instance.ResetGame();
    }
}
