using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeManager : MonoBehaviour
{
    List<MapNode> nodes;
    List<MapSegment> segments;
    public void MapNodeInitialize()
    {
        segments = new List<MapSegment>();
        nodes = new List<MapNode>();


    }
    // MAY NOT NEED THIS! DEPENDS IF MAP MANAGER CAN DO IT ALL
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
