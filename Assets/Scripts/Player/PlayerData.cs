using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ScriptableObject 
{
    public string playerName;
    public int playerNumber;
    public Team team;

    public float baseHealth;
    public float baseMovementSpeed;
    public float baseStamina;

    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode useKey;
    public KeyCode attackKey;
    public KeyCode interactKey;
    public KeyCode dropKey;
    public KeyCode alternateKey;
    public KeyCode lightKey;
    public KeyCode pauseKey;
    public KeyCode inventorySlotOne;
    public KeyCode inventorySlotTwo;
    public KeyCode inventorySlotThree;
    public KeyCode inventorySlotFour;

    public float groundDrag;
    public float jumpHeight;
    public bool allowed_to_move;

    public LayerMask whatIsGround;
    public float groundDistance;
}
