using System;
using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class ShopCameraSwitch : MonoBehaviour
{
    public CinemachineCamera isoCam;
    public CinemachineCamera shopCam;

    void Start()
    {
        isoCam.Priority = 10;
        shopCam.Priority = 0;
    }

    public void EnterShop()
    {
        shopCam.Priority = 20;
        isoCam.Priority = 0;
    }

    public void ExitShop()
    {
        isoCam.Priority = 20;
        shopCam.Priority = 0;
    }

    private void OnTriggerStay(Collider other) => EnterShop();

    private void OnTriggerExit(Collider other) => ExitShop();
}