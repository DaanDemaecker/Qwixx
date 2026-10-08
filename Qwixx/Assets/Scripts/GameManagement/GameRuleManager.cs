using UnityEngine;

public class GameRuleManager : MonoBehaviour
{
    private static GameRuleManager _instance;

    public static GameRuleManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private GameRules _gameRules = null;

    public int MaxLives
    {
        get
        {
            return _gameRules.MaxLives;
        }
    }

    public int MinPlayers
    {
        get
        {
            return _gameRules.MinPlayers;
        }
    }

    public int MaxPlayers
    {
        get
        {
            return _gameRules.MaxPlayers;
        }
    }

    public int MinRowsLocked
    {
        get
        {
            return _gameRules.MinRowsLocked;
        }
    }

    public int MinCrossesForLock
    {
        get
        {
            return _gameRules.MinCrossedForLock;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("An instance of this singleton already exists");
        }

        if(_gameRules == null)
        {
            Debug.LogError("No gamerules were given");
        }

        _instance = this;
    }

    public int GetScore(int crossedAmount)
    {
        int score = 0;

        if (crossedAmount > 0 && crossedAmount < _gameRules.ScoreConverter.Count)
        {
            score += _gameRules.ScoreConverter[crossedAmount - 1];
        }

        return score;
    }

    public int GetPenalty(int livesLeft)
    {
        return (_gameRules.MaxLives - livesLeft) * _gameRules.PenaltyPerLife;
    }
}