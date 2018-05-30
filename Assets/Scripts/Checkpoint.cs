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
        if (collider.gameObject.tag == "Player")
        {
            battleControlCenter.BeginLevelChange();
        }
                  
    }
    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            battleControlCenter.SwitchLevel();
        }
            
        
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            battleControlCenter.EndLevelChange();
        }
            
    }
}