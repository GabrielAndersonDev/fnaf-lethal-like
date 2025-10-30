using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemyAIWeight
{
    public EnemyState factor;
    public float weight;
}

[Serializable]
[CreateAssetMenu(fileName = "EnemyAIData", menuName = "Enemies/AI/Enemy AI Data")]
public class EnemyAIData : ScriptableObject
{
    [SerializeField]
    public List<EnemyAIWeight> weightList = new();

    public Dictionary<EnemyState, float> enemyAIWeight;

    private void Awake()
    {
        InitWeightDic();
    }

    private void InitWeightDic()
    {
        enemyAIWeight = new Dictionary<EnemyState, float>();
        enemyAIWeight.Clear();

        foreach (EnemyAIWeight weight in weightList)
        {
            if (!enemyAIWeight.ContainsKey(weight.factor))
            {
                enemyAIWeight.Add(weight.factor, weight.weight);
            }
            else
            {
                Debug.LogError("Weight list already contains key: " + weight.factor);
                Debug.Break();
            }
        }
    }
}
