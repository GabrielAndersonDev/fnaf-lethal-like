using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_Door", menuName = "Map/MapSegments/Door")]
public class DoorData : MapSegmentData
{
    public bool isElectric;
    public bool isWorking;
    public bool isOpen;
}
