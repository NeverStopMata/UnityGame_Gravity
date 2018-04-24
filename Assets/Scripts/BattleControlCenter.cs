using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleControlCenter : MonoBehaviour
{


    public List<GameObject> movalbeGOs;
    // Use this for initialization
    void Start()
    {
        movalbeGOs = new List<GameObject>();
        movalbeGOs.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        movalbeGOs.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
