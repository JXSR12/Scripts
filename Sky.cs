using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour
{
    public float skyboxRotateSpeed;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time*skyboxRotateSpeed);
    }
}