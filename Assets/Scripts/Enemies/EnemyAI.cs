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
    PlayerChase,
    NoiseHeard, // on sound heard, louder sounds take priority over quiet ones (unless source of sound has been spotted after search? - this may only be on harder difficulties)
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
    public bool isPlayerNoticed;

    public Dictionary<EnemyState, float> enemyValueDic;

    public virtual void DetermineState()
    {
        int playerIndex = -1;
        int noiseIndex = -1;
        
        if (spottedPlayers.Count > 0)
        {
            playerIndex = CalculateSpottedPlayers();
            isPlayerNoticed = true;
        }
        else
        {
            enemyValueDic[EnemyState.PlayerChase] = 0f;
        }

        if (noisesHeard.Count > 0)
        {
            noiseIndex = CalculateChosenNoise();
        }
        else
        {
            enemyValueDic[EnemyState.NoiseHeard] = 0f;
        }

        EnemyState selectedState = EnemyState.None;
        selectedState = CalculateEnemyState();

        if (!isPlayerNoticed
            && enemyValueDic[EnemyState.NoiseHeard] > 0f)
        {

        }

        // this is just to make sure that actual hunting/searching patterns don't start until the first sign of a player. isPlayerNoticed should get toggled by players talking, visually seeing a player, or maybe things changed that only players could do? (locked doors opening?) potential for animatronics to communicate to each other somehow. may not implement, depends on how smart they are or harder difficulties?
        if (!isPlayerNoticed
            && enemyValueDic[EnemyState.NoiseHeard] != 0f)
        {
            selectedState = EnemyState.StartOfNight;
        }

        switch (enemyState)
        {
            case EnemyState.PlayerChase:
                enemyState = selectedState;
                targetPlayerData = spottedPlayers[playerIndex];
                break;
            case EnemyState.NoiseHeard:
                enemyState = selectedState;
                targetNoiseSource = noisesHeard[noiseIndex];
                break;
            case EnemyState.None:
                break;
            default:
                enemyState = selectedState;
                break;
        }
    }

    public virtual int CalculateSpottedPlayers()
    {
        float defaultWeight = enemyAIData.enemyAIWeight[EnemyState.PlayerChase];

        if (spottedPlayers.Count <= 0)
        {
            enemyValueDic[EnemyState.PlayerChase] = 0f;
            return -1;
        }

        if (spottedPlayers.Count == 1)
        {
            enemyValueDic[EnemyState.PlayerChase] = defaultWeight;
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
        float bestScore = 0f;

        foreach (KeyValuePair<int, float> pair in playerScores)
        {
            if (bestPlayerIndex == -1 
                || pair.Value > playerScores[bestPlayerIndex])
            {
                bestPlayerIndex = pair.Key;
                bestScore = pair.Value;
            }
        }

        if (bestPlayerIndex >= 0
            && spottedPlayers[bestPlayerIndex].player != null)
        {
            enemyValueDic[EnemyState.PlayerChase] = enemyAIData.enemyAICurveDic[EnemyState.PlayerChase].Evaluate(bestScore);
        }
        else
        {
            bestPlayerIndex = -1;
            enemyValueDic[EnemyState.PlayerChase] = 0f;
            Debug.LogWarning("bestPlayerIndex was not >= 0 or the selected player was null.");
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

    public virtual int CalculateChosenNoise()
    {
        List<int> noiseTies = new();
        noiseTies.Clear();

        int index = -1;
        float noiseVal = 0f;

        float playerSpeechBuff = 1.5f;

        for (int i = 0; i < noisesHeard.Count; i++)
        {
            GameObject noise = noisesHeard[i];

            if (noise == null)
            {
                Debug.LogWarning("Noise is null in noisesHeard.");
                continue;
            }

            float tempNoiseVal = 0f;
            // another one where we calculate travel time on top?
            float distVal = Vector3.Distance(transform.position, noise.transform.position);

            if (distVal > hearingRange)
            {
                distVal = hearingRange;
            }

            distVal = enemyAIData.noiseCurve.distance.Evaluate(distVal);

            float intensityVal = 0f;

            if (noise.TryGetComponent(out AudioController audio))
            {
                intensityVal = enemyAIData.noiseCurve.intensity.Evaluate(audio.intensity);
            }
            else
            {
                Debug.LogError("Noise does not contain AudioController.");
                Debug.Break();
            }

            tempNoiseVal += distVal;
            tempNoiseVal += intensityVal;

            if (noise.CompareTag("Speech"))
            {
                tempNoiseVal += playerSpeechBuff;
                isPlayerNoticed = true;
            }

            if (tempNoiseVal > noiseVal)
            {
                noiseVal = tempNoiseVal;
                index = i;
                noiseTies.Clear();
            }
            else if (tempNoiseVal == noiseVal)
            {
                noiseTies.Add(i);

                if (!noiseTies.Contains(index))
                {
                    noiseTies.Add(index);
                }
            }
        }

        if (noiseTies.Count > 0)
        {
            int selectedNoise = UnityEngine.Random.Range(0, noiseTies.Count - 1);
            index = noiseTies[selectedNoise];
        }

        enemyValueDic[EnemyState.NoiseHeard] = enemyAIData.enemyAICurveDic[EnemyState.NoiseHeard].Evaluate(noiseVal);

        return index;
    }

    public EnemyState CalculateEnemyState()
    {
        List<EnemyState> stateTies = new();
        stateTies.Clear();

        EnemyState enemyState = EnemyState.None;
        float value = 0f;

        foreach (EnemyState state in enemyValueDic.Keys)
        {
            if (enemyValueDic[state] > value)
            {
                enemyState = state;
                value = enemyValueDic[state];
                stateTies.Clear();
            }
            else if (enemyValueDic[state] == value)
            {
                stateTies.Add(state);

                if (!stateTies.Contains(enemyState))
                {
                    stateTies.Add(enemyState);
                }
            }
        }

        // until/if i decide to add a specific priority order, it will just be random which state gets picked in a tie.
        bool isNoiseHeard = false;

        if (stateTies.Count > 0)
        {
            if (stateTies.Count == 1)
            {
                Debug.LogError("StateTies.Count is 1.");
                Debug.Break();
            }

            for (int i = 0; i < stateTies.Count; i++)
            {
                switch (stateTies[i])
                {
                    case EnemyState.PlayerChase:
                        return EnemyState.PlayerChase;
                    case EnemyState.NoiseHeard:
                        isNoiseHeard = true;
                        break;
                    case EnemyState.Invalid:
                        Debug.LogError("EnemyState is invalid during state determination switch.");
                        Debug.Break();
                        break;
                    default:
                        break;
                }
            }

            if (isNoiseHeard)
            {
                return EnemyState.NoiseHeard;
            }

            int index = UnityEngine.Random.Range(0, stateTies.Count - 1);
            enemyState = stateTies[index];
        }

        return enemyState;
    }
}
