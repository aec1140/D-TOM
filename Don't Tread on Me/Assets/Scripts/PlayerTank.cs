using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : MonoBehaviour {

    // these are for main hull rotation
    public float hullRotateSpeed = 100.0f;
    private float rotateSpeed;

    public int speed = 10;
    private float modSpeed;
    private float modRotateSpeed;

    public GameObject HPbar;
    private float currentHP;
    private float totalHP;
    private float percentHP;


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

        totalHP = this.gameObject.GetComponent<HP>().MaxHP;
        for (int e = 0; e < 10; e++)
        {
            HPbar.transform.GetChild(e).gameObject.SetActive(true);
        }

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (amDriver) //if driver
        {
            #region driver
            #region boost what a fucking mess
            //driver's boost
            if (Input.GetAxis("RightTrigger") == 1 && Input.GetAxis("LeftTrigger") == 1) //are the boost buttons being pressed?
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
            else if (Input.GetAxis("RightTrigger") != 1 || Input.GetAxis("LeftTrigger") != 1)
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
            if (Input.GetAxis("LeftThumbStick") < 0 && Input.GetAxis("RightThumbVertical") > 0)
            {
                Quaternion deltaRotation = Quaternion.Euler((new Vector3(0, 1, 0) * (rotateSpeed)) * Time.deltaTime);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }

            //rotate counterclockwise
            if (Input.GetAxis("LeftThumbStick") > 0 && Input.GetAxis("RightThumbVertical") < 0)
            {
                Quaternion deltaRotation = Quaternion.Euler((new Vector3(0, -1, 0) * (rotateSpeed)) * Time.deltaTime);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }

            //move forward
            if (Input.GetAxis("LeftThumbStick") < 0 && Input.GetAxis("RightThumbVertical") < 0)
            {
                    rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
            }

            //move backward
            if (Input.GetAxis("LeftThumbStick") > 0 && Input.GetAxis("RightThumbVertical") > 0)
            {
                    rb.MovePosition(transform.position + -transform.forward * Time.deltaTime * speed);
            }

            //left stick only
            if (Input.GetAxis("LeftThumbStick") < 0 && Input.GetAxis("RightThumbVertical") == 0)
            {
                rotateRigidBodyAroundPointBy(rb, rightTreadPivot.transform.position, rightTreadPivot.transform.up, (rotateSpeed * Time.deltaTime));
            }
            if (Input.GetAxis("LeftThumbStick") > 0 && Input.GetAxis("RightThumbVertical") == 0)
            {
                rotateRigidBodyAroundPointBy(rb, rightTreadPivot.transform.position, rightTreadPivot.transform.up, -(rotateSpeed * Time.deltaTime));
            }

            //right stick only
            if (Input.GetAxis("LeftThumbStick") == 0 && Input.GetAxis("RightThumbVertical") > 0)
            {
                rotateRigidBodyAroundPointBy(rb, leftTreadPivot.transform.position, leftTreadPivot.transform.up, (rotateSpeed * Time.deltaTime));
            }
            if (Input.GetAxis("LeftThumbStick") == 0 && Input.GetAxis("RightThumbVertical") < 0)
            {
                rotateRigidBodyAroundPointBy(rb, leftTreadPivot.transform.position, leftTreadPivot.transform.up, -(rotateSpeed * Time.deltaTime));
            }
            #endregion
        }
        else
        {
            #region default movement
            // Paul: commented out because I don't think we need this anymore?
            /*
            //move forward - currently very shitty on keyboard
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            if (Input.GetAxis("LeftThumbStick") < 0)
            {
                modSpeed = speed * -Input.GetAxis("LeftThumbStick");
                transform.Translate(Vector3.forward * modSpeed * Time.deltaTime);
            }

            //move backward - see move forward
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
            if (Input.GetAxis("LeftThumbStick") > 0)
            {
                modSpeed = speed * Input.GetAxis("LeftThumbStick");
                transform.Translate(Vector3.back * modSpeed * Time.deltaTime);
            }

            //rotate left
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0f, (-1 * rotateSpeed), 0f);
            }
            if (Input.GetAxis("RightThumbStick") < 0)
            {
                modRotateSpeed = rotateSpeed * -Input.GetAxis("RightThumbStick");
                transform.Rotate(0f, (-1 * modRotateSpeed), 0f);
            }

            //rotate right
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0f, (1 * rotateSpeed), 0f);
            }
            if (Input.GetAxis("RightThumbStick") > 0)
            {
                modRotateSpeed = rotateSpeed * Input.GetAxis("RightThumbStick");
                transform.Rotate(0f, (1 * modRotateSpeed), 0f);
            }
            */
            #endregion
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            flipTank();
        }
        currentHP = this.gameObject.GetComponent<HP>().getCurrHP();
        percentHP = currentHP / totalHP;
        CalcHPBar(percentHP);
    }
    #region old takeDamage
    //public void TakeDamage(float damage)
    //{
    //    //subtract damage from HP
    //    HP -= damage;
    //    // print("Current HP: " + HP);

    //    //if no HP
    //    if (HP <= 0)
    //    {
    //        //destroy the tank
    //        print("She's dead, Jim");
    //    }
    //}//take damage
    #endregion

    //this is like putting a bandaid on a gunshot wound
    public void flipTank()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0) //will be positive if local up vector has a down component and 1 if it's completely flipped
        {
            Vector3 newPos = new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z);
            rb.MovePosition(newPos);
            rotateRigidBodyAroundPointBy(rb, leftTreadPivot.transform.position, leftTreadPivot.transform.forward, 180);
        }
    }
    
    //method to calculate what pips to show in HP; probably not the most efficient solution
    //nested ifs would probably be better, but wouldnt look as nice
    public void CalcHPBar(float percentage)
    {
        int hidePipsStart = 11;
        if (percentage < 1 && percentage > 0.9)
        {
            hidePipsStart = 9;
        }
        else if (percentage < 0.9 && percentage > 0.8)
        {
            hidePipsStart = 8;
        }
        else if (percentage < 0.8 && percentage > 0.7)
        {
            hidePipsStart = 7;
        }
        else if (percentage < 0.7 && percentage > 0.6)
        {
            hidePipsStart = 6;
        }
        else if (percentage < 0.6 && percentage > 0.5)
        {
            hidePipsStart = 5;
        }
        else if (percentage < 0.5 && percentage > 0.4)
        {
            hidePipsStart = 4;
        }
        else if (percentage < 0.4 && percentage > 0.3)
        {
            hidePipsStart = 3;
        }
        else if (percentage < 0.3 && percentage > 0.2)
        {
            hidePipsStart = 2;
        }
        else if (percentage < 0.2 && percentage > 0.1)
        {
            hidePipsStart = 1;
        }
        else if (percentage < 0.1 && percentage > 0.0)
        {
            hidePipsStart = 0;
        }
        else if (percentage <= 0.0)
        {
            hidePipsStart = 0;
        }
        //iterates through the pips, starting at the value dictated by the series of ifs, and turns them off
        for (int e = hidePipsStart; e < 10; e++)
        {
            HPbar.transform.GetChild(e).gameObject.SetActive(false);
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
