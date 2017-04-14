using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class Engineer : MonoBehaviour {
    public bool amEngineer;

    public Rigidbody Projectile = null;

    

    // For Grenade aiming
    public GameObject target;
    public GameObject reticle;
    private Vector3 oldTargetPosition;
    public float translateSpeed = 10.0f;
    bool snapBack = true;
    public float aimDistance;

    // Reload Time
    float reloadTime;
    private float timeLast = 0.0f;

    Camera mainCamera;
    private const float SPAWN_DISTANCE = 3.5f;

    public GameObject engineerPanel;
    private Animator anim;

    private GameObject inputMngr;
    private PlayerRoles playerRoles;
    public PlayerID playerID;

    // TEMP For testing other ammo
    public bool isMine;


    // Use this for initialization
    void Start () {

        aimDistance = 15f;
        
        reloadTime = 2f;

        // Init
        anim = engineerPanel.GetComponent<Animator>();
        anim.enabled = true;

        inputMngr = GameObject.Find("InputManager");
        playerRoles = inputMngr.GetComponent<PlayerRoles>();
        playerRoles.HidePanel(anim);

        playerID = inputMngr.GetComponent<PlayerRoles>().engineer;
    }
	
	// Update is called once per frame
	void Update () {

        

        if (playerID != inputMngr.GetComponent<PlayerRoles>().engineer) return;

        if (amEngineer)
        {
            // Reticle follows the tank
            Vector3 follow = target.transform.position - oldTargetPosition;
            reticle.transform.Translate(follow, Space.World);

            oldTargetPosition = target.transform.position;

            // Hold to allow for Grenade aiming
            if(InputManager.GetAxis("Left Trigger", playerID) > 0)
            {
                reticle.SetActive(true);

                // sapBcks the reticle to tank position after pulling let tigger
                if (snapBack)
                {
                    reticle.transform.position = (target.transform.position + new Vector3(0,5.11f,0));
                    snapBack = false;
                }
               // if (Vector3.Distance(reticle.transform.position, target.transform.position) < aimDistance)
                
                    if (InputManager.GetAxis("Left Stick Vertical", playerID) != 0.0f)
                    {
                        reticle.transform.Translate(new Vector3(1, 0, 1) * -InputManager.GetAxis("Left Stick Vertical", playerID) * translateSpeed * Time.deltaTime, Space.World);

                        // Prevents player from moving reticle past a certain distance from tank. There is probably a better way to do this 
                        if (Vector3.Distance(reticle.transform.position, target.transform.position) >= aimDistance)
                        {
                            reticle.transform.Translate(new Vector3(1, 0, 1) * InputManager.GetAxis("Left Stick Vertical", playerID) * translateSpeed * Time.deltaTime, Space.World);
                        }

                    }
                    if (InputManager.GetAxis("Left Stick Horizontal", playerID) != 0.0f)
                    {
                        reticle.transform.Translate(new Vector3(-1, 0, 1) * -InputManager.GetAxis("Left Stick Horizontal", playerID) * translateSpeed * Time.deltaTime, Space.World);
                        
                        // Prevents player from moving reticle past a certain distance from tank. There is probably a better way to do this 
                        if (Vector3.Distance(reticle.transform.position, target.transform.position) >= aimDistance)
                        {
                            reticle.transform.Translate(new Vector3(-1, 0, 1) * InputManager.GetAxis("Left Stick Horizontal", playerID) * translateSpeed * Time.deltaTime, Space.World);
                        }
                    }

                if (InputManager.GetAxis("Right Trigger", playerID) > 0)
                {
                    if (Time.time - timeLast > reloadTime)
                    {

                        if (!isMine)
                        {
                            ThrowGrenade();
                        } else if (isMine)
                        {
                            Rigidbody clone = Instantiate(Projectile, target.transform.position + (SPAWN_DISTANCE * -target.transform.forward), target.transform.rotation) as Rigidbody;
                        }


                        timeLast = Time.time;
                    }
                } 

            }

            else
            {
                reticle.SetActive(false);
                snapBack = true;
            } 
        }

        if (InputManager.GetAxis("DPAD Vertical", playerID) == 1)
        {
            Gunner gunner = GetComponent<Gunner>();
            gunner.playerID = playerID;
            playerID = inputMngr.GetComponent<PlayerRoles>().gunner;

            inputMngr.GetComponent<PlayerRoles>().gunner = gunner.playerID;
            inputMngr.GetComponent<PlayerRoles>().engineer = playerID;
        }
        else if (InputManager.GetAxis("DPAD Vertical", playerID) == -1)
        {
            Driver driver = GetComponent<Driver>();
            driver.playerID = playerID;
            playerID = inputMngr.GetComponent<PlayerRoles>().driver;

            inputMngr.GetComponent<PlayerRoles>().driver = driver.playerID;
            inputMngr.GetComponent<PlayerRoles>().engineer = playerID;
        }
        else if (InputManager.GetAxis("DPAD Horizontal", playerID) == 1)
        {
            Commander commander = GetComponent<Commander>();
            commander.playerID = playerID;
            playerID = inputMngr.GetComponent<PlayerRoles>().commander;

            inputMngr.GetComponent<PlayerRoles>().commander = commander.playerID;
            inputMngr.GetComponent<PlayerRoles>().engineer = playerID;
        }

    }


    void ThrowGrenade()
    {

        // Gets the direction from tank to reticle. May not be the most optimal way.
        Vector3 reticleTowards = reticle.transform.position - target.transform.position;
        float reticleDistance = reticleTowards.magnitude;
        Vector3 reticleDirection = reticleTowards / reticleDistance;

        Rigidbody clone = Instantiate(Projectile, target.transform.position + (SPAWN_DISTANCE * reticleDirection), target.transform.rotation) as Rigidbody;

        // sets velocity based on reticle distance from tank. May not be the most optimal way.
        clone.velocity = reticleDirection * (reticleDistance - 3.1f);
        
    }


}
