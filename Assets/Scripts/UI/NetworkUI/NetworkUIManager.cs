using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkUIScript : NetworkBehaviour
{
    VisualElement uiDoc;

    bool useRandomSeed = true;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            uiDoc = GetComponent<UIDocument>().rootVisualElement;

            Button startBtn = uiDoc.Q<Button>("start-btn");
            Button useRandBtn = uiDoc.Q<Button>("use-rand-btn");

            useRandBtn.text = useRandomSeed ? "True" : "False";

            startBtn.clicked += OnStartClicked;
            useRandBtn.clicked += OnRandClicked;
        }
        else
        {
            Debug.LogError("GameManager.Instance is null");
            Debug.Break();
        }
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
            IntegerField seedField = uiDoc.Q<IntegerField>("seed");
            seed = seedField.value;
        }
        else
        {
            seed = Random.Range(0, 999999);
        }

        IntegerField maxSegCountField = uiDoc.Q<IntegerField>("max-seg-count");
        IntegerField roomCountField = uiDoc.Q<IntegerField>("room-count");
        IntegerField staffMinField = uiDoc.Q<IntegerField>("staff-min");
        IntegerField bathMinField = uiDoc.Q<IntegerField>("bath-min");
        FloatField diffSegBoostField = uiDoc.Q<FloatField>("diff-seg-boost");

        GameInfo info = new()
        {
            Seed = seed,
            MaxSegmentCount = maxSegCountField.value,
            RoomCount = roomCountField.value,
            StaffMin = staffMinField.value,
            BathMin = bathMinField.value,
            DiffSegBoost = diffSegBoostField.value
        };

        SerializableGameInfo gameInfo = info.GetSerializableGameInfo();
        GameManager.Instance.gameInfo.Value = gameInfo;

        NetworkScript.Singleton.LoadHostGame();
    }

    public void OnRandClicked()
    {
        Button useRandomBtn = uiDoc.Q<Button>("use-rand-btn");
        useRandomSeed = !useRandomSeed;
        useRandomBtn.text = useRandomSeed ? "True" : "False";
    }
}
