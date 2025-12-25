using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    // stand, move, attack, turn, look around, interact, stunned, deactivated
    // attack only when player spotted and in atk range
    // amount of time since last look around action affects look around rate
    // turn is turn in place

    public EnemyAction DetermineAction(EnemyState state)
    {
        
        return EnemyAction.Invalid;
    }

    public float StandCalc(EnemyState state)
    {
        if (enemyAIDataRef.enemyActionDictionary.TryGetValue(state, out var actionDict))
        {
            if (actionDict.TryGetValue(EnemyAction.Stand, out var rate))
            {
                return rate;
            }
        }
        return 0f;
    }
}
