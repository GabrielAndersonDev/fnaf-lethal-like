using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ScriptableObject 
{
    // From settings
    public string playerName;

    public Team team = Team.Player;

    public float baseHealth;
    public float baseMovementSpeed;
    public float baseStamina;

    // From gameplay
    public bool isDead;
    public bool allowedToMove;
    public float currentHealth;
}
