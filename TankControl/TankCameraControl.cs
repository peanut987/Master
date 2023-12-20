using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCameraControl : MonoBehaviour
{
  
    public GameObject[] tanks;
    public Vector3 targetCameraPos = Vector3.zero;
    public Camera mainCamera;

    public Vector3 currenVelocity = Vector3.zero;
    public float smoothTime = 1;
    public float maxSmoothspeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
      
    }

    // Update is called once per frame
    void Update()
    {
        ResetCameraPos();
    }

    void ResetCameraPos()
    {
        Vector3 sumPos = Vector3.zero;
        foreach(var tank in tanks)
        {
            sumPos += tank.transform.position;
        }
        if(tanks.Length>0)
        {
            targetCameraPos = sumPos / tanks.Length;
        }
        targetCameraPos.y = mainCamera.transform.position.y;
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetCameraPos,ref currenVelocity,smoothTime,maxSmoothspeed);
    }
}
