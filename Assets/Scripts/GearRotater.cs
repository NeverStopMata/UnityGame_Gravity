using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearRotater : MonoBehaviour {

    public Vector3 rotateAxis;
    public float rotateSpeed = 10.0f;//xixi
    // Use this for initialization
    void Start () {
        rotateSpeed = 100.0f;

    }
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * rotateSpeed);
	}
}
