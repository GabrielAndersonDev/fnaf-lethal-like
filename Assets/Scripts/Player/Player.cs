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
    public bool isDead = false;

    [Header("Basic Stats")]
    public float baseHealth;
    public float baseMovementSpeed;
    public float baseStamina;

    public float currentHealth;

    public PlayerData playerData;

    [SerializeField]
    PlayerCam playerCam;

    public void PlayerInit()
    {
        SaveManager saveManager = SaveManager.Singleton;

        RecallSavedInventory();

        playerData = Instantiate(playerData);

        playerName = NetworkScript.Singleton.localPlayerProfileData.playerName.ToString();

        // don't forget to change key assignment from being controlled by the PlayerData to the settings save when successfully implemented

        if (playerData != null)
        {
            // This is temporary until I add either Steam name compatibility or having players choose their name
            // May add a more specific player ID along with player number. Will have to do more research on multiplayer.

            playerData.playerName = playerName;
            team = playerData.team;

            isDead = playerData.isDead;
            allowedToMove = playerData.allowedToMove;
            currentHealth = playerData.currentHealth;

            forwardKey = saveManager.savedPlayerSettings.forwardKey;
            backwardKey = saveManager.savedPlayerSettings.backwardKey;
            leftKey = saveManager.savedPlayerSettings.leftKey;
            rightKey = saveManager.savedPlayerSettings.rightKey;
            jumpKey = saveManager.savedPlayerSettings.jumpKey;
            useKey = saveManager.savedPlayerSettings.useKey;
            attackKey = saveManager.savedPlayerSettings.attackKey;
            interactKey = saveManager.savedPlayerSettings.interactKey;
            dropKey = saveManager.savedPlayerSettings.dropKey;
            alternateKey = saveManager.savedPlayerSettings.alternateKey;
            lightKey = saveManager.savedPlayerSettings.lightKey;
            pauseKey = saveManager.savedPlayerSettings.pauseKey;
            inventorySlotOne = saveManager.savedPlayerSettings.inventorySlotOne;
            inventorySlotTwo = saveManager.savedPlayerSettings.inventorySlotTwo;
            inventorySlotThree = saveManager.savedPlayerSettings.inventorySlotThree;
            inventorySlotFour = saveManager.savedPlayerSettings.inventorySlotFour;
            playerListKey = saveManager.savedPlayerSettings.playerListKey;

            whatIsGround = LayerMask.GetMask("whatIsGround");

            UIManager.Singleton.InitUI(this);
        }
        else
        {
            Debug.LogError($"playerData is {playerData}");
            Debug.Break();
        }
    }
}

