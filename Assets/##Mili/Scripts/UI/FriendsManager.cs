using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GWebUtility;
using SimpleJSON;

public class FriendsManager : UIPage
{
    public Button showFriends;
    public Button showRequests;

    public Transform friendsPanel;
    public Transform requestPanel;


    public AcceptRejectElement acceptReject;
    public MyFriend myFriend;

    private void OnEnable()
    {
        GetFriends();
        GetRequests();
    }



    // Start is called before the first frame update
    void Start()
    {
        showFriends.image.color  = Color.green;
        showRequests.image.color = Color.grey;
        ShowFriends(true);
        ShowRequest(false);
    }

    public void ShowFriends(bool state)
    {
        friendsPanel.gameObject.SetActive(state);
        if (state)
        {
            showFriends.image.color = Color.green;
            showRequests.image.color = Color.grey;
            ShowRequest(!state);
        }
    }

    public void ShowRequest(bool state)
    {
        requestPanel.gameObject.SetActive(state);
        if (state)
        {
            showFriends.image.color = Color.grey;
            showRequests.image.color = Color.green;
            ShowFriends(!state);
        }
    }

    public void Goback()
    {
        UIManager.instance.TransitionTo(UIPage.PageType.MAINMENU);
    }

    public void AddFriends()
    {
        UIManager.instance.TransitionTo(UIPage.PageType.SEARCH_FRIENDS);
    }
    /*
     * 
     * {
    "status": "1",
    "result": [
        {
            "_id": "5c334ca09d2b1d2611e2eeec",
            "name": "adfsd gh"
        },
        {
            "_id": "5c335181e4cbb927887d42d8",
            "name": "asdeaghsd sdf1"
        }
    ],
    "message": "Invited friend list"
}
     * 
     * 
     * 
     */
    void GetFriends()
    {
        Web.Create()
         .SetUrl("http://52.66.82.72:2095/friendLists", Web.RequestType.POST, Web.ResponseType.TEXT)
         .AddField("status", "accepted")
         .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
         .AddHeader("Content-Type", "application/x-www-form-urlencoded")         
         

         .SetOnSuccessDelegate((Web _web, Response _response) =>
         {
             JSONNode node = JSONNode.Parse(_response.GetText());
           //  SearchFriendsData friendsdata = JsonUtility.FromJson<SearchFriendsData>(node["result"].ToString());

             for (int i = 0; i < node["result"].Count; i++)
             {
                 MyFriend newFriend = Instantiate(myFriend);
                 newFriend.transform.SetParent(requestPanel);
                 newFriend.Set(node["result"][i]["name"], node["result"][i]["_id"]);
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
    /*
     * 
     * 
     * {
"status":"1",
"result":{
"userlists":[
{
"registration_type":"normal",
"score":4,
"win":11,
"lost":10,
"draw":0,
"forfeit":8,
"_id":"5d8869e3ad3d552a7f8cd88b",
"name":"Harry",
"username":"harry@gmail.com"
}
]
},
"message":"Last played player list"
}
     * 
     * 
     * 
     * 
     */
    void GetRequests()
    {
        Web.Create()
         .SetUrl("http://52.66.82.72:2095/invitedFriends", Web.RequestType.POST, Web.ResponseType.TEXT)
         .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
         .AddHeader("Content-Type", "application/x-www-form-urlencoded")

         .SetOnSuccessDelegate((Web _web, Response _response) =>
         {
             JSONNode node = JSONNode.Parse(_response.GetText());             
             SearchFriendsData friendsdata = JsonUtility.FromJson<SearchFriendsData>(node["result"].ToString());

             for (int i = 0; i < friendsdata.userlists.Length; i++)
             {
                 AcceptRejectElement accept = Instantiate(acceptReject);
                 accept.transform.SetParent(requestPanel);
                 accept.transform.localScale = Vector3.one;
                 accept.Set(friendsdata.userlists[i].name, friendsdata.userlists[i]._id);                
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
