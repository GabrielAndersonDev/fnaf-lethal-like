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
        DefaultState = EnemyState.Wandering;
        enemyAction = EnemyAction.Stand;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void PlayerSpotted(Player player)
    {
        base.PlayerSpotted(player);
    }
}
