using UnityEngine;

public enum EnemyState
{
    Invalid = -2,
    None = -1,
    First,
    Search = First,
    Pursuit,
    Max
}

public class MovingEnemy : Enemy
{
    bool isStunned;

    [Header("Enemy Stats")]
    public int baseSpeed;
    public int baseStamina;

    [Header("Enemy Movement AI")]
    [SerializeField]
    EnemyMovementData movementAi;
    EnemyState enemyState;


}
