using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private ScoreSheet _scoreSheet = null;

    public ScoreSheet ScoreSheet
    {
        get
        {
            return _scoreSheet;
        }
    }

    public void InitializeValues(string name, Color color)
    {
        if(_scoreSheet != null)
        {
            _scoreSheet.SetInfo(name, color);
        }
    }

    public void StartTurn(bool activePlayer)
    {
        if(_scoreSheet != null)
        {
            _scoreSheet.StartTurn(activePlayer);
        }
    }

    public void LockRow(int rowIndex)
    {
        if(_scoreSheet != null)
        {
            _scoreSheet.LockRow(rowIndex);
        }
    }

    public void SetRollData(DiceRoll.DiceRollData data, bool isActivePlayer)
    {
        if(_scoreSheet != null)
        {
            _scoreSheet.SetRollData(data, isActivePlayer);
        }
    }

    public int GetScore()
    {
        if(_scoreSheet != null)
        {
            return _scoreSheet.GetScore();
        }
        return 0;
    }
}
