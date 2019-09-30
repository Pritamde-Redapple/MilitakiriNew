using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GWebUtility;
using SimpleJSON;
public class AcceptRejectElement : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    string id;
    public void Set(string n, string id)
    {
        playerName.text = n;
        this.id = id;
    }

    public void OnAccept(GameObject go)
    {
        Web.Create()
 .SetUrl("http://52.66.82.72:2095/acceptInvitation", Web.RequestType.POST, Web.ResponseType.TEXT)
 
 .AddField("sent_by_user", id)
 .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
 .AddHeader("Content-Type", "application/x-www-form-urlencoded")


 .SetOnSuccessDelegate((Web _web, Response _response) =>
 {
     Debug.Log(_response.GetText());
     _web.Close();
     go.SetActive(false);
 })
.SetOnFailureDelegate((Web _web, Response _response) =>
{
    Debug.Log(_response.GetText());
    _web.Close();
})
.Connect();
    }

    public void OnReject(GameObject go)
    {
        Web.Create()
.SetUrl("http://52.66.82.72:2095/rejectInvitation", Web.RequestType.POST, Web.ResponseType.TEXT)
.AddField("sent_by_user", id)
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
