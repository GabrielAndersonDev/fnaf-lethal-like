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
public struct EnemyVisionWeight
{
    public EnemyVisionState factor;
    public float weight;
}

[Serializable]
[CreateAssetMenu(fileName = "EnemyAIData", menuName = "Enemies/AI/Enemy AI Data")]
public class EnemyAIData : ScriptableObject
{
    public List<EnemyAIWeight> aiWeightList = new();
    public List<EnemyVisionWeight> visionWeightList = new();

    public Dictionary<EnemyState, float> enemyAIWeight;
    public Dictionary<EnemyVisionState, float> enemyVisionWeight;

    public AnimationCurve distanceFromPlayerCurve;

    private void Awake()
    {
        InitWeightDic();
    }

    private void InitWeightDic()
    {
        enemyAIWeight = new Dictionary<EnemyState, float>();
        enemyVisionWeight = new Dictionary<EnemyVisionState, float>();
        enemyAIWeight.Clear();
        enemyVisionWeight.Clear();

        foreach (EnemyAIWeight weight in aiWeightList)
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

        foreach (EnemyVisionWeight weight in visionWeightList)
        {
            if (!enemyVisionWeight.ContainsKey(weight.factor))
            {
                enemyVisionWeight.Add(weight.factor, weight.weight);
            }
            else
            {
                Debug.LogError("Vision weight list already contains key: " + weight.factor);
                Debug.Break();
            }
        }
    }
}
