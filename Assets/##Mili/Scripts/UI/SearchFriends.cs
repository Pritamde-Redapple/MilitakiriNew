using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWebUtility;
using SimpleJSON;
using TMPro;
public class SearchFriends : UIPage
{

    public PotentialFriend potentialFriend;
    public Transform spawnPoint;
    public TextMeshProUGUI searchValueText;
    private void OnEnable()
    {
        FetchPotentialFriends();
    }

    public void GoBack()
    {
        UIManager.instance.TransitionTo(UIPage.PageType.FRIENDS);
    }


    /*        
{
"status":"1",
"result":{
"userlists":[
{
"registration_type":"normal",
"score":8,
"win":7,
"lost":6,
"draw":0,
"forfeit":0,
"_id":"5d8869e3ad3d552a7f8cd88b",
"name":"Harry",
"username":"harry@gmail.com"
}
]
},
"message":"Last played player list"
}

    */




    void FetchPotentialFriends()
    {
        Debug.Log("Access: " + Database.GetString(Database.Key.ACCESS_TOKEN));
        Web.Create()
          .SetUrl("http://52.66.82.72:2095/invitedFriends", Web.RequestType.POST, Web.ResponseType.TEXT)
          .AddHeader("Content-Type", "application/x-www-form-urlencoded")
          
          .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))

          .SetOnSuccessDelegate((Web _web, Response _response) =>
          {
              JSONNode node = JSONNode.Parse(_response.GetText());
              Debug.Log("Access: " + node["result"]["userlists"].ToString());
              SearchFriendsData friendsdata = JsonUtility.FromJson<SearchFriendsData>(node["result"].ToString());
              Debug.Log("Name>>>>>>>>>>"+ friendsdata.userlists[0].name);

              for (int i = 0; i < friendsdata.userlists.Length; i++)
              {
                  PotentialFriend pf = Instantiate(potentialFriend);
                  pf.transform.SetParent(spawnPoint);
                  pf.transform.localScale = Vector3.one;
                  pf.Set(friendsdata.userlists[i].name, friendsdata.userlists[i]._id);
              }
              _web.Close();

          })
         .SetOnFailureDelegate((Web _web, Response _response) =>
         {
             Debug.Log(_response.GetText());
             _web.Close();
         })
         .Connect();
    }

    public void SearchSpecificFriend()
    {
        Debug.Log("Text sending: "+ searchValueText.text + " Access : "+ Database.GetString(Database.Key.ACCESS_TOKEN));
        Web.Create()
         .SetUrl("http://52.66.82.72:2095/invitedFriends", Web.RequestType.POST, Web.ResponseType.TEXT)         
        
         .AddHeader("Content-Type", "application/x-www-form-urlencoded")
         .AddField("search", searchValueText.text)
         .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
         
         .SetOnSuccessDelegate((Web _web, Response _response) =>
         {
             Debug.Log("Access: " + _response.ToString());
             JSONNode node = JSONNode.Parse(_response.GetText());
             Debug.Log("Access: " + node["result"].ToString());            
             SearchFriendsData friendsdata = JsonUtility.FromJson<SearchFriendsData>(node["result"].ToString());
             Debug.Log("Name>>>>>>>>>>" + friendsdata.userlists[0].name);

             foreach (Transform item in spawnPoint)
             {
                 Destroy(item.gameObject);
             }

             for (int i = 0; i < friendsdata.userlists.Length; i++)
             {
                 PotentialFriend pf = Instantiate(potentialFriend);
                 pf.transform.SetParent(spawnPoint);
                 pf.transform.localScale = Vector3.one;
                 pf.Set(friendsdata.userlists[i].name, friendsdata.userlists[i]._id);
             }
             _web.Close();
         })
        .SetOnFailureDelegate((Web _web, Response _response) =>
        {
            Debug.Log(_response.GetText());
            _web.Close();
        })
        .Connect();
    }

    public void CloseSearchResult()
    {
        foreach (Transform item in spawnPoint)
        {
            Destroy(item.gameObject);
        }

        FetchPotentialFriends();
    }
}
