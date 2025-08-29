using UnityEngine;

public class ItemNode : Node
{
    public ItemSpawnType[] itemSpawnTypes;
    public bool isUsed;
    public bool isNone;

    public override void InitNode(MapSegment parentSeg)
    {
        base.InitNode(parentSeg);
    }
}
