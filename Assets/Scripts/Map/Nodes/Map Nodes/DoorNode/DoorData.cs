using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_doorNode", menuName = "Nodes/Map/DoorNode")]
public class DoorData : MapNodeData
{
    public bool isWorking;
    public bool isOpen;
}
