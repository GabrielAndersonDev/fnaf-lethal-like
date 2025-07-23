using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void EnemyGen(MapSegment seg)
    {

    }
}
