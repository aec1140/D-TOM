using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyTank : MonoBehaviour
{

    // Variable Declaration
    public Rigidbody Projectile = null;
    private const float SPAWN_DISTANCE = 1.0f;
    public int power = 20;
    public float detectionRange = 100;
    public float shootingRange = 100;
    public bool rocketTrue;

    public Transform Launcher;
    public GameObject tankBody;
    public GameObject tankTop;
    public GameObject target;

    public float speed = 8;
    public float rotateSpeed = 2.5f;

    public float reloadTime = 3.0f;
    private float timeLast = 0.0f;

    public ParticleSystem explosion;

    // Use this for initialization
    void Start()
    {
        rocketTrue = true;
    }

    void Update()
    {
        Acquire();

        float currentHP = this.gameObject.GetComponent<HP>().getCurrHP();
        if (currentHP <= 0)
        {
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }

    //changed all forces to point up, because for some rason the launcher.transform.forward points down into the ground
    void FireProjectile(int type)
    {
        Rigidbody clone;
        if (type == 0)
        {
            clone = Instantiate(Projectile, Launcher.transform.position + (SPAWN_DISTANCE * Launcher.transform.up), Launcher.transform.rotation) as Rigidbody;
        }
        else
        {
            clone = Instantiate(Projectile, Launcher.transform.position + (SPAWN_DISTANCE * Launcher.transform.up), Launcher.transform.rotation) as Rigidbody;
        }
        clone.velocity = Launcher.transform.TransformDirection(Vector3.up * power);
        Explode explo = (Explode)clone.gameObject.AddComponent(typeof(Explode)); //clone.AddComponent<Explode>();
        //Destroy(clone);
    }

    
    void Acquire()
    {
        /*if the target is within detection range*/
        if (Vector3.Distance(target.transform.position, tankBody.transform.position) < detectionRange)
        {
            //print("target sighted");
            /*if the target is within shooting range*/
            if (Vector3.Distance(target.transform.position, tankBody.transform.position) < shootingRange)
            {
                //print("target within range");
                /*aim(deltatime) tankTop at target*/
                Vector3 dir = target.transform.position - tankTop.transform.position;
                dir.y = 0; // keep the direction strictly horizontal
                Quaternion rot = Quaternion.LookRotation(dir);
                // slerp to the desired rotation over time
                tankTop.transform.rotation = Quaternion.Slerp(tankTop.transform.rotation, rot, rotateSpeed * Time.deltaTime);

                //print("taking aim");
                /*and fire*/
                //print("firing");
                if (Time.time - timeLast > reloadTime)
                {
                    rocketTrue = true;
                    FireProjectile(0);
                    timeLast = Time.time;
                }//reload time
            }
            else
            {
                //print("repositioning");
                /*else aim tank bottom at target*/
                Vector3 dir = target.transform.position - tankBody.transform.position;
                dir.y = 0; // keep the direction strictly horizontal
                Quaternion rot = Quaternion.LookRotation(dir);
                // slerp to the desired rotation over time
                tankBody.transform.rotation = Quaternion.Slerp(tankBody.transform.rotation, rot, rotateSpeed * Time.deltaTime);

                /*and drive forward(deltatime)*/
                tankBody.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }
        //else do a small patrol route?





        //if (Vector3.Distance(target.transform.position, transform.position) < detectionRange)
        //{
        //    if (Vector3.Angle(Launcher.transform.forward, target.transform.position - Launcher.transform.position) < 10)
        //    {
        //        if (Vector3.Distance(target.transform.position, transform.position) > power)
        //        {
        //            turretBody.transform.Rotate(-5,0,0);
        //        }//if target is within detection range but outside of cannon range
        //    }//if turret is facing target
        //    else
        //    {
        //        turretBody.transform.LookAt(target.transform);
        //    }//if turret is not facing target, face target
        //}//if target is within detection range
    }
}
