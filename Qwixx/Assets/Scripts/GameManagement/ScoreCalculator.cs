using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    private const int MAX_LIVES = 4;

    public static int MaxLives
    {
        get
        {
            return MAX_LIVES;
        }
    }

    private const int PENALTY_PER_LIFE = 5;

    [SerializeField]
    private List<int> _scoreConverter = new();

    public int GetScore(int crossedAmount)
    {
        int score = 0;

        if (crossedAmount > 0 && crossedAmount < _scoreConverter.Count)
        {
            score += _scoreConverter[crossedAmount - 1];
        }

        return score;
    }

    public int GetPenalty(int livesLeft)
    {
        return (MAX_LIVES - livesLeft) * PENALTY_PER_LIFE;
    }
}
