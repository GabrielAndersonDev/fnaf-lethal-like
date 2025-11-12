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
public struct EnemyAICurve
{
    public EnemyState factor;
    public AnimationCurve curve;
}

[Serializable]
public struct EnemyNoiseCurve
{
    public AnimationCurve distance;
    public AnimationCurve intensity;
}

[Serializable]
[CreateAssetMenu(fileName = "EnemyAIData", menuName = "Enemies/AI/Enemy AI Data")]
public class EnemyAIData : ScriptableObject
{
    [SerializeField]
    List<EnemyAIWeight> aiWeightList = new();
    [SerializeField]
    List<EnemyVisionWeight> visionWeightList = new();
    [SerializeField]
    List<EnemyAICurve> enemyAICurves = new();

    public Dictionary<EnemyState, float> enemyAIWeight;
    public Dictionary<EnemyVisionState, float> enemyVisionWeight;
    public Dictionary<EnemyState, AnimationCurve> enemyAICurveDic;

    public AnimationCurve distanceFromPlayerCurve;
    public EnemyNoiseCurve noiseCurve;

    private void Awake()
    {
        InitWeightDic();
    }

    private void InitWeightDic()
    {
        enemyAIWeight = new Dictionary<EnemyState, float>();
        enemyVisionWeight = new Dictionary<EnemyVisionState, float>();
        enemyAICurveDic = new Dictionary<EnemyState, AnimationCurve>();

        enemyAIWeight.Clear();
        enemyVisionWeight.Clear();
        enemyAICurveDic.Clear();

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

        foreach (EnemyAICurve curve in enemyAICurves)
        {
            if (!enemyAICurveDic.ContainsKey(curve.factor))
            {
                enemyAICurveDic.Add(curve.factor, curve.curve);
            }
            else
            {
                Debug.LogError("EnemyState already exists in enemyAICurveDic.");
                Debug.Break();
            }
        }
    }
}
