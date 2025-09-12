using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Steamworks;

public partial class Player : NetworkBehaviour
{
    [Header("Player Info")]
    public string playerName;
    public Team team;

    [Header("Basic Stats")]
    public float baseHealth;
    public float baseMovementSpeed;
    public float baseStamina;

    public float currentHealth;

    public PlayerData playerData;

    [SerializeField]
    PlayerCam playerCam;

    public void PlayerInit(PlayerData data)
    {
        playerData = data;
        playerName = NetworkScript.Singleton.localPlayerProfileData.playerName.ToString();

        // don't forget to change key assignment from being controlled by the PlayerData to the settings save when successfully implemented

        if (playerData != null)
        {
            // This is temporary until I add either Steam name compatibility or having players choose their name
            // May add a more specific player ID along with player number. Will have to do more research on multiplayer.

            data.playerName = playerName;
            team = data.team;

            baseHealth = data.baseHealth;
            baseMovementSpeed = data.baseMovementSpeed;
            baseStamina = data.baseStamina;

            forwardKey = data.forwardKey;
            backwardKey = data.backwardKey;
            leftKey = data.leftKey;
            rightKey = data.rightKey;
            jumpKey = data.jumpKey;
            useKey = data.useKey;
            attackKey = data.attackKey;
            interactKey = data.interactKey;
            dropKey = data.dropKey;
            alternateKey = data.alternateKey;
            lightKey = data.lightKey;
            pauseKey = data.pauseKey;
            inventorySlotOne = data.inventorySlotOne;
            inventorySlotTwo = data.inventorySlotTwo;
            inventorySlotThree = data.inventorySlotThree;
            inventorySlotFour = data.inventorySlotFour;
            playerListKey = data.playerListKey;

            groundDrag = data.groundDrag;
            jumpHeight = data.jumpHeight;
            allowed_to_move = data.allowed_to_move;

            whatIsGround = data.whatIsGround;
            groundDistance = data.groundDistance;

            isTogglePlayerList = data.isTogglePlayerList;
        }
        else
        {
            Debug.LogError($"playerData is {playerData}");
            Debug.Break();
        }
    }
}

