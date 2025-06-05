using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseObject;
    VisualElement pauseUi;
    [SerializeField]
    GameObject guiObject;
    VisualElement GUI;

    Button resumeBtn;
    Button settingsBtn;
    Button mainReturnBtn;
    Button quitBtn;
    Box popupOverlay;
    Box popupBox;
    TextElement popupTitle;
    Button popupConfirmBtn;
    Button popupCancelBtn;

    public static UIManager Singleton { get; private set; }
    
    public bool isPaused = false;
    bool isPopup = false;
    private bool isReturnMain;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }

    private void Start()
    {
        pauseUi = pauseObject.GetComponent<UIDocument>().rootVisualElement;
        GUI = guiObject.GetComponent<UIDocument>().rootVisualElement;

        resumeBtn = pauseUi.Q<Button>("resume-btn");
        settingsBtn = pauseUi.Q<Button>("resume-btn");
        mainReturnBtn = pauseUi.Q<Button>("main-return-btn");
        quitBtn = pauseUi.Q<Button>("quit-btn");

        popupOverlay = pauseUi.Q<Box>("popup-overlay");
        popupBox = pauseUi.Q<Box>("popup-box");
        popupTitle = pauseUi.Q<TextElement>("popup-title");
        popupConfirmBtn = pauseUi.Q<Button>("popup-confirm-btn");
        popupCancelBtn = pauseUi.Q<Button>("popup-cancel-btn");

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

        PopupClassCheck();

        resumeBtn.clicked += ResumeBtnClicked;
        settingsBtn.clicked += SettingsBtnClicked;
        mainReturnBtn.clicked += MainReturnBtnClicked;
        quitBtn.clicked += QuitBtnClicked;
    }

    private void PopupClassCheck()
    {
        if (isPopup)
        {
            resumeBtn.SetEnabled(false);
            settingsBtn.SetEnabled(false);
            mainReturnBtn.SetEnabled(false);
            quitBtn.SetEnabled(false);

            popupOverlay.RemoveFromClassList("popup-disabled");
            popupOverlay.AddToClassList("popup-enabled");

            popupBox.RemoveFromClassList("popup-disabled");
            popupBox.AddToClassList("popup-enabled");
        }
        else
        {
            resumeBtn.SetEnabled(true);
            settingsBtn.SetEnabled(true);
            mainReturnBtn.SetEnabled(true);
            quitBtn.SetEnabled(true);

            popupOverlay.RemoveFromClassList("popup-enabled");
            popupOverlay.AddToClassList("popup-disabled");

            popupBox.RemoveFromClassList("popup-enabled");
            popupBox.AddToClassList("popup-disabled");
        }
    }
    
    public bool TogglePause()
    {
        isPaused = !isPaused;

        if (!isPaused)
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
        Debug.LogError("Settings doesn't exist yet :(");
        Debug.Break();
    }

    private void MainReturnBtnClicked()
    {
        isReturnMain = true;
        TogglePopup("Are you sure you'd like to return to the main menu?");
    }

    private void QuitBtnClicked()
    {
        isReturnMain = false;
        TogglePopup("Are you sure you'd like to quit?");
    }

    private void TogglePopup(string textEntry)
    {
        isPopup = !isPopup;
        PopupClassCheck();

        if (isPopup)
        {
            popupTitle.text = textEntry;

            popupConfirmBtn.clicked += PopupConfirmBtnClicked;
            popupCancelBtn.clicked += PopupCancelBtnClicked;
        }
        else
        {
            popupConfirmBtn.clicked -= PopupConfirmBtnClicked;
            popupCancelBtn.clicked -= PopupCancelBtnClicked;
        }
    }

    private void PopupConfirmBtnClicked()
    {
        Debug.Log("Stopping host or server...");
        NetworkScript.Singleton.Disconnect();

        if (isReturnMain)
        {
            Debug.Log("Returning to main menu...");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("Quitting application...");
            Application.Quit();
        }
    }

    private void PopupCancelBtnClicked()
    {
        TogglePopup("none");
        Debug.Log(isPopup);
    }
}
