using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationItem : MonoBehaviour
{
    public TextMeshProUGUI message;

    public void Set(string n)
    {
        message.text = n;
    }
}
