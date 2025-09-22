using NUnit.Framework;
using UnityEngine;

public class EntranceSegment : MapSegment
{
    public PlayerSpawnNode[] playerSpawnNodes;

    // Whether the entrance is open for players to enter or exit
    public bool isOpen;
}
