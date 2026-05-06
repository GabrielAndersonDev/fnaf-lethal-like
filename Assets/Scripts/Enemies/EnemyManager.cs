using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class EnemyPrefabPair
{
    public EnemyType type;
    public GameObject prefab;
}

[System.Serializable]
public class  EnemyDataPair
{
    public EnemyType type;
    public EnemyData data;
}

public class EnemyManager : NetworkBehaviour
{
    public static EnemyManager Singleton { get; private set; }

    public Dictionary<int, Enemy> spawnedEnemies = new();
    public List<EnemyType> roomSpawnedEnemyTypes = new();

    public List<EnemyPrefabPair> prefabPairList = new();
    public List<EnemyDataPair> enemyDataPairList = new();

    public Dictionary<EnemyType, GameObject> enemyPrefabDic = new();
    public Dictionary<EnemyType, EnemyData> enemyDataDic = new();

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;

        EnemyDicPop();
    }

    public void EnemyDicPop()
    {
        enemyPrefabDic.Clear();
        enemyDataDic.Clear();

        foreach (EnemyPrefabPair pair in prefabPairList)
        {
            if (!enemyPrefabDic.ContainsKey(pair.type))
            {
                enemyPrefabDic.Add(pair.type, pair.prefab);
            }
            else
            {
                Debug.LogWarning($"Enemy type {pair.type} already exists in the dictionary.");
                continue;
            }
        }

        foreach (EnemyDataPair pair in enemyDataPairList)
        {
            if (!enemyDataDic.ContainsKey(pair.type))
            {
                enemyDataDic.Add(pair.type, pair.data);
            }
            else
            {
                Debug.LogWarning($"Enemy type {pair.type} already exists in the dictionary.");
                continue;
            }
        }
    }

    public void PopulateAllEnemies()
    {
        foreach (MapSegment seg in MapManager.Singleton.segments)
        {
            if (seg != null
                && seg.GetType() == typeof(RoomSegment))
            {
                RoomSegment seg2 = (RoomSegment)seg;
                PopulateEnemies(seg2);
            }
        }
    }

    public void PopulateEnemies(RoomSegment seg)
    {
        roomSpawnedEnemyTypes.Clear();

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Enemy population can only be done on the server.");
            return;
        }

        if (seg.isEnemyGen)
        {
            Debug.Log($"Segment: {seg} is already populated with enemies.");
            return;
        }

        foreach (EnemySpawnNode node in seg.enemySpawnNodes)
        {
            if (node.isSpawned)
            {
                Debug.Log($"EnemySpawnNode {node.name} is already spawned.");
                continue;
            }

            EnemyType enemyType = SelectEnemyType(seg, node);

            if (enemyType == EnemyType.Invalid)
            {
                Debug.LogError($"No valid enemy type selected for {node.name} in segment {seg.name}.");
                Debug.Break();
                return;
            }
            else if (enemyType == EnemyType.None)
            {
                node.isNone = true;
                continue;
            }

            SpawnEnemy(node, enemyType);
        }

        // Will eventually add a check to see for minimum enemies spawned per segment
        seg.isEnemyGen = true;
    }

    public EnemyType SelectEnemyType(RoomSegment seg, EnemySpawnNode node)
    {
        if (node == null
            || seg == null)
        {
            Debug.LogError("EnemySpawnNode is null");
            Debug.Break();
            return EnemyType.Invalid;
        }

        Dictionary<EnemyType, float> enemyRates = new();

        foreach (EnemyType enemyType in node.enemySpawnNodeGraph.enemySpawnRate.Keys)
        {
            if (enemyRates.ContainsKey(enemyType))
            {
                Debug.LogWarning($"Enemy type {enemyType} already exists in the rates dictionary.");
                continue;
            }
            else
            {
                float rate = 1f;

                if (roomSpawnedEnemyTypes.Contains(enemyType))
                {
                    rate = 0f;
                }

                enemyRates.Add(enemyType, rate); // Initialize with a base rate of 1
            }
        }

        foreach (EnemyType enemyType in enemyRates.Keys.ToListPooled())
        {
            if (enemyType == EnemyType.None)
            {
                continue; // Skip None type
            }

            if (seg.enemySpawned[enemyType])
            {
                enemyRates[enemyType] = 0f;
            }
            else
            {
                enemyRates[enemyType] = node.enemySpawnNodeGraph.enemySpawnRate[enemyType].Evaluate(enemyRates[enemyType]);
            }
        }

        return GetEnemyRates(enemyRates);
    }

    public EnemyType GetEnemyRates(Dictionary<EnemyType, float> enemyRates)
    {
        if (enemyRates == null || enemyRates.Count == 0)
        {
            Debug.LogError("Enemy rates dictionary is null or empty.");
            Debug.Break();
        }

        Dictionary<EnemyType, int> enemyIntPair = new();

        int totalInt = 0;
        int selectedInt;

        foreach (EnemyType enemyType in enemyRates.Keys)
        {
            int enemyValue = (int)(enemyRates[enemyType] * 100);
            enemyIntPair.Add(enemyType, enemyValue);
            totalInt += enemyValue;
        }

        if (totalInt > 0)
        {
            selectedInt = UnityEngine.Random.Range(0, totalInt);
            int compareInt = 0;
            foreach (EnemyType enemyType in enemyIntPair.Keys)
            {
                compareInt += enemyIntPair[enemyType];
                if (compareInt >= selectedInt)
                {
                    return enemyType;
                }
            }
        } 
        else
        {
            Debug.LogError("Total enemy rates is zero or less. Cannot select an enemy type.");
            Debug.Break();
        }
        return EnemyType.Invalid;
    }

    public Enemy SpawnEnemy(EnemySpawnNode node, EnemyType enemyType)
    {
        GameObject newEnemy = Instantiate(enemyPrefabDic[enemyType], node.spawnLocation, node.spawnRotation);
        EnemyData enemyData = Instantiate(enemyDataDic[enemyType]);

        int enemyIDValue = spawnedEnemies.Count + 1;

        if (enemyData == null)
        {
            Debug.LogError($"Enemy data for type {enemyType} is null.");
            Debug.Break();
            return null;
        }

        if (newEnemy.TryGetComponent<Enemy>(out var enemyComponent))
        {
            newEnemy.GetComponent<NetworkObject>().Spawn();
            enemyComponent.enemyID.Value = enemyIDValue;
            enemyComponent.InitializeEnemy(enemyData);
            enemyComponent.roomSpawnedIn = node.parentSegment.gameObject;
            node.isSpawned = true;

            spawnedEnemies.Add(enemyIDValue, enemyComponent);
            roomSpawnedEnemyTypes.Add(enemyType);

            return enemyComponent;
        }
        else
        {
            Debug.LogError($"Enemy prefab for type {enemyType} does not have an Enemy component.");
            Debug.Break();
            return null;
        }
    }

    public void DestroyAllEnemies()
    {
        List<int> enemyIDs = new();

        foreach (int i in spawnedEnemies.Keys)
        {
            enemyIDs.Add(i);
        }

        Debug.Log("Destroying all enemies.");
        foreach (int enemy in enemyIDs)
        {
            Debug.Log("Destroying enemy " + spawnedEnemies[enemy] + " with ID of " + enemy);
            DestroyTargetEnemy(spawnedEnemies[enemy]);
        }
    }

    public void DestroyTargetEnemy(Enemy enemy)
    {
        if (spawnedEnemies.ContainsKey(enemy.enemyID.Value))
        {
            spawnedEnemies.Remove(enemy.enemyID.Value);
        }
        else
        {
            Debug.Log("Enemy does not exist in spawnedEnemies dictionary.");
        }

        enemy.GetComponent<NetworkObject>().Despawn(true);
    }
}
