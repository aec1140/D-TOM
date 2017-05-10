using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyInfantry : MonoBehaviour {


    // Variable Declaration
    public Rigidbody Projectile = null;
    private const float SPAWN_DISTANCE = 1.0f;
    public int power = 50;
    public float detectionRange = 50;
    public float shootingRange = 15;

    public GameObject target;

    public bool slowed;
    public float slowAmount = 2f;

    public float speed = 4;
    public float rotateSpeed = 4f;
    float baseSpeed;
    float baseRotateSpeed;

    public float reloadTimeSeed = 3.0f;
    private float reloadTime;
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

        pointWorth = 10;

        slowed = false;
        baseSpeed = speed;

        target = GameObject.Find("Player");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        reloadTime = Random.Range(reloadTimeSeed - 1.0f, reloadTimeSeed + 1.0f);
    }

    void Update()
    {
        Acquire();

        float currentHP = this.gameObject.GetComponent<HP>().getCurrHP();
        if (currentHP <= 0)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Instantiate(explosion, transform.position, transform.rotation);
            gameManager.SetScore(pointWorth);
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
    void FireProjectile()
    {
        Rigidbody clone;
        clone = Instantiate(Projectile, transform.position + (SPAWN_DISTANCE * transform.forward), transform.rotation) as Rigidbody;
        clone.velocity = transform.TransformDirection(Vector3.forward * power);
    }


    void Acquire()
    {
        /*if the target is within detection range*/
        if (target && Vector3.Distance(target.transform.position, transform.position) < detectionRange)
        {
            //print("target sighted");
            /*if the target is within shooting range*/
            if (Vector3.Distance(target.transform.position, transform.position) < shootingRange)
            {
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
