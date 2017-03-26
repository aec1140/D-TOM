using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class PlayerTank : MonoBehaviour {

    public PlayerID driver;

    // these are for main hull rotation
    public float hullRotateSpeed = 100.0f;
    private float rotateSpeed;

    public int speed = 10;
    private float modSpeed;
    private float modRotateSpeed;

    //public float HP = 100;

    public int BoostTotal = 300; //5 seconds of boost
    private int BoostFrames = 300;
    private int i;
    public int boostCooldown = 60;
    private int framesSinceLastBoost;
    private bool canBoost;

    public bool amDriver;
    private Rigidbody rb;
    public GameObject rightTreadPivot;
    public GameObject leftTreadPivot;

    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody>();
        rotateSpeed = hullRotateSpeed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (amDriver) //if driver
        {
            #region driver
            #region boost what a fucking mess
            //driver's boost
            if (InputManager.GetAxis("Right Trigger", driver) == 1 && InputManager.GetAxis("Left Trigger", driver) == 1) //are the boost buttons being pressed?
            {
                if (BoostFrames > 0 && canBoost == true) //is there boost left, and is it on cooldown?
                {
                    //then go fast
                    speed = 10 * 2;
                    rotateSpeed = hullRotateSpeed * 2;

                    //but reduce the boost pool
                    BoostFrames--;
                }//if
                else
                {
                    speed = 10;
                    rotateSpeed = hullRotateSpeed;
                }
            }//if 
            else if (InputManager.GetAxis("Right Trigger", driver) != 1 || InputManager.GetAxis("Left Trigger", driver) != 1)
            {
                speed = 10;
                rotateSpeed = hullRotateSpeed;

                i++;
                if (i >= 3)
                {
                    if (BoostFrames <= BoostTotal)
                    {
                        BoostFrames++;
                    }
                    i = 0;
                }
            }

            if (BoostFrames == 0)
            {
                canBoost = false;
            }
            if (canBoost == false)
            {
                if (BoostFrames >= boostCooldown)
                {
                    canBoost = true;
                }
            }
            #endregion

            //rotate clockwise
            if (InputManager.GetAxis("Left Stick Vertical", driver) < 0 && InputManager.GetAxis("Right Stick Vertical", driver) > 0)
            {
                Quaternion deltaRotation = Quaternion.Euler((new Vector3(0, 1, 0) * (rotateSpeed)) * Time.deltaTime);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }

            //rotate counterclockwise
            if (InputManager.GetAxis("Left Stick Vertical", driver) > 0 && InputManager.GetAxis("Right Stick Vertical", driver) < 0)
            {
                Quaternion deltaRotation = Quaternion.Euler((new Vector3(0, -1, 0) * (rotateSpeed)) * Time.deltaTime);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }

            //move forward
            if (InputManager.GetAxis("Left Stick Vertical", driver) < 0 && InputManager.GetAxis("Right Stick Vertical", driver) < 0)
            {
                rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
            }

            //move backward
            if (InputManager.GetAxis("Left Stick Vertical", driver) > 0 && InputManager.GetAxis("Right Stick Vertical", driver) > 0)
            {
                rb.MovePosition(transform.position + -transform.forward * Time.deltaTime * speed);
            }

            //left stick only
            if (InputManager.GetAxis("Left Stick Vertical", driver) < 0 && InputManager.GetAxis("Right Stick Vertical", driver) == 0)
            {
                rotateRigidBodyAroundPointBy(rb, rightTreadPivot.transform.position, rightTreadPivot.transform.up, (rotateSpeed * Time.deltaTime));
            }
            if (InputManager.GetAxis("Left Stick Vertical", driver) > 0 && InputManager.GetAxis("Right Stick Vertical", driver) == 0)
            {
                rotateRigidBodyAroundPointBy(rb, rightTreadPivot.transform.position, rightTreadPivot.transform.up, -(rotateSpeed * Time.deltaTime));
            }

            //right stick only
            if (InputManager.GetAxis("Left Stick Vertical", driver) == 0 && InputManager.GetAxis("Right Stick Vertical", driver) > 0)
            {
                rotateRigidBodyAroundPointBy(rb, leftTreadPivot.transform.position, leftTreadPivot.transform.up, (rotateSpeed * Time.deltaTime));
            }
            if (InputManager.GetAxis("Left Stick Vertical", driver) == 0 && InputManager.GetAxis("Right Stick Vertical", driver) < 0)
            {
                rotateRigidBodyAroundPointBy(rb, leftTreadPivot.transform.position, leftTreadPivot.transform.up, -(rotateSpeed * Time.deltaTime));
            }
            #endregion
        }
    }

    public void rotateRigidBodyAroundPointBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        //rb is what is rotating
        //origin is the point around which to rotate
        //axis is the axis we are rotating around
        //angle is how much, so use rotspeed * deltatime?
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        rb.MoveRotation(rb.transform.rotation * q);
    }//rotate rigidbody around point
}
