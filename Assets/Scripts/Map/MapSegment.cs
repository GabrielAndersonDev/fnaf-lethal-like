using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SegmentType
{
    Invalid = -2,
    None = -1,
    First,
    MainRoom = First,
    Entrance,
    Hallway,
    Max
}
public class MapSegment : MonoBehaviour
{
    public MapSegmentData mapSegmentData;
    public string segmentName;
    public SegmentType segmentType;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
