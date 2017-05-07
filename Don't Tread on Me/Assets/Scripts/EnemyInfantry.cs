using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyInfantry : MonoBehaviour {


    // Variable Declaration
    public Rigidbody Projectile = null;
    private const float SPAWN_DISTANCE = 1.0f;
    public int power = 20;
    public float detectionRange = 50;
    public float shootingRange = 10;

    public GameObject target;

    public bool slowed;
    public float slowAmount = 2f;

    public float speed = 4;
    public float rotateSpeed = 4f;
    float baseSpeed;
    float baseRotateSpeed;

    public float reloadTime = 3.0f;
    private float timeLast = 0.0f;

    public ParticleSystem explosion;

    // Use this for initialization
    void Start()
    {
        slowed = false;
        baseSpeed = speed;

        target = GameObject.Find("Player");

        //this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX;
        //this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        Acquire();

        float currentHP = this.gameObject.GetComponent<HP>().getCurrHP();
        if (currentHP <= 0)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }

        if (slowed)
        {
            speed = 2f;
            rotateSpeed = 1f;
        }
        else
        {
            speed = baseSpeed;
            rotateSpeed = baseRotateSpeed;
        }
    }

    //changed all forces to point up, because for some rason the launcher.transform.forward points down into the ground
    void FireProjectile(int type)
    {
        Rigidbody clone;
        if (type == 0)
        {
            clone = Instantiate(Projectile, transform.position + (SPAWN_DISTANCE * transform.up), transform.rotation) as Rigidbody;
        }
        else
        {
            clone = Instantiate(Projectile, transform.position + (SPAWN_DISTANCE * transform.up), transform.rotation) as Rigidbody;
        }
        clone.velocity = transform.TransformDirection(Vector3.up * power);
        //Destroy(clone);
    }


    void Acquire()
    {
        /*if the target is within detection range*/
        if (Vector3.Distance(target.transform.position, transform.position) < detectionRange)
        {
            //print("target sighted");
            /*if the target is within shooting range*/
            if (Vector3.Distance(target.transform.position, transform.position) < shootingRange)
            {
                //print("target within range");
                /*aim at target*/
                transform.LookAt(target.transform.position);
                // print(this.gameObject.name + " shooting");
                //print("taking aim");
                /*and fire*/
                //print("firing");
                //commneted out because different infantry types are a little out of scope
                //if (Time.time - timeLast > reloadTime)
                //{
                //    FireProjectile(0);
                //    timeLast = Time.time;
                //}//reload time
            }
            else
            {
                // print("repositioning");
                /*else aim tank bottom at target*/
                transform.LookAt(target.transform.position);
                /*and drive forward(deltatime)*/
                GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
            }
        }
        //else do a small patrol route?
    }
}
