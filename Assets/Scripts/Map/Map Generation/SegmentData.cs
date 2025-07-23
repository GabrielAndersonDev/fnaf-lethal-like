using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SegValueObj
{
    public MapSegmentType segType;
    public SegmentValueData segValue;
}

[System.Serializable]
public class SegDataObj
{
    public MapSegmentType segType;
    public MapSegmentData segData;
}

[System.Serializable]
public class RoomPrefabObj
{
    public RoomType roomType;
    public GameObject roomPrefab;
}

[CreateAssetMenu(fileName = "SegmentDataCollection", menuName = "Map/Data/SegmentDataCollection")]
public class SegmentData : ScriptableObject
{
    public List<SegValueObj> valueObjList = new();
    public List<SegDataObj> dataObjList = new();
    public List<RoomPrefabObj> roomObjList = new();

    public Dictionary<MapSegmentType, SegmentValueData> segValueDic;
    public Dictionary<MapSegmentType, MapSegmentData> segDataDic;
    public Dictionary<RoomType, GameObject> roomPrefabDic;

    private void OnEnable()
    {
        segValueDic = new Dictionary<MapSegmentType, SegmentValueData>();
        segDataDic = new Dictionary<MapSegmentType, MapSegmentData>();
        roomPrefabDic = new Dictionary<RoomType, GameObject>();

        foreach (SegValueObj valueObj in valueObjList)
        {
            if (!segValueDic.ContainsKey(valueObj.segType))
            {
                segValueDic.Add(valueObj.segType, valueObj.segValue);
            }
        }

        foreach (SegDataObj dataObj in dataObjList)
        {
            if (!segDataDic.ContainsKey(dataObj.segType))
            {
                segDataDic.Add(dataObj.segType, dataObj.segData);
            }
        }

        foreach (RoomPrefabObj roomObj in roomObjList)
        {
            if (!roomPrefabDic.ContainsKey(roomObj.roomType))
            {
                roomPrefabDic.Add(roomObj.roomType, roomObj.roomPrefab);
            }
        }
    }
}
