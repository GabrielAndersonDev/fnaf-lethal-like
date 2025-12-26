using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    public virtual void EnemyStand()
    {
        Debug.Log("The enemy is doing nothing.");
    }

    public virtual void EnemyTurn()
    {
        Debug.Log("The enemy is turning.");
    }

    public virtual void EnemyLookAround()
    {
        Debug.Log("The enemy is looking around.");
    }

    public virtual void EnemyInteract()
    {
        Debug.Log("The enemy is interacting.");
    }

    public virtual void EnemyStunned()
    {
        Debug.Log("The enemy is stunned.");
    }

    public virtual void EnemyDeactivated()
    {
        Debug.Log("The enemy is deactivated.");
    }
}
