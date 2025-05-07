using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public partial class Player : NetworkBehaviour
{
    [Header("Player Info")]
    public string playerName;
    public int playerNumber;
    public Team team;

    [Header("Basic Stats")]
    public float baseHealth;
    public float baseMovementSpeed;
    public float baseStamina;

    public PlayerData playerData;
    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    ItemManager itemManager;
    
    private void Start()
    {
        // see if we switch these to rpc OnNetworkSpawn??
        rb.freezeRotation = true;
        itemManager = GameObject.FindObjectOfType<ItemManager>();
        PlayerInit(playerData, 1);

        InventoryInit();
    }

    private void Update()
    {
        // ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);

        // PlayerInput();

        // handle drag
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    public void PlayerInit(PlayerData data, int playerListNumber)
    {
        playerData = data;

        if (playerData != null)
        {
            // This is temporary until I add either Steam name compatibility or having players choose their name
            // May add a more specific player ID along with player number. Will have to do more research on multiplayer.
            playerName = data.playerName;

            playerNumber = playerListNumber;
            team = data.team;

            baseHealth = data.baseHealth;
            baseMovementSpeed = data.baseMovementSpeed;
            baseStamina = data.baseStamina;

            playerPrefab = data.playerPrefab;
            cameraPrefab = data.cameraPrefab;

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

            groundDrag = data.groundDrag;
            jumpHeight = data.jumpHeight;
            allowed_to_move = data.allowed_to_move;

            whatIsGround = data.whatIsGround;
            groundDistance = data.groundDistance;
        }
        else
        {
            Debug.LogError($"playerData is {playerData}");
            Debug.Break();
        }
    }
}

