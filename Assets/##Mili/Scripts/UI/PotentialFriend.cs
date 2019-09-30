using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GWebUtility;
public class PotentialFriend : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public string id;
   
    public void Set(string n, string id)
    {
        playerName.text = n;
        this.id = id;
    }
    
    public void SendInvite(GameObject go)
    {
        Web.Create()
        .SetUrl("http://52.66.82.72:2095/sendInvitation", Web.RequestType.POST, Web.ResponseType.TEXT)
        .AddField("received_by_user", id)
        .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
        .AddHeader("Content-Type", "application/x-www-form-urlencoded")


        .SetOnSuccessDelegate((Web _web, Response _response) =>
        {
            Debug.Log(_response.GetText());
            go.SetActive(false);
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
