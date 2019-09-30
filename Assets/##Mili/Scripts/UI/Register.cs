﻿using GWebUtility;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register : UIPage {

    public InputField iName;
    public InputField iUserName;
    public InputField iPassword;
    public InputField iConfirmPass;
    string genderString = "male";
    public Dropdown dropdown;
    private int dropDrownId= 0;
    private void Start()
    {
        GetDropdownData();
    }
    public void SignUpClicked()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("no internet");
            PopupCanvas.Instance.ShowAlertPopUp("Please check your internet connection!");
            return;
        }
        else if (iName.text == "")
        {
            PopupCanvas.Instance.ShowAlertPopUp("Enter Name");
            return;
        }
        if (string.IsNullOrEmpty(iUserName.text)/* || validate.ValidateEmail(email.text) == Validate.ErrorCode.INVALID*/)
        {
            PopupCanvas.Instance.ShowAlertPopUp("Enter Mobile No or Email Id");
            return;
        }
        else if (iPassword.text == "")
        {
            PopupCanvas.Instance.ShowAlertPopUp("Enter Password");
            return;
        }
        else if (iConfirmPass.text == "")
        {
            PopupCanvas.Instance.ShowAlertPopUp("Enter confirm Password");
            return;
        }
        else if (iPassword.text != iConfirmPass.text)
        {
            PopupCanvas.Instance.ShowAlertPopUp("Password didn't match");
            return;
        }

        else
        {
            RegisterUpAPICall();
        }
    }

    public void OnToggleChanged(Toggle toggle)
    {
        Database.PutString(Database.Key.GENDER, toggle.name);
    }

    public void RegisterUpAPICall()
    {
        LoadingCanvas.Instance.ShowLoadingPopUp("Loading...");
         Debug.Log("the Name of the Country and Code is: " + GetDropDownValue() + " "+ Configuration.Instance.GetCountryCode(GetDropDownValue()).ToString());
        Web.Create()
           .SetUrl(Configuration.Instance.GetApi(Configuration.ApiKey.REGISTRATION), Web.RequestType.POST, Web.ResponseType.TEXT)
           .AddField(Constants.NAME, iName.text)
           .AddField(Constants.USER_NAME, iUserName.text)
           .AddField(Constants.PASSWORD, iPassword.text)
           .AddField(Constants.GENDER, genderString)
           .AddField("country", GetDropDownValue())
           .AddField("country_code", Configuration.Instance.GetCountryCode(GetDropDownValue()).ToString())
          .SetOnSuccessDelegate((Web _web, Response _response) =>
          {
              Debug.Log(_response.GetText());
              LoadingCanvas.Instance.HideLoadingPopUp();

              JSONNode node = JSONNode.Parse(_response.GetText());

              if (node["status"].Value == ErrorCode.SUCCESS_CODE)
              {

                  Database.PutString(Database.Key.ACCESS_TOKEN, node["result"]["access_token"].Value);
                  Database.PutString(Database.Key.PLAYER_ID, node["result"]["id"].Value);
                 // Database.Instance.PutString(Database.Key.FIRST_NAME, node["result"]["first_name"].Value);
                 // Database.Instance.PutString(Database.Key.LAST_NAME, node["result"]["last_name"].Value);

                 
                 Database.PutString(Database.Key.IMAGE, node["result"]["image"].Value);
                  
                
                 UIManager.instance.TransitionTo(UIPage.PageType.AVATAR);
              }
              else if ((node["status"]).Value == ErrorCode.UNIQUIE_MOBILE)
              {
                  PopupCanvas.Instance.ShowAlertPopUp("Mobile Number already registered");
              }
              else if (node["status"].Value == ErrorCode.ERROR_LOGIN_GAME_RUNNING)
              {
                  PopupCanvas.Instance.ShowAlertPopUp(node["message"].Value);
#if _D_I
                  Debug.Log(node["message"].Value);
#endif
              }
              else
              {
                  PopupCanvas.Instance.ShowAlertPopUp(node["message"].Value);
              }

              _web.Close();

          })
          .SetOnFailureDelegate((Web _web, Response _response) =>
          {
              Debug.Log(_response.GetText());
              LoadingCanvas.Instance.HideLoadingPopUp();
              if (_response.GetText().Contains("check the connectivity"))
              {
                  PopupCanvas.Instance.ShowAlertPopUp("Please check your internet connection!");
              }
              else
              {
                  PopupCanvas.Instance.ShowAlertPopUp("Server not found!");
              }
              Debug.Log(_response.GetText());
              _web.Close();
          })
          .Connect();
    }


    public void LoginClicked()
    {
        UIManager.instance.TransitionTo(PageType.LOGIN);
    }

    void GetDropdownData()
    {
        string[] allCountryNames = Configuration.Instance.GetAllCountryNames();
        PopulateDropdown(dropdown, allCountryNames);
    }

    void PopulateDropdown(Dropdown dropdown, string[] optionsArray)
    {
        List<string> options = new List<string>();
        foreach (var option in optionsArray)
        {
            options.Add(option); // Or whatever you want for a label
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    public void SelectedDropDownIndex(int i)
    {
        Debug.Log(dropdown.options[dropdown.value].text);
    }

    public string GetDropDownValue()
    {
        return dropdown.options[dropdown.value].text;
    }

    
}
