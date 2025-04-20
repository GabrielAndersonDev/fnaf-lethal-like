using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SegmentDataCollection", menuName = "Map/SegmentDataCollection")]
public class SegmentData : ScriptableObject
{
    public MapSegmentData entrance;
    public SegmentValueData entranceValue;

    public MapSegmentData room;
    public SegmentValueData roomValue;

    public MapSegmentData hallway;
    public SegmentValueData hallwayValue;

    public MapSegmentData door;
    public SegmentValueData doorValue;

    public MapSegmentData staff;
    public SegmentValueData staffValue;

    public MapSegmentData bathroom;
    public SegmentValueData bathroomValue;

    public SegmentValueData none;
}
