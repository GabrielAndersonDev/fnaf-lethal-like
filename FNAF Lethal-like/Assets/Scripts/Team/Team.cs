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
        switch (team)
        {
            case Team.PLAYER:
                return "Player";
            case Team.ALLY:
                return "Ally";
            case Team.NEUTRAL:
                return "Passive";
            case Team.ENEMY:
                return "Hostile";
            default:
                return "Error";
        }
    }
}