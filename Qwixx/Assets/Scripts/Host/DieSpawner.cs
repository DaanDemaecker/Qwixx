using UnityEngine;

public class DieSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _diePrefab = null;

    [SerializeField]
    private GameObject _dieParent = null;

    [SerializeField]
    private DiceColor _dieColor = DiceColor.Color0;

    public Die SpawnDie()
    {
        if(_diePrefab != null)
        {
            GameObject die = Instantiate(_diePrefab);

            die.transform.SetParent(_dieParent.transform, false);
            die.transform.position = transform.position;
            Die dieComponent = die.GetComponent<Die>();

            if (dieComponent != null)
            {
                dieComponent.BeginPosition = transform.position;
                dieComponent.SetColor(_dieColor);
                return dieComponent;
            }
        }

        return null;
    }
}
