using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EndscreenManager : MonoBehaviour
{
    [SerializeField]
    private List<EndscreenPlayerUi> _players = null;

    private void Awake()
    {
        PlayerManager playerMangar = FindAnyObjectByType<PlayerManager>();

        if(playerMangar != null)
        {
            NetworkList<PlayerData> playerDatas = playerMangar.PlayerDatas;

            List<PlayerData> sortedDatas = SortList(playerDatas);

            SetPlayerUis(sortedDatas);
        }
    }

    private void SetPlayerUis(List<PlayerData> playerDatas)
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

    private List<PlayerData> SortList(NetworkList<PlayerData> playerDatas)
    {
        List<PlayerData> sortedDatas = new();

        List<int> toIgnoreIndices = new();

        while(playerDatas.Count > toIgnoreIndices.Count)
        {
            int index = GetMaxScoreIndex(playerDatas, toIgnoreIndices);
            sortedDatas.Add(playerDatas[index]);

            toIgnoreIndices.Add(index);
        }
        

        return sortedDatas;
    }

    private int GetMaxScoreIndex(NetworkList<PlayerData> playerDatas, List<int> toIgnoreIndices)
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
}
