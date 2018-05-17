using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class BattleControlCenter : MonoBehaviour
{


    private List<GameObject> movalbeGOs;
    private TimeBacker timebacker;
    // Use this for initialization
    void Start()
    {
        movalbeGOs = new List<GameObject>();
        movalbeGOs.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        movalbeGOs.AddRange(GameObject.FindGameObjectsWithTag("Dynamic"));
        timebacker = new TimeBacker(movalbeGOs, 600f);

    }

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Restart"))
        {
            timebacker.StartRewind();
        }
        //松开时停止
        if (CrossPlatformInputManager.GetButtonUp("Restart"))
        {
            timebacker.StopRewind();
        }
    }
    private void FixedUpdate()
    {
        timebacker.Execution();
    }

}
