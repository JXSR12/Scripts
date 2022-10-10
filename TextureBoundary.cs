using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureBoundary : MonoBehaviour
{
    public string textureName;
    public string defaultTextureName;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.GetComponent<Player>() != null)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.SetTextureName(textureName);
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.GetComponent<Player>() != null)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.SetTextureName(defaultTextureName);
        }
    }
}
