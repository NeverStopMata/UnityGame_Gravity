using UnityEngine;
using System.Collections;
using GoogleARCore.HelloAR;

public class DieDetectShell : MonoBehaviour
{
    private Transform playerTransform;
    HelloARController mainControlCenter;
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mainControlCenter = GameObject.FindGameObjectWithTag("MainController").GetComponent<HelloARController>();
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            mainControlCenter.FinishGame(false);
        }
    }

}