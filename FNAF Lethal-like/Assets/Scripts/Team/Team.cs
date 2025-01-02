using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    PLAYER,
    ALLY,
    NEUTRAL,
    ENEMY
}

public static class TeamExtender
{
    public static string TeamString(this Team team)
    {
        return team switch
        {
            Team.PLAYER => "Player",
            Team.ALLY => "Ally",
            Team.NEUTRAL => "Passive",
            Team.ENEMY => "Hostile",
            _ => "Error",
        };
    }
}