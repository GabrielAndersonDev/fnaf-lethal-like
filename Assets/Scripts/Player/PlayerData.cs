using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ScriptableObject 
{
    // From settings
    public string playerName;
    public ulong steamID;

    public Team team = Team.Player;

    public float baseHealth;
    public float baseMovementSpeed;
    public float baseStamina;
    public float baseSprintSpeed;

    public float groundDrag;
    public float jumpHeight;
    public float groundDistance;


    // From gameplay
    public bool isDead;
    public bool allowedToMove;
}
