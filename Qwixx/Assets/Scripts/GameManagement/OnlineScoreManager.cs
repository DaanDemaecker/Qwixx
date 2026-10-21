using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class OnlineScoreManager : NetworkBehaviour
{
    public struct PlayerScoreData : INetworkSerializable, IEquatable<PlayerScoreData>
    {
        public FixedString32Bytes PlayerName;
        public FixedString32Bytes PlayerId;
        public Color Color;
        public int Score;
        public bool ScoreSet;

        public bool Equals(PlayerScoreData other)
        {
            return PlayerId.Equals(other.PlayerId);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref PlayerId);
            serializer.SerializeValue(ref Score);
            serializer.SerializeValue(ref ScoreSet);

            float r = Color.r;
            float g = Color.g;
            float b = Color.b;
            float a = Color.a;

            serializer.SerializeValue(ref r);
            serializer.SerializeValue(ref g);
            serializer.SerializeValue(ref b);
            serializer.SerializeValue(ref a);

            if (serializer.IsReader)
            {
                Color = new Color(r, g, b, a);
            }
        }
    }

    private NetworkList<PlayerScoreData> _playerScores;

    public NetworkList<PlayerScoreData> PlayerScores
    {
        get
        {
            return _playerScores;
        }
    }

    private static OnlineScoreManager _instance = null;

    public static OnlineScoreManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public UnityEvent AllPlayerReadyEvent;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
            return;
        }
        _instance = this;

        _playerScores = new NetworkList<PlayerScoreData>();
    }

    public void ResetGame()
    {
        _playerScores.Clear();
    }

    public void RegisterPlayer(FixedString32Bytes playerId, FixedString32Bytes playerName, Color playerColor)
    {
        RegisterPlayerRpc(playerId, playerName, playerColor);
    }

    [Rpc(SendTo.Server, InvokePermission =RpcInvokePermission.Everyone)]
    private void RegisterPlayerRpc(FixedString32Bytes playerId, FixedString32Bytes playerName, Color playerColor)
    {
        _playerScores.Add(new PlayerScoreData { PlayerName = playerName, PlayerId = playerId, Color = playerColor, Score = 0, ScoreSet = false });
    }

    public void RegisterScore(FixedString32Bytes playerId, int score)
    {
        RegisterScoreRpc(playerId, score);
    }

    [Rpc(SendTo.Server)]
    private void RegisterScoreRpc(FixedString32Bytes playerId, int score)
    {
        int index = GetIndexById(playerId);

        if (index < 0 && index >= _playerScores.Count)
        {
            return;
        }

        PlayerScoreData newData = new PlayerScoreData
        {
            PlayerName = _playerScores[index].PlayerName,
            PlayerId = playerId,
            Color = _playerScores[index].Color,
            Score = score,
            ScoreSet = true
        };

        _playerScores.Set(index, newData, true);

        if (AllPlayerReady())
        {
            AllPlayerReadyEvent.Invoke();
        }
    }

    private int GetIndexById(FixedString32Bytes playerId)
    {
        for(int i = 0; i < _playerScores.Count; ++i)
        {
            if(_playerScores[i].PlayerId.Equals(playerId))
            {
                return i;
            }
        }

        return -1;
    }

    private bool AllPlayerReady()
    {
        foreach (var playerScore in _playerScores)
        {
            if (!playerScore.ScoreSet)
            {
                return false;
            }
        }

        return true;
    }
}
