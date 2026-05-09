using Steamworks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NetworkUIScript : NetworkBehaviour
{
    public static NetworkUIScript Singleton { get; internal set; }

    [SerializeField]
    GameObject networkSceneObj;
    VisualElement networkScene;

    [SerializeField]
    GameObject playerMenuObj;
    VisualElement playerMenu;

    [SerializeField]
    VisualTreeAsset playerTemplate;

    bool useRandomSeed = true;
    bool isPlayerListOpen = false;
    bool isNetworkScene = true;

    Box playerSceneContainer;
    Box playerMenuContainer;

    Button backBtn;

    [SerializeField]
    Texture2D defaultAvatar;

    Player localPlayer;

    // make this a dropdown within individual player settings later
    public PlayerPrefabType localPlayerType = PlayerPrefabType.Basic;

    Dictionary<PlayerProfileData, PlayerTemplate> connectedPlayerDic;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void InitNetworkMenu()
    {
        if (GameManager.Singleton != null)
        {
            connectedPlayerDic = new();
            connectedPlayerDic.Clear();

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            networkScene = networkSceneObj.GetComponent<UIDocument>().rootVisualElement;
            playerMenu = playerMenuObj.GetComponent<UIDocument>().rootVisualElement;

            playerSceneContainer = networkScene.Q<Box>("player-container");
            playerMenuContainer = playerMenu.Q<Box>("player-container");

            Button startBtn = networkScene.Q<Button>("start-btn");
            Button useRandBtn = networkScene.Q<Button>("use-rand-btn");
            backBtn = networkScene.Q<Button>("back-btn");

            useRandBtn.text = useRandomSeed ? "True" : "False";

            SetNetworkDisplay();

            startBtn.clicked += OnStartClicked;
            useRandBtn.clicked += OnRandClicked;
            backBtn.clicked += OnBackBtnClicked;

            if (NetworkManager.Singleton.IsHost)
            {
                InitPlayerDic();
            }
        }
        else
        {
            Debug.LogError("GameManager.Instance is null");
            Debug.Break();
        }
    }

    public void PlayerListSceneCheck()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "NetworkMenu":
                InitNetworkMenu();
                networkScene.style.display = DisplayStyle.Flex;
                playerMenu.style.display = DisplayStyle.None;
                isNetworkScene = true;
                break;
            case "MainMenu":
                networkScene.style.display = DisplayStyle.None;
                playerMenu.style.display = DisplayStyle.None;
                isNetworkScene = false;
                isPlayerListOpen = false;
                break;
            case "VanScene":
                networkScene.style.display = DisplayStyle.None;
                isNetworkScene = false;
                break;
            case "GameScene":
                networkScene.style.display = DisplayStyle.None;
                isNetworkScene = false;
                break;
            default:
                networkScene.style.display = DisplayStyle.None;
                isNetworkScene = false;
                break;
        }

        SetNetworkDisplay();
    }

    private void SetNetworkDisplay()
    {
        if (isNetworkScene)
        {
            isPlayerListOpen = false;
            networkScene.style.display = DisplayStyle.Flex;
            playerMenu.style.display = DisplayStyle.None;

            if (playerMenuContainer.childCount > 0
                || playerSceneContainer.childCount <= 0)
            {
                AddAllPlayerTemplates();
            }
        }
        else
        {
            if (localPlayer == null
                && NetworkManager.Singleton.LocalClient.PlayerObject != null)
            {
                localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
            }

            if (localPlayer != null
                && localPlayer.isPlayerListOpen)
            {
                isPlayerListOpen = true;
                playerMenu.style.display = DisplayStyle.Flex;
            } 
            else if (localPlayer != null
                && !localPlayer.isPlayerListOpen)
            {
                isPlayerListOpen = false;
                playerMenu.style.display = DisplayStyle.None;
            }

            networkScene.style.display = DisplayStyle.None;

            if (playerSceneContainer.childCount > 0
                || playerMenuContainer.childCount <= 0)
            {
                AddAllPlayerTemplates();
            }
        }
    }

    private void OnStartClicked()
    {
        int seed;

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Start can only be clicked by host.");
            return;
        }

        if (!useRandomSeed)
        {
            IntegerField seedField = networkScene.Q<IntegerField>("seed");
            seed = seedField.value;
        }
        else
        {
            seed = Random.Range(0, 999999);
        }

        IntegerField maxSegCountField = networkScene.Q<IntegerField>("max-seg-count");
        IntegerField roomCountField = networkScene.Q<IntegerField>("room-count");
        IntegerField staffMinField = networkScene.Q<IntegerField>("staff-min");
        IntegerField bathMinField = networkScene.Q<IntegerField>("bath-min");
        FloatField diffSegBoostField = networkScene.Q<FloatField>("diff-seg-boost");

        GameInfo info = ScriptableObject.CreateInstance<GameInfo>();

        info.Seed = seed;
        info.MaxSegmentCount = maxSegCountField.value;
        info.RoomCount = roomCountField.value;
        info.StaffMin = staffMinField.value;
        info.BathMin = bathMinField.value;
        info.DiffSegBoost = diffSegBoostField.value;

        SerializableGameInfo gameInfo = info.GetSerializableGameInfo();
        GameManager.Singleton.gameInfo.Value = gameInfo;
        GameManager.Singleton.isRandomSeed.Value = useRandomSeed;

        NetworkScript.Singleton.LoadVanScene();
    }

    private void OnRandClicked()
    {
        if (isNetworkScene)
        {
            Button useRandomBtn = networkScene.Q<Button>("use-rand-btn");
            useRandomSeed = !useRandomSeed;
            useRandomBtn.text = useRandomSeed ? "True" : "False";
        }
        else
        {
            Debug.LogWarning("Calling OnRandClicked when not in NetworkMenu scene");
        }

        return;
    }

    public void ToggleDisplayPlayerList(bool isOpen)
    {
        if (isNetworkScene)
        {
            playerMenu.style.display = DisplayStyle.None;
            return;
        }
        else
        {
            isPlayerListOpen = isOpen;

            if (isPlayerListOpen)
            {
                playerMenu.style.display = DisplayStyle.Flex;
            }
            else
            {
                playerMenu.style.display = DisplayStyle.None;
            }
        }
    }

    public void InitPlayerDic()
    {
        connectedPlayerDic.Clear();

        AddAllPlayerTemplates();
    }

    public void AddAllPlayerTemplates()
    {
        playerSceneContainer.Clear();
        playerMenuContainer.Clear();

        foreach (PlayerProfileData player in NetworkScript.Singleton.allPlayerProfileData)
        {
            if (!connectedPlayerDic.ContainsKey(player))
            {
                var playerCont = playerTemplate.Instantiate();
                PlayerTemplate template = playerCont.Q<PlayerTemplate>();

                template.TemplateInit(defaultAvatar, player.playerName.ToString(), player.steamID);

                connectedPlayerDic.Add(player, template);

                if (isNetworkScene)
                {
                    playerSceneContainer.Add(template);
                }
                else
                {
                    playerMenuContainer.Add(template);
                }
            }
            else
            {
                if (isNetworkScene)
                {
                    playerSceneContainer.Add(connectedPlayerDic[player]);
                }
                else
                {
                    playerMenuContainer.Add(connectedPlayerDic[player]);
                }
            }
        }
    }

    public void AddPlayerToDic(PlayerProfileData player)
    {
        if (!connectedPlayerDic.ContainsKey(player))
        {
            var playerCont = playerTemplate.Instantiate();
            PlayerTemplate template = playerCont.Q<PlayerTemplate>();

            template.TemplateInit(defaultAvatar, player.playerName.ToString(), player.steamID);

            connectedPlayerDic.Add(player, template);

            playerSceneContainer.Add(template);
            playerMenuContainer.Add(template);
        }
        else
        {
            Debug.LogWarning($"Player {player.playerName} already in connectedPlayerDic");
        }
    }

    public void RemovePlayerFromDic(PlayerProfileData player)
    {
        if (connectedPlayerDic.ContainsKey(player))
        {
            playerSceneContainer.Remove(connectedPlayerDic[player]);
            playerMenuContainer.Remove(connectedPlayerDic[player]);

            connectedPlayerDic.Remove(player);
        }
        else
        {
            Debug.LogWarning($"Player {player.playerName} not found in connectedPlayerDic");
        }
    }

    void OnBackBtnClicked()
    {
        NetworkScript.Singleton.Disconnect();
        NetworkScript.Singleton.LoadMainMenu();
    }
}
