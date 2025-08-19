using System.Collections.Generic;
using UnityEngine;

public enum EnemyAiValue
{
    Invalid = -2,
    None = -1,
    First,
    Test = First,
    Max
}

public class EnemyAiValueObj
{
    public EnemyAiValue value;
    public AnimationCurve curve;
}

[CreateAssetMenu(fileName = "EnemyMovementData", menuName = "Enemies/AI/Enemy Movement Data")]
public class EnemyMovementData : ScriptableObject
{
    public Dictionary<EnemyAiValue, AnimationCurve> EnemyAiValueDic = new();

    [SerializeField]
    EnemyAiValueObj test;

    private void OnEnable()
    {
        EnemyAiValueDic.Clear();

        EnemyAiValueDic.Add(test.value, test.curve);
    }
}
