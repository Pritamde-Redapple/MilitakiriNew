﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupCanvas : MonoBehaviour {

    public static PopupCanvas Instance;
    public Text txtPopUp;
    public Text txtPopUpForNotEnoughChip;
    public GameObject popUpCanvas;
    public GameObject quitPopupCanvas;
    public GameObject notEnoughChipCanvas;
    public GameObject signInCanvas;
    //public GameObject gameSelectionPanel;
    //public bool bIsPopupActive;
    public  void  Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    public void OnEnable()
    {
        txtPopUp.text = "";
        popUpCanvas.SetActive(false);
       // quitPopupCanvas.SetActive(false);
    }
    public void ShowAlertPopUp(string _popUpTxt)
    {
       
        popUpCanvas.SetActive(true);
        txtPopUp.text = _popUpTxt;
    }
    public void HideAlertPopUp()
    {
        popUpCanvas.SetActive(false);
    }

    public void ShowQuitPopUp()
    {
        quitPopupCanvas.SetActive(true);
    }


    public void QuitNo()
    {
        quitPopupCanvas.SetActive(false);
    }

    public void QuitYes()
    {
        Application.Quit();
    }
    
    public void SignInClicked()
    {
        UIManager.instance.TransitionTo(UIPage.PageType.LOGIN);
        signInCanvas.SetActive(false);
    }

    public void UpdatedAvatarImage(bool state)
    {
        if (state == true)
        {
            popUpCanvas.SetActive(true);
            txtPopUp.text = "Avatar Image Updated";
            PopUp.OnClosePopUp += GoToMainMenu;
        }
        else
        {
            popUpCanvas.SetActive(true);
            txtPopUp.text = "Failed to update avatar image";
            PopUp.OnClosePopUp += GoToMainMenu;
        }
    }



    private void GoToMainMenu(PopUp.PopUpType obj)
    {
        UIManager.instance.TransitionTo(UIPage.PageType.MAINMENU);
        PopUp.OnClosePopUp -= GoToMainMenu;
    }

    //void GoToMainMenu()
    //{
    //    UIManager.instance.TransitionTo(UIPage.PageType.MAINMENU);
    //}

}
