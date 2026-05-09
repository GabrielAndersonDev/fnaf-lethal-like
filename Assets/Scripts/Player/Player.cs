using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Steamworks;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using Assets.Scripts.Game;

public partial class Player : NetworkBehaviour
{
    [Header("Player Info")]
    public string playerName;
    public ulong steamID;
    public Team team;
    public bool isDead = false;

    [Header("Basic Stats")]
    public float baseHealth;
    public float currentHealth;

    public float baseMovementSpeed;

    public float baseStamina;
    public float currentStamina;

    public float baseSprintSpeed;

    public List<PlayerData> deadPlayerFollowers = new();

    public PlayerData playerData;

    [SerializeField]
    PlayerCam playerCam;

    public GameObject[] raycastNodes = new GameObject[5];

    public void PlayerInit()
    {
        playerData = Instantiate(playerData);

        playerName = NetworkScript.Singleton.localPlayerProfileData.playerName.ToString();
        steamID = NetworkScript.Singleton.localPlayerProfileData.steamID;

        InitKeyDictionary();

        // don't forget to change key assignment from being controlled by the PlayerData to the settings save when successfully implemented

        if (playerData != null)
        {
            playerData.playerName = playerName;
            playerData.steamID = steamID;
            team = playerData.team;

            baseHealth = playerData.baseHealth;
            currentHealth = playerData.baseHealth;

            baseMovementSpeed = playerData.baseMovementSpeed;

            baseStamina = playerData.baseStamina;
            currentStamina = playerData.baseStamina;

            baseSprintSpeed = playerData.baseSprintSpeed;

            isDead = playerData.isDead;
            allowedToMove = playerData.allowedToMove;

            groundDrag = playerData.groundDrag;
            jumpHeight = playerData.jumpHeight;
            groundDistance = playerData.groundDistance;

            isTogglePlayerList = GameManager.Singleton.playerSettings.isTogglePlayerList;

            whatIsGround = LayerMask.GetMask("whatIsGround");

            if (InputManager.Singleton == null
                || playerCam.GetComponent<Camera>() == null)
            {
                Debug.LogError("InputManager.Singleton is null in PlayerInit.");
                Debug.Break();
            }

            InputManager.Singleton.AssignPlayerInfo(this, playerCam.GetComponent<Camera>());

            if (UIManager.Singleton == null)
            {
                Debug.LogError("UIManager.Singleton is null in PlayerInit.");
                Debug.Break();
            }

            UIManager.Singleton.AssignPlayerToUI(this);

            AssignInputActions();
            ToggleActionEvents(true);
            Debug.Log(moveAction + " is enabled at end of PlayerInit? " + moveAction.enabled);
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

        if (GameManager.Singleton.playerSettings.keyArray != null)
        {
            foreach (KeyCodeObj obj in GameManager.Singleton.playerSettings.keyArray)
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

