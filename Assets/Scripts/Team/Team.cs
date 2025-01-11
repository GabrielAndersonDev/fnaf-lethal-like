using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Invalid = -2,
    None = -1,
    First,
    Player = First,
    Ally,
    Neutral,
    Enemy,
    Max
}

public static class TeamExtender
{
    public static string TeamString(this Team team)
    {
        return team switch
        {
            Team.Player => "Player",
            Team.Ally => "Ally",
            Team.Neutral => "Passive",
            Team.Enemy => "Hostile",
            _ => "Error",
        };
    }
}