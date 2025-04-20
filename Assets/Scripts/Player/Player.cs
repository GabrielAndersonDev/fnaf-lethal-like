using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public partial class Player : MonoBehaviour
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

    Transform cameraPos;
    Transform cameraOrientation;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        itemManager = GameObject.FindObjectOfType<ItemManager>();

        InventoryInit();
    }

    private void Update()
    {
        // ground check
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);

        PlayerInput();

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void PlayerInit(PlayerData data, int playerListNumber, Camera camera)
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

            playerCamera = camera;

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

    public void CameraInit(GameObject newCamera, GameObject newPlayer)
    {
        MoveCamera moveCam = newCamera.GetComponent<MoveCamera>();
        PlayerCam playerCam = newCamera.GetComponentInChildren<PlayerCam>();

        if ( moveCam != null && playerCam != null)
        {
            CheckCameraTransform(newPlayer);

            if (cameraPos == null || cameraOrientation == null)
            {
                Debug.LogError($"cameraPos is {cameraPos}, cameraOrientation is {cameraOrientation}");
                Debug.Break();
            }

            moveCam.cameraPosition = cameraPos;
            playerCam.orientation = cameraOrientation;
        }
        else
        {
            Debug.LogError("Error with moveCam and playerCam assignment.");
            Debug.Break();
        }
    }

    public void CheckCameraTransform(GameObject newPlayer)
    {
        foreach (Transform transform in newPlayer.GetComponentsInChildren<Transform>())
        {
            if (transform.CompareTag("CameraPos"))
            {
                cameraPos = transform;
            }
            if (transform.CompareTag("Orientation"))
            {
                cameraOrientation = transform;
            }
        }
    }
}

