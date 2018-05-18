using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
public class GravityController : MonoBehaviour
{

    // Use this for initialization

    [HideInInspector]
    public bool isChangingGravity = false;
    private float changingProcess = 0.0f;//
    enum ChangingMode { rotate, revert }
    public Vector3 originGravityDrct = new Vector3(0, -9.81f, 0);

    public Vector3 targetGravityDrct = new Vector3(0, -9.81f, 0);
    public float gravityRotateSpeed = 3.0f;
    public Transform mainCamera_transform;
    private Transform battle_transform;
    public bool canChangeGravityInAir = false;
    private GameObject player;
    private ChangingMode changeMode;
    private ThirdPersonCharacter tpc;
    private enum slideDrct { upSlide, downSlide, leftSlide, righSlide, noSlide }
    private CapsuleCollider plrCapsule;
    private Vector3 GravityDrctInBattleWorld;

    // Update is called once per frame
    void Start()
    {
        player = this.gameObject;
        battle_transform = transform.parent;
  
        //Physics.gravity = new Vector3(0, -9.81F, 0);
        GravityDrctInBattleWorld = new Vector3(0, -9.81F, 0);
        Physics.gravity = GravityDrctInBattleWorld;
        originGravityDrct = Physics.gravity;
        targetGravityDrct = Physics.gravity;
        tpc = player.GetComponent<ThirdPersonCharacter>();
        if (mainCamera_transform == null)
        {
            mainCamera_transform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        plrCapsule = gameObject.GetComponent<CapsuleCollider>();
        //dsada


    }
    private void FixedUpdate()
    {
        float x = CrossPlatformInputManager.GetAxis("Mouse X");
        float y = CrossPlatformInputManager.GetAxis("Mouse Y");
        if(x!=0||y!=0)
        {
            CrossPlatformInputManager.SetAxis("Mouse X", 0.0f);
            CrossPlatformInputManager.SetAxis("Mouse Y", 0.0f);
        }
        GravityDrctInBattleWorld = battle_transform.InverseTransformDirection(Physics.gravity);

        
        if (isChangingGravity)
        {
            float step = gravityRotateSpeed * Time.deltaTime;
            changingProcess += step;

            if (changeMode == ChangingMode.rotate)
            {
                if (changingProcess > 1)
                {
                    changingProcess = 1;
                    isChangingGravity = false;
                }
                //Vector3 lastGravityDrct = Physics.gravity;//
                Vector3 lastGravityDrct = GravityDrctInBattleWorld;//


                //Physics.gravity = Vector3.Slerp(originGravityDrct, targetGravityDrct, changingProcess);

                GravityDrctInBattleWorld = Vector3.Slerp(originGravityDrct, targetGravityDrct, changingProcess);
                Physics.gravity = battle_transform.TransformDirection(GravityDrctInBattleWorld);


                //player.transform.rotation = Quaternion.FromToRotation(lastGravityDrct, Physics.gravity) * player.transform.rotation;
                player.transform.rotation = Quaternion.FromToRotation(battle_transform.TransformDirection(lastGravityDrct), Physics.gravity) * player.transform.rotation;
            }
            else if (changeMode == ChangingMode.revert)
            {
                player.transform.RotateAround(player.transform.position + player.transform.up  * plrCapsule.height * transform.localScale.y, player.transform.right, 180.0f * step);//(new Vector3(1, 0, 0), 3.14f * step);

                if (changingProcess > 1)
                {
                    changingProcess = 1;
                    isChangingGravity = false;
                    player.transform.rotation = Quaternion.FromToRotation(-player.transform.up, targetGravityDrct) * player.transform.rotation;
                }
                //Physics.gravity = Vector3.Lerp(originGravityDrct, targetGravityDrct, changingProcess);
                GravityDrctInBattleWorld = Vector3.Lerp(originGravityDrct, targetGravityDrct, changingProcess);
                Physics.gravity = battle_transform.TransformDirection(GravityDrctInBattleWorld);

            }

            //Debug.Log (player.transform.up);

        }

        else if ((canChangeGravityInAir || tpc.m_IsGrounded) && getSlideDrctFromAxis(x,y) == slideDrct.upSlide)
        {      
            targetGravityDrct = getPlayerDrct(battle_transform.InverseTransformDirection(player.transform.forward));
            //originGravityDrct = Physics.gravity;
            originGravityDrct = GravityDrctInBattleWorld;
            isChangingGravity = true;
            changingProcess = 0;
            changeMode = ChangingMode.rotate;
        }

        else if ((canChangeGravityInAir || tpc.m_IsGrounded) && getSlideDrctFromAxis(x, y) == slideDrct.downSlide)
        {
            //targetGravityDrct = getPlayerDrct(player.transform.forward);
            //originGravityDrct = getPlayerDrct(-Physics.gravity) * 0.3f;
            originGravityDrct = getPlayerDrct(-GravityDrctInBattleWorld) * 0.3f;
            //targetGravityDrct = getPlayerDrct(-Physics.gravity);
            targetGravityDrct = getPlayerDrct(-GravityDrctInBattleWorld);
            isChangingGravity = true;
            changingProcess = 0;
            changeMode = ChangingMode.revert;
        }

        //Debug.Log (targetGravityDrct);

    }
    public static Vector3 getPlayerDrct(Vector3 originDirection)
    {

        if (originDirection.x > 0 && Mathf.Abs(originDirection.y) / originDirection.x <= 1 && Mathf.Abs(originDirection.z) / originDirection.x <= 1)
        {
            Vector3 tempVector = new Vector3(9.81f, 0, 0);
            

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
    slideDrct getSlideDrctFromAxis(float x, float y)
    {

       // return slideDrct.downSlide;
        if (Mathf.Abs(x) < y)
        {
            return slideDrct.upSlide;
        }

        else if (-Mathf.Abs(x) > y)
        {
            return slideDrct.downSlide;
        }
        //else if (Mathf.Abs(y) < x)
        //{
        //    return slideDrct.righSlide;
        //}
        //else if (-Mathf.Abs(y) > x)
        //{
        //    return slideDrct.leftSlide;
        //}
        else
        {
            return slideDrct.noSlide;
        }
    }
}