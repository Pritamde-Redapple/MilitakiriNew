using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dustbin : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Pawn Fell");
        MilitakiriAudioManager.OnTableDrop?.Invoke();
        Destroy(collision.gameObject,2);
    }
}
