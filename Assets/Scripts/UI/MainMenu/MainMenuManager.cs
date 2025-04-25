using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    int maxSegInt = 20;
    VisualElement uiDoc;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            uiDoc = GetComponent<UIDocument>().rootVisualElement;

            Button start = uiDoc.Q<Button>("start");

            start.clicked += OnPlayClicked;
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
        SceneManager.LoadScene("InsideTest");
    }
}
