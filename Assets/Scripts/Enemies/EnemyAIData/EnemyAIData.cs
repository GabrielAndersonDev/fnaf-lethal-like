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
public struct EnemyNoiseCurve
{
    public AnimationCurve distance;
    public AnimationCurve intensity;
}

[Serializable]
public struct EnemyActionRate
{
    public EnemyAction factor;
    public float rate;
}

[Serializable]
public struct EnemyActionRateList
{
    public EnemyState state;
    public List<EnemyActionRate> actionRates;
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
    List<EnemyActionRateList> actionRateList = new();

    public Dictionary<EnemyState, float> enemyAIWeight;
    public Dictionary<EnemyVisionState, float> enemyVisionWeight;
    public Dictionary<EnemyState, Dictionary<EnemyAction, float>> enemyActionDictionary;

    public Dictionary<EnemyState, float> enemyAIRates;

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
        enemyActionDictionary = new Dictionary<EnemyState, Dictionary<EnemyAction, float>>();
        enemyAIRates = new Dictionary<EnemyState, float>();

        enemyAIWeight.Clear();
        enemyVisionWeight.Clear();
        enemyActionDictionary.Clear();
        enemyAIRates.Clear();

        foreach (EnemyAIWeight weight in aiWeightList)
        {
            if (!enemyAIWeight.ContainsKey(weight.factor))
            {
                enemyAIWeight.Add(weight.factor, weight.weight);
                enemyAIRates.Add(weight.factor, 1f);
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

        foreach (EnemyActionRateList list in actionRateList)
        {
            EnemyState state = list.state;
            Dictionary<EnemyAction, float> actionRates = new();


            for (int i = 0; i < list.actionRates.Count; i++)
            {
                EnemyAction action = list.actionRates[i].factor;
                float rate = list.actionRates[i].rate;

                if (actionRates.ContainsKey(action))
                {
                    Debug.LogError("Action rate list for state " + state + " already contains action: " + action);
                    Debug.Break();
                    continue;
                }

                actionRates.Add(action, rate);
            }

            enemyActionDictionary.Add(state, actionRates);
        }
    }
}
