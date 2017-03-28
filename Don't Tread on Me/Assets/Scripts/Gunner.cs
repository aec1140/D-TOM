using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : MonoBehaviour {

    public bool amGunner;

    public GameObject Cannon;
    public GameObject cannonPivot;
    private int angleCurrent;

    // for gunner rotation
    Quaternion oldRotation;
    public float gunnerRotateSpeed = 1.0f;

    // Use this for initialization
    void Start () {
        oldRotation = this.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        #region tank top rotation
        //turn Top of Tank to the right
        if (amGunner)
        {
            if (Input.GetAxis("RightThumbStick") != 0.0f)
            {
                this.transform.Rotate(0f, (Input.GetAxis("RightThumbStick") * gunnerRotateSpeed), 0f);
                oldRotation = this.transform.rotation;
            }
        }

        // 100.0f is hardcoded until I figure how to properly deal with parent child rotation
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, oldRotation, Time.deltaTime * 100.0f); 
        #endregion

        #region cannon angle
        //point cannon up
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("DPadUpDown") > 0)
        {
            if (angleCurrent < 100)
            {
                Cannon.transform.RotateAround(cannonPivot.transform.position, cannonPivot.transform.right, -10 * Time.deltaTime);
                angleCurrent++;
            }
        }

        //point cannon down
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("DPadUpDown") < 0)
        {
            if (angleCurrent > -80)
            {
                Cannon.transform.RotateAround(cannonPivot.transform.position, cannonPivot.transform.right, 10 * Time.deltaTime);
                angleCurrent--;
            }
        }
        #endregion
    }
}
