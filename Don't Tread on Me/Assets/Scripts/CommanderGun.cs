using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderGun : MonoBehaviour {

    public bool amCommander = true;
    public float rotateSpeed = 1;
    public float deadZone = 0.1f;
    public GameObject bullet;
    public GameObject tracer;
    public GameObject firePoint;
    public float bulletSpeed = 20.0f;
    public float reloadTime = 0.5f;
    public int tracerCap = 4;

    private float timeLast = 0.0f;
    private float oldX = 0;
    private float oldY = 0;
    private int tracerCounter = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (amCommander)
        {
            float x = Input.GetAxis("RightThumbStick");
            float y = Input.GetAxis("RightThumbVertical");

            float angle;

            if(Mathf.Abs(x) < deadZone && Mathf.Abs(y) < deadZone)
            {
                angle = Mathf.Atan2(oldX, -oldY);
            }
            else
            {
                oldX = x;
                oldY = y;
                angle = Mathf.Atan2(x, -y);
                Fire();
            }

            Quaternion destination = Quaternion.EulerAngles(0, angle, 0) * Quaternion.AngleAxis(45, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, destination, Time.deltaTime * rotateSpeed);
        }
    }

    void Fire()
    {
        if (Time.time - timeLast > reloadTime)
        {
            GameObject newBullet;

            if(tracerCounter == tracerCap)
            {
                newBullet = Instantiate(tracer, firePoint.transform.position, transform.rotation);
                tracerCounter = 0;
            }
            else
            {
                newBullet = Instantiate(bullet, firePoint.transform.position, transform.rotation);
                tracerCounter++;
            }

            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
            timeLast = Time.time;
        }
    }
}
