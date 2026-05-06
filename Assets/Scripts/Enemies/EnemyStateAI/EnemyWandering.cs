using System.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    public virtual IEnumerator EnemyStateWandering()
    {
        while (_state == EnemyState.Wandering)
        {

        }
        if (isAwareOfPlayers)
        {
            // enemy will now/is more likely to wander outside of spawned area
        }

        yield return null;
    }
}
