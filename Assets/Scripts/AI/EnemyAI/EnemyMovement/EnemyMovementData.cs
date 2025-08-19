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

public class EnemyMovementData : MonoBehaviour
{
    
}
