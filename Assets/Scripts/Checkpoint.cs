using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Transform playerTransform;
    BattleControlCenter battleControlCenter;
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        battleControlCenter = GameObject.FindGameObjectWithTag("Battle").GetComponent< BattleControlCenter>();
    }

    void OnTriggerEnter(Collider collider)
    {
        battleControlCenter.BeginLevelChange();
        //TimeBacker _timebacker = GetComponentInParent<BattleControlCenter>().timebacker;
        //_timebacker.clearRecord();
        
    }
    private void OnTriggerStay(Collider collider)
    {
        battleControlCenter.SwitchLevel();
        
    }
    void OnTriggerExit(Collider collider)
    {
        //TimeBacker _timebacker = GetComponentInParent<BattleControlCenter>().timebacker;
        battleControlCenter.EndLevelChange();
    }
}