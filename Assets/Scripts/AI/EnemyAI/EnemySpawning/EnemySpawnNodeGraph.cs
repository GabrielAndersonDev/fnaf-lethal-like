using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnGraph
{
    public EnemyType enemyType;
    public AnimationCurve spawnRateCurve;
}

[CreateAssetMenu(fileName = "EnemySpawnNodeGraph", menuName = "Enemies/Graphs/Enemy Spawn Node Graph")]
public class EnemySpawnNodeGraph : ScriptableObject
{
    public List<EnemySpawnGraph> enemySpawnGraphs = new();
    public Dictionary<EnemyType, AnimationCurve> enemySpawnRate;

    private void OnEnable()
    {
        enemySpawnRate = new Dictionary<EnemyType, AnimationCurve>();

        foreach (EnemySpawnGraph graph in enemySpawnGraphs)
        {
            if (!enemySpawnRate.ContainsKey(graph.enemyType))
            {
                enemySpawnRate.Add(graph.enemyType, graph.spawnRateCurve);
            }
        }
    }
}
