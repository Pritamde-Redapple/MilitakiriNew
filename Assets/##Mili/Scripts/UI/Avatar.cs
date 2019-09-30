using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using GWebUtility;
public class Avatar : UIPage
{
    public AvatarImage[] toggleImages;
    string selectedImageID;
    public Button confirmButton;
    Sprite selectedDP;
    private void OnEnable()
    {
        toggleImages = GetComponentsInChildren<AvatarImage>(true);

        GetAvatarData();
        
    }

    void GetAvatarData()
    {
            Web.Create()
       .SetUrl(Configuration.Instance.GetApi(Configuration.ApiKey.AVATAR_LIST), Web.RequestType.POST, Web.ResponseType.TEXT)

       .AddHeader("Content-Type", "application/x-www-form-urlencoded")
       .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
       .SetOnSuccessDelegate((Web _web, Response _response) =>
      {
          AllImages allImages = JsonUtility.FromJson<AllImages>(_response.GetText());
          string url = "http://52.66.82.72:2095/images/avatar/";
          for (int i = 0; i < allImages.result.Length; i++)
          {
              StartCoroutine(DownloadImage(url + allImages.result[i].image, i, allImages.result[i]._id));
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





    IEnumerator DownloadImage(string MediaUrl, int id, string serverID)
    {
        Debug.Log("Getting Image from: "+ MediaUrl);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;


            Sprite sprite = Sprite.Create(texture2D,new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);


            toggleImages[id].SetImage(sprite, serverID, MediaUrl);
            toggleImages[id].gameObject.SetActive(true);
        }
    }

    public void SelectedThisImage(AvatarImage toggle)
    {
        selectedImageID = toggle.serverID;
        confirmButton.interactable = true;
        selectedDP = toggle.thisImage.sprite;
        Database.PutString(Database.Key.AVATAR_LINK, toggle.imageURL);
    }

    public void ConfirmImage()
    {
        SocketController.instance.ConfirmAvatarImage(selectedImageID);
    }
}
