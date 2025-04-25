using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject networkManager;

    NetworkManager net;
    int maxSegInt = 20;
    VisualElement uiDoc;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            DontDestroyOnLoad(networkManager);
            uiDoc = GetComponent<UIDocument>().rootVisualElement;
            net = gameObject.GetComponent<NetworkManager>();

            Button start = uiDoc.Q<Button>("start");
            Button hostButton = uiDoc.Q<Button>("host-button");
            Button clientButton = uiDoc.Q<Button>("client-button");

            start.clicked += OnPlayClicked;
            hostButton.clicked += OnHostClicked;
            clientButton.clicked += OnClientClicked;
        }
        else
        {
            Debug.LogError("MainMenuManager: GameManager not found");
            Debug.Break();
        }
    }

    public void OnPlayClicked()
    {
        IntegerField integerField = uiDoc.Q<IntegerField>("max-seg-int");
        maxSegInt = integerField.value;
        GameManager.Instance.gameData.maxSegmentCount = maxSegInt;
        Debug.Log("play clicked");
        SceneManager.LoadScene("GameScene");
    }

    public void OnHostClicked()
    {
        
    }

    public void OnClientClicked()
    {

    }
}
