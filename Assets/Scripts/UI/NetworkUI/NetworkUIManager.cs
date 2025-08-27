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

    Box playerSceneContainer;
    Box playerMenuContainer;

    Dictionary<ulong, PlayerTemplate> connectedPlayerDic = new();

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

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            networkScene = networkSceneObj.GetComponent<UIDocument>().rootVisualElement;
            playerMenu = playerMenuObj.GetComponent<UIDocument>().rootVisualElement;

            playerSceneContainer = networkScene.Q<Box>("player-container");
            playerMenuContainer = playerMenu.Q<Box>("player-container");

            Button startBtn = networkScene.Q<Button>("start-btn");
            Button useRandBtn = networkScene.Q<Button>("use-rand-btn");

            useRandBtn.text = useRandomSeed ? "True" : "False";

            if (isPlayerListOpen)
            {
                playerMenu.style.display = DisplayStyle.Flex;
            }
            else
            {
                playerMenu.style.display = DisplayStyle.None;
            }

                startBtn.clicked += OnStartClicked;
            useRandBtn.clicked += OnRandClicked;
        }
        else
        {
            Debug.LogError("GameManager.Instance is null");
            Debug.Break();
        }
    }

    public void PlayerListSceneCheck()
    {

    }

    private bool IsSceneCheck(string targetScene)
    {
        List<Scene> syncScenes = NetworkManager.Singleton.SceneManager.GetSynchronizedScenes();

        foreach (Scene scene in syncScenes)
        {
            if (targetScene == scene.name)
            {
                return true;
            }
        }

        return false;
    }

    public void OnStartClicked()
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
        GameManager.Instance.gameInfo.Value = gameInfo;

        NetworkScript.Singleton.LoadHostGame();
    }

    public void OnRandClicked()
    {
        if (IsSceneCheck("NetworkMenu"))
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

    public void InitPlayerDic(List<ulong> players)
    {

    }
}
