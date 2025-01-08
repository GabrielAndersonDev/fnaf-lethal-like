using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeactivateType
{
    // WIP on names/cases. may add more methods or change them around
    MINIGAME,
    EXTERNAL,
    ITEM
}

public abstract class DeactivateMethod : ScriptableObject
{
    public Enemy enemy;

    public DeactivateType deactivateType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // again, will add more when i figure out wtf i'm doing

    public virtual void BeginMinigame(Enemy enemy, Player player)
    {

    }
}
