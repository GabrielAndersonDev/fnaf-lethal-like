using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PopulateEnemies(MapSegment seg)
    {
        foreach (EnemyType enemy in seg.segmentData.spawnableEnemies)
        {
            if (enemy == EnemyType.None)
            {
                Debug.Log("EnemyType is None");
                return;
            }
        }

        foreach (EnemySpawnNode node in seg.enemySpawnNodes)
        {
            if (node.isSpawned)
            {
                Debug.Log($"EnemySpawnNode {node.name} is already spawned.");
                continue;
            }

            SelectEnemyType(seg, node);
        }
    }

    public void SelectEnemyType(MapSegment seg, EnemySpawnNode node)
    {
        Dictionary<EnemyType, float> enemyRates = node.spawnableEnemies;

        if (node == null)
        {
            Debug.LogError("EnemySpawnNode is null");
            Debug.Break();
            return;
        }

        foreach (EnemyType enemyType in enemyRates.Keys)
        {
            if (seg.enemySpawned[enemyType])
            {
                enemyRates[enemyType] = 0f;
            }
            else
            {
                enemyRates[enemyType] = node.enemySpawnNodeGraph.enemySpawnRate[enemyType].Evaluate(enemyRates[enemyType]);
            }
        }

        
    }

    public Dictionary<EnemyType, float> GetEnemyRates(Dictionary<EnemyType, float> enemyRates)
    {
        if (enemyRates == null || enemyRates.Count == 0)
        {
            Debug.LogError("Enemy rates dictionary is null or empty.");
            Debug.Break();
        }
        


        return enemyRates;
    }
}
