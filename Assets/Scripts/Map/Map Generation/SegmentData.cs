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

[CreateAssetMenu(fileName = "SegmentDataCollection", menuName = "Map/Data/SegmentDataCollection")]
public class SegmentData : ScriptableObject
{
    public List<SegValueObj> valueObjList = new();
    public List<SegDataObj> dataObjList = new();

    public Dictionary<MapSegmentType, SegmentValueData> segValueDic;
    public Dictionary<MapSegmentType, MapSegmentData> segDataDic;

    private void OnEnable()
    {
        segValueDic = new Dictionary<MapSegmentType, SegmentValueData>();
        segDataDic = new Dictionary<MapSegmentType, MapSegmentData>();

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
    }
}
