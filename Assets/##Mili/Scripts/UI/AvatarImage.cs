using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarImage : MonoBehaviour
{
    public Toggle thisToggle;
    public Image thisImage;
    public string serverID;
    public string imageURL;
   public void SetImage(Sprite sprite,string id, string thisURL)
    {
        thisImage.sprite = sprite;
        thisToggle.name = sprite.name;
        serverID = id;
        imageURL = thisURL;
    }
}
