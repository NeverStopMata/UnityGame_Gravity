using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{

    // Use this for initialization

    [HideInInspector]
    public bool isChangingGravity = false;
    public Vector3 targetGravityDrct = new Vector3(0, -9.81f, 0);
    public Vector3 currentGravityDrct = new Vector3(0, -9.81f, 0);
    public GameObject player;
    public float gravityRotateSpeed = 3.0f;
    public float littleDifference = 0.5f;
    public Transform mainCamera_transform;

    // Update is called once per frame
    void Start()
    {
        player = this.gameObject;
        currentGravityDrct = Physics.gravity;
        targetGravityDrct = Physics.gravity;
        littleDifference = 0.5f;
        if (mainCamera_transform == null)
        {
            mainCamera_transform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        //dsada


    }
    void Update()
    {
        if (isChangingGravity)
        {
            float step = gravityRotateSpeed * Time.deltaTime;
            Vector3 lastGravityDrct = currentGravityDrct;
            currentGravityDrct = Vector3.RotateTowards(lastGravityDrct, targetGravityDrct, step, 0.0f);
            if (Vector3.Distance(currentGravityDrct, targetGravityDrct) < littleDifference)
            {
                //Debug.Log ("finish changing gravity!");
                currentGravityDrct = targetGravityDrct;
                isChangingGravity = false;
            }
            Physics.gravity = currentGravityDrct;

            player.transform.rotation = Quaternion.FromToRotation(lastGravityDrct, currentGravityDrct) * player.transform.rotation;
            //Debug.Log (player.transform.up);

        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A))
        {
            //targetGravityDrct = getPlayerDrct(-Vector3.Cross(getPlayerDrct(player.transform.forward), currentGravityDrct));
            targetGravityDrct = getPlayerDrct(Vector3.ProjectOnPlane(-mainCamera_transform.right, currentGravityDrct));
            isChangingGravity = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            //targetGravityDrct = getPlayerDrct(Vector3.Cross(getPlayerDrct(player.transform.forward), currentGravityDrct));
            targetGravityDrct = getPlayerDrct(Vector3.ProjectOnPlane(mainCamera_transform.right, currentGravityDrct));
            isChangingGravity = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
        {
            //targetGravityDrct = getPlayerDrct(player.transform.forward);
            if(currentGravityDrct.y != 0)
            {
                targetGravityDrct = getPlayerDrct(Vector3.ProjectOnPlane(mainCamera_transform.forward, currentGravityDrct));
            }
            else
            {
                targetGravityDrct = getPlayerDrct(Vector3.ProjectOnPlane(mainCamera_transform.up, currentGravityDrct));
            }
            
            isChangingGravity = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            //targetGravityDrct = -getPlayerDrct(player.transform.forward);
            if (currentGravityDrct.y != 0)
            {
                targetGravityDrct = getPlayerDrct(Vector3.ProjectOnPlane(-mainCamera_transform.forward, currentGravityDrct));
            }
            else
            {
                targetGravityDrct = getPlayerDrct(Vector3.ProjectOnPlane(-mainCamera_transform.up, currentGravityDrct));
            }
            isChangingGravity = true;
        }

        //Debug.Log (targetGravityDrct);

    }
    Vector3 getPlayerDrct(Vector3 originDirection)
    {

        if (originDirection.x > 0 && Mathf.Abs(originDirection.y) / originDirection.x <= 1 && Mathf.Abs(originDirection.z) / originDirection.x <= 1)
        {
            return new Vector3(9.81f, 0, 0);
        }
        else if (originDirection.x < 0 && Mathf.Abs(originDirection.y / originDirection.x) <= 1 && Mathf.Abs(originDirection.z / originDirection.x) <= 1)
        {
            return new Vector3(-9.81f, 0, 0);
        }
        else if (originDirection.y > 0 && Mathf.Abs(originDirection.z / originDirection.y) <= 1 && Mathf.Abs(originDirection.x / originDirection.y) < 1)
        {
            return new Vector3(0, 9.81f, 0);
        }
        else if (originDirection.y < 0 && Mathf.Abs(originDirection.z / originDirection.y) <= 1 && Mathf.Abs(originDirection.x / originDirection.y) < 1)
        {
            return new Vector3(0, -9.81f, 0);
        }
        else if (originDirection.z > 0 && Mathf.Abs(originDirection.x / originDirection.z) < 1 && Mathf.Abs(originDirection.y / originDirection.z) < 1)
        {
            return new Vector3(0, 0, 9.81f);
        }
        else if (originDirection.z < 0 && Mathf.Abs(originDirection.x / originDirection.z) <= 1 && Mathf.Abs(originDirection.y / originDirection.z) < 1)
        {
            return new Vector3(0, 0, -9.81f);
        }
        else
        {
            Debug.Log("error about direction telling");
            return new Vector3(0, -9.81f, 0);

        }
    }
}