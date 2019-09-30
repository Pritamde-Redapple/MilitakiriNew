using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyFriend : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public string id;
    public void Set(string n, string id)
    {
        playerName.name = n;
        this.id = id;
    }
}
