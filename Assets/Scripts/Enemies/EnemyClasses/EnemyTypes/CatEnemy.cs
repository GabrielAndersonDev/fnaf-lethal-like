using System.Collections.Generic;
using UnityEngine;

public class CatEnemy : MovingEnemy
{
    public override void InitializeEnemy(EnemyData data)
    {
        base.InitializeEnemy(data);
        canSee = true;
    }
}
