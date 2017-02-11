using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMove : MonoBehaviour {

    public int speed = 10;
    public int rotateSpeed = 1;
    public GameObject TankTop;
    public GameObject Cannon;
    public GameObject pivot;
    private float modSpeed;
    private float modRotateSpeed;
    private int angleCurrent;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

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

        //turn Top of Tank to the right
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("DPadLeftRight") > 0)
        {
            TankTop.transform.Rotate(0f, (1 * rotateSpeed), 0f);
        }

        //turn Top of Tank to the left
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("DPadLeftRight") < 0)
        {
            TankTop.transform.Rotate(0f, (-1 * rotateSpeed), 0f);
        }

        //point cannon up
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("DPadUpDown") > 0)
        {
            if (angleCurrent < 100) {
                Cannon.transform.RotateAround(pivot.transform.position, pivot.transform.right, -10 * Time.deltaTime);
                angleCurrent++;
            }
        }

        //point cannon down
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("DPadUpDown") < 0)
        {
            if (angleCurrent > -80) {
                Cannon.transform.RotateAround(pivot.transform.position, pivot.transform.right, 10 * Time.deltaTime);
                angleCurrent--;
            }
        }
    }
}
