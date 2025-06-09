using UnityEngine;

public class ItemNode : Node
{
    public ItemSpawnType[] itemSpawnTypes;
    public bool isUsed;
    public bool isNone;

    public override void InitNode(MapManager map, MapSegment parentSeg)
    {
        base.InitNode(map, parentSeg);
    }
}
