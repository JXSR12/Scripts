using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingZone : MonoBehaviour
{
    public int hpHealedPerFrame;
    public Canvas canvas;
    public Transform lookAt;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.SetHealingMode(true, hpHealedPerFrame);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.SetHealingMode(false, 0);
        }
    }

    void Update()
    {
        canvas.transform.LookAt(2 * transform.position - lookAt.position);
    }
}
