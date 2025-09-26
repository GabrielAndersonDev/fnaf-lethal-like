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
    public ulong steamID;
    public Team team;
    public bool isDead = false;

    [Header("Basic Stats")]
    public float baseHealth;
    public float baseMovementSpeed;
    public float baseStamina;
    public float baseSprintSpeed;

    public float currentHealth;
    public List<PlayerData> deadPlayerFollowers = new();

    public PlayerData playerData;

    [SerializeField]
    PlayerCam playerCam;

    public void PlayerInit()
    {
        SaveManager saveManager = SaveManager.Singleton;

        RecallSavedInventory();

        playerData = Instantiate(playerData);

        playerName = NetworkScript.Singleton.localPlayerProfileData.playerName.ToString();
        steamID = NetworkScript.Singleton.localPlayerProfileData.steamID;

        InitKeyDictionary();

        // don't forget to change key assignment from being controlled by the PlayerData to the settings save when successfully implemented

        if (playerData != null)
        {
            playerData.playerName = playerName;
            steamID = playerData.steamID;
            team = playerData.team;

            baseHealth = playerData.baseHealth;
            baseMovementSpeed = playerData.baseMovementSpeed;
            baseStamina = playerData.baseStamina;
            baseSprintSpeed = playerData.baseSprintSpeed;
            currentHealth = playerData.currentHealth;

            isDead = playerData.isDead;
            allowedToMove = playerData.allowedToMove;
            currentHealth = playerData.currentHealth;

            groundDrag = playerData.groundDrag;
            jumpHeight = playerData.jumpHeight;
            groundDistance = playerData.groundDistance;

            isTogglePlayerList = saveManager.savedPlayerSettings.isTogglePlayerList;

            whatIsGround = LayerMask.GetMask("whatIsGround");

            UIManager.Singleton.AssignPlayerToUI(this);
        }
        else
        {
            Debug.LogError($"playerData is {playerData}");
            Debug.Break();
        }
    }

    void InitKeyDictionary()
    {
        keyDictionary = new();
        keyDictionary.Clear();

        if (SaveManager.Singleton.savedPlayerSettings.keyArray != null)
        {
            foreach (KeyCodeObj obj in SaveManager.Singleton.savedPlayerSettings.keyArray)
            {
                if (keyDictionary.ContainsKey(obj.name))
                {
                    Debug.Log("Dictionary already contains " +  obj.name);
                    continue;
                }
                
                keyDictionary.Add(obj.name, obj.key);
            }
        }
        else
        {
            Debug.LogError("Key array in saved player settings is null.");
            Debug.Break();
        }
    }

    public void KeyDictionaryUpdate(KeyCodeObj keyObj)
    {
        if (keyObj.name != null
            && keyDictionary.ContainsKey(keyObj.name))
        {
            keyDictionary[keyObj.name] = keyObj.key;
        }
        else
        {
            Debug.Log("Key dictionary does not contain key " + keyObj.name);
            Debug.Break();
        }
    }
}

