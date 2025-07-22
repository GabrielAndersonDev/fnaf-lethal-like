using UnityEngine;

public enum  RoomType
{
    Invalid = -2,
    None = -1,
    First,
    Party = First,
    Space,
    Fantasy,
    Mine,
    Casino,
    Max
}
public class RoomSegment : MapSegment
{
    public RoomType roomType;
}
