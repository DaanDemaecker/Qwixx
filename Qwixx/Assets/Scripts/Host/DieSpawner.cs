using UnityEngine;

public class DieSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _diePrefab = null;

    [SerializeField]
    private GameObject _dieParent = null;

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
                return dieComponent;
            }
        }

        return null;
    }
}
