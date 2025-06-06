using UnityEngine;

public class ItemNode : Node
{
    public ItemSpawnType spawnType;
    public bool isUsed;
    public bool isNone;

    public override void InitNode(MapManager map, MapSegment parentSeg)
    {
        base.InitNode(map, parentSeg);
        
        if (itemNodeData != null)
        {
            isUsed = itemNodeData.isUsed;
            isNone = itemNodeData.isNone;
        }
        else
        {
            Debug.LogError($"itemNodeData is {itemNodeData}");
            Debug.Break();
        }
    }
}
