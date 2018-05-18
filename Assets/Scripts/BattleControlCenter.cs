using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class BattleControlCenter : MonoBehaviour
{

    public float plrHeightBattleWorld = 0.5f;//battle坐标系下的人体合适高度
    private float heighOffset;
    public float switchLevelSpeed = 0.1f;
    private List<GameObject> movalbeGOs;
    public TimeBacker timebacker;
    private GameObject player;
    private Transform playerTransform;
    private bool isSwitchingLevel = false;


    // Use this for initialization
    void Start()
    {
        movalbeGOs = new List<GameObject>();
         
        movalbeGOs.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        movalbeGOs.AddRange(GameObject.FindGameObjectsWithTag("Dynamic"));
        timebacker = new TimeBacker(movalbeGOs, 600f, transform);
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerTransform = player.transform;
        heighOffset = transform.position.y;
    }

    public void BeginLevelChange()
    {
        timebacker.clearRecord();
        isSwitchingLevel = true;
    }

    public void EndLevelChange()
    {
        isSwitchingLevel = false;
    }

    public void SwitchLevel()
    {
        float heightOffsetRealityWorld = plrHeightBattleWorld + heighOffset;
        if (playerTransform.position.y > heightOffsetRealityWorld)
        {
            if (player.transform.position.y - switchLevelSpeed * Time.fixedDeltaTime < heightOffsetRealityWorld)
            {
                float deltaY = playerTransform.position.y - heightOffsetRealityWorld;
                transform.position = transform.position - new Vector3(0, deltaY, 0);
            }
            else
            {
                transform.Translate(new Vector3(0, -switchLevelSpeed) * Time.fixedDeltaTime, 0);
            }
        }

        else if (playerTransform.position.y < heightOffsetRealityWorld)
        {
            if (playerTransform.position.y + switchLevelSpeed * Time.fixedDeltaTime > heightOffsetRealityWorld)
            {
                float deltaY = playerTransform.position.y - heightOffsetRealityWorld;
                transform.position = transform.position - new Vector3(0, deltaY, 0);
            }
            else
            {
                transform.Translate(new Vector3(0, switchLevelSpeed) * Time.fixedDeltaTime, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Timeback") && !isSwitchingLevel)
        {
            timebacker.StartRewind();
        }
        //松开时停止
        if (CrossPlatformInputManager.GetButtonUp("Timeback") && !isSwitchingLevel)
        {
            timebacker.StopRewind();
        }
    }
    private void FixedUpdate()
    {
        if(!isSwitchingLevel)
        {
            timebacker.Execution();
        }
    }
    

}
