using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseObject;
    VisualElement pauseUi;
    [SerializeField]
    GameObject guiObject;
    VisualElement GUI; 

    public static UIManager Instance { get; private set; }
    
    public bool isPaused = false;
    bool isPopup = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        pauseUi = pauseObject.GetComponent<UIDocument>().rootVisualElement;
        GUI = guiObject.GetComponent<UIDocument>().rootVisualElement;

        Button resumeBtn = pauseUi.Q<Button>("resume-btn");
        Button settingsBtn = pauseUi.Q<Button>("resume-btn");
        Button mainReturnBtn = pauseUi.Q<Button>("main-return-btn");
        Button quitBtn = pauseUi.Q<Button>("quit-btn");
        Button popupConfirmBtn = pauseUi.Q<Button>("popup-confirm-btn");
        Button popupCancelBtn = pauseUi.Q<Button>("popup-cancel-btn");

        if (isPaused)
        {
            pauseUi.SetEnabled(true);
            pauseUi.visible = true;
        }
        else
        {
            pauseUi.SetEnabled(false);
            pauseUi.visible = false;
        }

        resumeBtn.clicked += ResumeBtnClicked;
        settingsBtn.clicked += SettingsBtnClicked;
        mainReturnBtn.clicked += MainReturnBtnClicked;
        quitBtn.clicked += QuitBtnClicked;
    }
    
    public bool TogglePause()
    {
        if (isPaused)
        {
            pauseUi.SetEnabled(false);
            isPaused = false;
            pauseUi.visible = false;
        }
        else
        {
            pauseUi.SetEnabled(true);
            isPaused = true;
            pauseUi.visible = true;
        }
        return isPaused;
    }

    private void ResumeBtnClicked()
    {
        TogglePause();
    }

    private void SettingsBtnClicked()
    {
        
    }

    private void MainReturnBtnClicked()
    {

    }

    private void QuitBtnClicked()
    {

    }
}
