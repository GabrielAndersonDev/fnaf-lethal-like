using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum EnemyState
{
    Invalid = -2,
    None = -1,
    First,
    Wandering = First,
    Chasing,
    Searching,
    NoiseHeard, // on sound heard, louder sounds take priority over quiet ones (unless source of sound has been spotted after search? - this may only be on harder difficulties)
    Distracted,  //this is for laser pointer on cat or ball on dog, for example
    Interacting,
    Deactivated,
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
    public EnemyAction enemyAction;
    public EnemyAIData enemyAIData;
    private Coroutine EnemyCoroutine;
    private bool isDistractionActive = false;
    private bool isInteractablesAround = false;

    [Header("Player Tracking")]
    private Dictionary<Player, EnemyPlayerData> playerToPlayerDataDictionary;
    [SerializeField]
    private List<Player> spottedPlayers;
    private int totalPlayers;
    public bool isAwareOfPlayers;
    private float targetPlayerMovementDirection;

    public EnemyState DefaultState;
    [SerializeField]
    private EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    public delegate void OnStateChangeEvent(EnemyState previousState, EnemyState newState);
    public event OnStateChangeEvent OnStateChange;

    [SerializeField]
    private EnemyPlayerData _targetPlayerData;
    public EnemyPlayerData TargetPlayerData
    {
        get
        {
            return _targetPlayerData;
        }
        set
        {
            OnPlayerTargetChange?.Invoke(_targetPlayerData, value);
            _targetPlayerData = value;
        }
    }

    public delegate void OnPlayerTargetChangeEvent(EnemyPlayerData previousTarget, EnemyPlayerData newTarget);
    public event OnPlayerTargetChangeEvent OnPlayerTargetChange;

    private void OnDisable()
    {
        _state = DefaultState;
    }

    // state should generally change less
    public IEnumerator DetermineStateCoroutine()
    {
        while (true)
        {
            int noiseIndex = -1;
            Player player = null;

            if (spottedPlayers.Count > 0)
            {
                yield return player = CalculateSpottedPlayers();

                if (player != null)
                {
                    isAwareOfPlayers = true;
                }
            }
            else
            {
                enemyAIData.enemyAIRates[EnemyState.Chasing] = 0f;

                if (_state == EnemyState.Searching
                    || _state == EnemyState.Chasing)
                {
                    enemyAIData.enemyAIRates[EnemyState.Searching] = 10f;
                }
            }

            if (noisesHeard.Count > 0)
            {
                yield return noiseIndex = CalculateChosenNoise();
            }
            else
            {
                enemyAIData.enemyAIRates[EnemyState.NoiseHeard] = 0f;

                if (_state == EnemyState.Searching
                    || _state == EnemyState.Chasing)
                {
                    enemyAIData.enemyAIRates[EnemyState.Searching] = 10f;
                }
            }


            // distraction handling incomplete. add logic to set isDistractionActive true/false based on distraction object state
            if (isDistractionActive == true)
            {
                enemyAIData.enemyAIRates[EnemyState.Distracted] = enemyAIData.enemyAIWeight[EnemyState.Distracted];
            }
            else
            {
                enemyAIData.enemyAIRates[EnemyState.Distracted] = 0f;
            }

            if (isInteractablesAround == true)
            {
                enemyAIData.enemyAIRates[EnemyState.Interacting] = enemyAIData.enemyAIWeight[EnemyState.Interacting];
            }
            else
            {
                enemyAIData.enemyAIRates[EnemyState.Interacting] = 0f;
            }

            EnemyState selectedState;

            yield return selectedState = CalculateEnemyState();

            // this is just to make sure that actual hunting/searching patterns don't start until the first sign of a player. isAwareOfPlayers should get toggled by players talking, visually seeing a player, or maybe things changed that only players could do? (locked doors opening?) potential for animatronics to communicate to each other somehow. may not implement, depends on how smart they are or harder difficulties?
            if (!isAwareOfPlayers
                && enemyAIData.enemyAIRates[EnemyState.NoiseHeard] == 0f)
            {
                selectedState = EnemyState.Wandering;
            }

            if (selectedState != _state)
            {
                switch (selectedState)
                {
                    case EnemyState.Chasing:
                        if (_targetPlayerData.player != player
                            && player != null)
                        {
                            TargetPlayerData = playerToPlayerDataDictionary[player];

                            if (TrackPlayerDirectionCoroutine != null)
                            {
                                StopCoroutine(TrackPlayerDirectionCoroutine);
                            }

                            TrackPlayerDirectionCoroutine = StartCoroutine(TrackPlayerDirection(player));
                        }

                        // may change to track any visible player, rather than just the chased player.

                        State = selectedState;
                        break;
                    case EnemyState.NoiseHeard:
                        targetNoiseSource = noisesHeard[noiseIndex];
                        State = selectedState;
                        break;
                    default:
                        State = selectedState;
                        break;
                }
            }
            else if (selectedState == EnemyState.Chasing
                && _targetPlayerData.player != player
                && player != null)
            {
                TargetPlayerData = playerToPlayerDataDictionary[player];

                if (TrackPlayerDirectionCoroutine != null)
                {
                    StopCoroutine(TrackPlayerDirectionCoroutine);
                }

                TrackPlayerDirectionCoroutine = StartCoroutine(TrackPlayerDirection(player));
            }
            yield return _waitForSeconds0_2;
        }
    }

    public virtual Player CalculateSpottedPlayers()
    {
        if (spottedPlayers.Count <= 0)
        {
            enemyAIData.enemyAIRates[EnemyState.Chasing] = 0f;
            return null;
        }

        Dictionary<Player, float> playerScores = new();

        foreach (Player player in spottedPlayers)
        {
            float score = 0f;
            EnemyPlayerData playerData = playerToPlayerDataDictionary[player];

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

            if (!playerScores.ContainsKey(player))
            {
                playerScores.Add(player, score);
            }
        }

        Player bestPlayer = null;
        float bestScore = 0f;

        foreach (KeyValuePair<Player, float> pair in playerScores)
        {
            if (bestPlayer == null
                || pair.Value > playerScores[bestPlayer])
            {
                bestPlayer = pair.Key;
                bestScore = pair.Value;
            }
        }

        if (bestPlayer != null)
        {
            // to alter enemy bias, change either weight list OR distanceFromPlayerCurve
            bestScore *= enemyAIData.enemyAIWeight[EnemyState.Chasing];
            enemyAIData.enemyAIRates[EnemyState.Chasing] = bestScore;
        }
        else
        {
            enemyAIData.enemyAIRates[EnemyState.Chasing] = 0f;
            Debug.LogWarning("bestPlayerIndex was not >= 0 or the selected player was null.");
            return null;
        }

        return bestPlayer;
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
                isAwareOfPlayers = true;
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

        noiseVal *= enemyAIData.enemyAIWeight[EnemyState.NoiseHeard];
        enemyAIData.enemyAIRates[EnemyState.NoiseHeard] = noiseVal;

        return index;
    }

    public EnemyState CalculateEnemyState()
    {
        List<EnemyState> stateTies = new();
        stateTies.Clear();

        EnemyState enemyState = EnemyState.None;
        float value = 0f;

        foreach (EnemyState state in enemyAIData.enemyAIRates.Keys)
        {
            if (enemyAIData.enemyAIRates[state] > value)
            {
                enemyState = state;
                value = enemyAIData.enemyAIRates[state];
                stateTies.Clear();
            }
            else if (enemyAIData.enemyAIRates[state] == value)
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
                    case EnemyState.Chasing:
                        return EnemyState.Chasing;
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

            if (_state == EnemyState.Searching)
            {
                return EnemyState.Searching;
            }

            int index = UnityEngine.Random.Range(0, stateTies.Count - 1);
            enemyState = stateTies[index];
        }

        return enemyState;
    }

    private void HandleStateChange(EnemyState previousState, EnemyState newState)
    {
        if (previousState != newState)
        {
            Debug.Log($"Enemy {enemyID} changing state from {previousState} to {newState}.");

            if (EnemyCoroutine != null)
            {
                StopCoroutine(EnemyCoroutine);
            }

            if (newState != EnemyState.Chasing)
            {
                StopChasingCoroutines();
            }

            if (newState != EnemyState.Searching
                && SearchLastKnownCoroutine != null)
            {
                StopCoroutine(SearchLastKnownCoroutine);
            }

            switch (newState)
            {
                case EnemyState.Wandering:
                    Debug.Log("Starting wandering coroutine.");
                    EnemyCoroutine = StartCoroutine(EnemyStateWandering());
                    break;
                case EnemyState.Chasing:
                    Debug.Log("Starting Chasing Coroutine.");
                    EnemyCoroutine = StartCoroutine(EnemyStateChasing());
                    break;
                case EnemyState.Searching:
                    Debug.Log("Starting searching coroutine.");
                    EnemyCoroutine = StartCoroutine(EnemyStateSearching());
                    break;
                case EnemyState.NoiseHeard:
                    EnemyCoroutine = StartCoroutine(EnemyStateNoiseHeard());
                    break;
                case EnemyState.Distracted:
                    EnemyCoroutine = StartCoroutine(EnemyStateDistracted());
                    break;
                case EnemyState.Interacting:
                    EnemyCoroutine = StartCoroutine(EnemyStateInteracting());
                    break;
                case EnemyState.Deactivated:
                    EnemyCoroutine = StartCoroutine(EnemyStateDeactivated());
                    break;
                default:
                    Debug.LogWarning("Unhandled enemy state change.");
                    Debug.Break();
                    break;
            }
        }
    }
}