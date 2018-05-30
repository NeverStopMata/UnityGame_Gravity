using UnityEngine;
using System.Collections;
using GoogleARCore.HelloAR;

public class Endpoint : MonoBehaviour
{
    private Transform playerTransform;
    HelloARController mainControlCenter;
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mainControlCenter = GameObject.FindGameObjectWithTag("MainController").GetComponent<HelloARController>();
    }

    void OnTriggerEnter(Collider collider)
    {
        mainControlCenter.FinishGame(true);
    }
    //void OnTriggerStay(Collider collider)
    //{
    //    battleControlCenter.SwitchLevel();

    //}
    //void OnTriggerExit(Collider collider)
    //{
    //    battleControlCenter.EndLevelChange();
    //}
}