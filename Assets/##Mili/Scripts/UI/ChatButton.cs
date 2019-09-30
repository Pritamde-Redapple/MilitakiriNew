using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatButton : MonoBehaviour
{
    public string message;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Text>().text = message;
        GetComponent<Button>().onClick.AddListener(()=> GamePlay.instance.SendAMessage(message));
    }

    
}
