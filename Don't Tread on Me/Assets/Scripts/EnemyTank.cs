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

    public bool slowed;
    public float slowAmount = 2f;

    public float speed = 8;
    public float rotateSpeed = 2.5f;
    float baseSpeed;
    float baseRotateSpeed;

    public float reloadTime = 3.0f;
    private float timeLast = 0.0f;

    GameManager gameManager;
    GameObject gm;

    public int pointWorth;

    public ParticleSystem explosion;

    // Use this for initialization
    void Start()
    {
        gm = GameObject.Find("GameManager");
        gameManager = gm.GetComponent<GameManager>();

        pointWorth = 50;
        
        target = GameObject.Find("Player");
        rocketTrue = true;
        slowed = false;

        baseSpeed = speed;
        baseRotateSpeed = rotateSpeed;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        Acquire();

        float currentHP = this.gameObject.GetComponent<HP>().getCurrHP();
        if (currentHP <= 0)
        {
            Instantiate(explosion, transform.position, transform.rotation);
            gameManager.SetScore(pointWorth);
            Destroy(this.gameObject);
        }

        if (slowed)
        {
            speed = 4f;
            rotateSpeed = 1f;
        }
        else
        {
            speed = baseSpeed;
            rotateSpeed = baseRotateSpeed;
        }
    }

    //changed all forces to point up, because for some rason the launcher.transform.forward points down into the ground
    void FireProjectile()
    {
        Rigidbody clone;
        clone = Instantiate(Projectile, Launcher.transform.position + (SPAWN_DISTANCE * Launcher.transform.up), Launcher.transform.rotation) as Rigidbody;
        clone.velocity = Launcher.transform.TransformDirection(Vector3.up * power);
        //Destroy(clone);
    }

    
    void Acquire()
    {
        /*if the target is within detection range*/
        if (target && Vector3.Distance(target.transform.position, tankBody.transform.position) < detectionRange)
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
                transform.LookAt(target.transform.position);

                //check to make sure you're not going to hit your friend
                Ray ray = new Ray(transform.position, target.transform.position - transform.position);
                //if hit something within certain distance, pick another direction
                RaycastHit obstacle;
                //Debug.DrawRay(transform.position, target.transform.position - transform.position);
                if (Physics.Raycast(ray, out obstacle, shootingRange))
                {
                    //if what you hit is the player, terrain, or the flamethrowerbox
                    if (obstacle.collider.GetComponent<PlayerTank>() || obstacle.collider.name == "Terrain" || obstacle.collider.name == "flameThrowerBox")
                    {
                        //shoot at it
                        if (Time.time - timeLast > reloadTime)
                        {
                            FireProjectile();
                            timeLast = Time.time;
                        }//reload time
                    }
                    else
                    {
                        //do nothing
                        //print(obstacle.collider.name + " in path");
                        return;
                    }
                }
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
