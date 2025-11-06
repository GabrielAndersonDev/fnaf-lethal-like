using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovingEnemy : Enemy
{
    [Header("Enemy Stats")]
    public int baseSpeed;

    public override void OnNetworkSpawn()
    {
        enemyState = EnemyState.StartOfNight;
        enemyAction = EnemyAction.Stand;
    }

    public override void Update()
    {
        base.Update();
        EnemyStateCheck();
    }

    public override void EnemyMove()
    {
        base.EnemyMove();
    }

    private void EnemyStateCheck()
    {
        // Implement state checking logic here
    }

    public override void PlayerSpotted(Player player)
    {
        base.PlayerSpotted(player);
    }
}
