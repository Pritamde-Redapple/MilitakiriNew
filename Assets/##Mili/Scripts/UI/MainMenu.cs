using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GWebUtility;
using SimpleJSON;

public class MainMenu : UIPage {

    public Text notificationCount;
    public NotificationItem notificationItem;
    public Transform notificationParent;
    private void OnEnable()
    {
        GetNotifications();
    }

    public void PlayClicked()
    {
        UIManager.instance.TransitionTo(PageType.CHOOSE);
    }
    public void TutorialClicked()
    {

    }
    public void LeaderboardClicked()
    {
        UIManager.instance.TransitionTo(PageType.LEADERBOARD);
    }
    public void OtherClicked()
    {

    }
    public void SubscriptionClicked()
    {

    }
    public void FriendListClicked()
    {
        UIManager.instance.TransitionTo(PageType.FRIENDS);
    }

    public void CheckAvatars()
    {
        UIManager.instance.TransitionTo(PageType.AVATAR);
    }

    public void Logout()
    {
        Database.DeleteEverything();
        UIManager.instance.TransitionTo(PageType.SPLASH_LOGIN);
    }

    /*
     * 
     * 
     * //Success
{
    "status": "1",
    "result": {
        "notilists": [
            {
                "read_unread": 0,
                "_id": "5d847e080b7e9099351ed570",
                "sent_by_user": "5c343f5538822b17292a1734",
                "received_by_user": "5c335181e4cbb927887d42d8",
                "message": "asd sent you a friend request",
                "created_at": "2019-01-08T10:43:39.726Z",
                "updated_at": "2019-01-08T10:43:39.727Z"
            },
            {
                "read_unread": 0,
                "_id": "5d847e450b7e9099351ed57b",
                "sent_by_user": "5c343f5538822b17292a1734",
                "received_by_user": "5c335181e4cbb927887d42d8",
                "message": "asd sent you a friend request",
                "created_at": "2019-01-08T10:43:39.726Z",
                "updated_at": "2019-01-08T10:43:39.727Z"
            }
        ]
    },
    "message": "All notification lists fetched."
}

     * 
     * 
     * 
     * 
     */
    public void GetNotifications()
    {
        Web.Create()
         .SetUrl("http://52.66.82.72:2095/notificationLists", Web.RequestType.POST, Web.ResponseType.TEXT)
         .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
         .AddHeader("Content-Type", "application/x-www-form-urlencoded")
         .SetOnSuccessDelegate((Web _web, Response _response) =>
         {
             JSONNode node = JSONNode.Parse(_response.GetText());
             notificationCount.text = ""+ node["result"]["notilists"].Count;

             for (int i = 0; i < node["result"]["notilists"].Count; i++)
             {
                 NotificationItem newItem = Instantiate(notificationItem);
                 newItem.transform.SetParent(notificationParent);
                 newItem.transform.localScale = Vector3.one;
                 newItem.Set(node["result"]["notilists"][i]["message"].Value);
             }
                 Debug.Log(_response.GetText());
             _web.Close();
         })
        .SetOnFailureDelegate((Web _web, Response _response) =>
        {
            Debug.Log(_response.GetText());
            _web.Close();
        })
        .Connect();
    }
}
