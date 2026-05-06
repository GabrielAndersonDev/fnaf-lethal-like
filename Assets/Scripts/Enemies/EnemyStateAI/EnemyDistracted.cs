using System.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    public virtual IEnumerator EnemyStateDistracted()
    {
        while (_state == EnemyState.Distracted)
        {

        }
        if (isAwareOfPlayers)
        {
            // enemy will now/is more likely to wander outside of spawned area
        }

        yield return null;
    }
}
