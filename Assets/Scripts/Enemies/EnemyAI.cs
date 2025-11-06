using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum EnemyState
{
    Invalid = -2,
    None = -1,
    First,
    Default = First,
    StartOfNight,
    PlayerSpotted,
    SoundHeard, // on sound heard, louder sounds take priority over quiet ones (unless source of sound has been spotted after search? - this may only be on harder difficulties)
    Distracted,  //this is for laser pointer on cat or ball on dog, for example
    Disabled,
    Max
}

public enum EnemyVisionState
{
    Invalid = -2,
    None = -1,
    First,
    InRange = First,
    InVision,
    Chased
}

public partial class Enemy : NetworkBehaviour
{
    [Header("AI")]
    public EnemyState enemyState;
    public EnemyAction enemyAction;
    public EnemyAIData enemyAIData;

    public virtual void DetermineState()
    {
        if (spottedPlayers.Count > 0)
        {
            int playerIndex = CalculateSpottedPlayers();
        }
    }

    public virtual int CalculateSpottedPlayers()
    {
        if (spottedPlayers.Count == 1)
        {
            return spottedPlayers[0].index;
        }
        Dictionary<int, float> playerScores = new();

        foreach (EnemyPlayerData playerData in spottedPlayers)
        {
            float score = 0f;

            if (playerData.isInRange)
            {
                score += enemyAIData.enemyVisionWeight[EnemyVisionState.InRange];
            }

            if (playerData.isInVision)
            {
                score += enemyAIData.enemyVisionWeight[EnemyVisionState.InVision];
            }

            if (playerData.isChased)
            {
                score += enemyAIData.enemyVisionWeight[EnemyVisionState.Chased];
            }

            score += CalculatePlayerDistance(playerData);

            if (!playerScores.ContainsKey(playerData.index))
            {
                playerScores.Add(playerData.index, score);
            }
        }

        int bestPlayerIndex = -1;

        foreach (KeyValuePair<int, float> pair in playerScores)
        {
            if (bestPlayerIndex == -1 
                || pair.Value > playerScores[bestPlayerIndex])
            {
                bestPlayerIndex = pair.Key;
            }
        }

        return bestPlayerIndex;
    }

    public float CalculatePlayerDistance(EnemyPlayerData playerData)
    {
        // change to use walking distance later to ensure obstacles are considered
        float score;
        float distance = Vector3.Distance(transform.position, playerData.player.transform.position);
        score = enemyAIData.distanceFromPlayerCurve.Evaluate(distance);

        return score;
    }
}
