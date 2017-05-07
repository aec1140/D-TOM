using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

public class Driver : MonoBehaviour {

    // these are for main hull rotation
    public float hullRotateSpeed = 100.0f;
    private float rotateSpeed;

    public int speed = 10;
    private float modSpeed;
    private float modRotateSpeed;

    public int BoostTotal = 300; //5 seconds of boost
    private int BoostFrames = 300;
    private int i;
    public int boostCooldown = 60;
    private int framesSinceLastBoost;
    private bool canBoost;

    public float flameDPS;
    public ParticleSystem flames;

    private bool harpoonOut;
    public bool GetHarpoonOut() { return harpoonOut; }
    public void SetHarpoonOut(bool harp) { harpoonOut = harp; }
    public Rigidbody harpoon;
    private Rigidbody harpoonClone;

    private Rigidbody rb;
    public GameObject rightTreadPivot;
    public GameObject leftTreadPivot;

    public GameObject abilityIcon;
    public Sprite[] abilityIcons = new Sprite[3];

    private GameObject inputMngr;
    private PlayerRoles playerRoles;
    public PlayerID playerID;

    // mode switching
    public enum AmmoTypes { Boost, Flamethrower, Harpoon };
    private int selectedMode = ((int)AmmoTypes.Boost);

    public GameObject driverPanel;
    public GameObject ammoPanel;
    private Animator anim;

    private List<string> currentCombo = new List<string>();

    private List<string> boost_combo = new List<string> { "Button A", "Button B", "Button X", "Button A" };
    public GameObject[] boost_buttons = new GameObject[4];

    private List<string> flamethrower_combo = new List<string> { "Button Y", "Button B", "Button X", "Button A" };
    public GameObject[] flamethrower_buttons = new GameObject[4];

    private List<string> harpoon_combo = new List<string> { "Button A", "Button A", "Button X", "Button Y" };
    public GameObject[] harpoon_buttons = new GameObject[4];

    private Dictionary<AmmoTypes, List<string>> ammoCombos = new Dictionary<AmmoTypes, List<string>>();
    private Dictionary<List<string>, GameObject[]> comboButtons = new Dictionary<List<string>, GameObject[]>();

    public GameObject playerIcon;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rotateSpeed = hullRotateSpeed;

        //harpoonActive = true;

        // init ammo swap
        ammoCombos.Add(AmmoTypes.Boost, boost_combo);
        ammoCombos.Add(AmmoTypes.Flamethrower, flamethrower_combo);
        ammoCombos.Add(AmmoTypes.Harpoon, harpoon_combo);

        comboButtons.Add(boost_combo, boost_buttons);
        comboButtons.Add(flamethrower_combo, flamethrower_buttons);
        comboButtons.Add(harpoon_combo, harpoon_buttons);

        // Init
        anim = driverPanel.GetComponent<Animator>();
        anim.enabled = true;

        inputMngr = GameObject.Find("InputManager");
      
        playerRoles = inputMngr.GetComponent<PlayerRoles>();
      
        playerRoles.HidePanel(anim);
        playerRoles.SetComboTextures(comboButtons);

        playerID = inputMngr.GetComponent<PlayerRoles>().driver;

        abilityIcon.GetComponent<Image>().sprite = abilityIcons[selectedMode];
    }
	
    //if something stays in the trigger box
    void OnTriggerStay(Collider other)
    {
        //if the flamethrower is selected
        if (selectedMode == 1)
        {
            //if the player is holding down the buttons
            if (InputManager.GetAxis("Right Trigger", playerID) == 1 && InputManager.GetAxis("Left Trigger", playerID) == 1) //are the boost buttons being pressed?
            {
                //if the thing in the trigger box has life
                if ((other.gameObject.GetComponent("HP") as HP) != null)
                {
                    //call the thing's takeDamage method
                    other.gameObject.GetComponent<HP>().TakeDamage(Time.deltaTime * flameDPS);
                }//if life
            }//if buttons
        }//if flamethrower
    }//onTriggerStay

    //boy i hope this doesnt count the flamethrower box
    void OnCollisionEnter(Collision other)
    {
        //if you are currently slamming, have a harpoon out and what you hit is an enemy tank
        if (other.gameObject.GetComponent<EnemyTank>() && harpoonOut && harpoonClone.GetComponent<Harpoon>().GetSlamActive())
        {
            //remove the harpoon, tell it the slamming is over and wipe the enemy off the face of the planet
            harpoonClone.GetComponent<Harpoon>().ReleaseHook();
            harpoonClone.GetComponent<Harpoon>().SetSlamActive(false);
            other.gameObject.GetComponent<HP>().TakeDamage(200);
        }
    }

    void LaunchHarpoon()
    {
        if (!harpoonOut)
        {
            harpoonClone = Instantiate(harpoon, transform.position + (5 * transform.forward) + (1 * transform.up), transform.rotation * Quaternion.Euler(90,0,0)) as Rigidbody;
            //move the projectile via transform
            //harpoonClone.velocity = transform.TransformDirection(Vector3.forward * 50); //50 as power
            //move the projectile via rigidbody
            harpoonClone.AddForce(transform.forward * 2500); //2500 as force
            harpoonOut = true;
        }
    }

	// Update is called once per frame
	void Update () {

        if (playerID != inputMngr.GetComponent<PlayerRoles>().driver) return;

        playerIcon.GetComponent<Image>().sprite = playerRoles.helmets[(int)playerID];

        if (selectedMode == 1)
        {
            if ((InputManager.GetAxis("Right Trigger", playerID) == 1 && InputManager.GetAxis("Left Trigger", playerID) == 1))
            {
                flames.Emit(5);
            }
        }
        else if (selectedMode == 2)
        {
            //press and hold left trigger to launch harpoon
            if ((InputManager.GetAxis("Left Trigger", playerID) == 1))
            {
                //if a harpoon is not currently out
                if (!harpoonOut)
                {
                    LaunchHarpoon();
                }
            }

            //release to destroy hook + release the hooked obj
            if(InputManager.GetAxis("Left Trigger", playerID) <= 0.5f)
            {
                //if the harpoon is out and has hooked something
                if (harpoonOut)
                {
                    harpoonOut = false;
                    if (harpoonClone)
                    {
                        harpoonClone.GetComponent<Harpoon>().ReleaseHook();
                    }
                }
            }

            //press b to initiate slam attack for the glorious union
            if (InputManager.GetButtonDown("Button B", playerID))
            {
                if (harpoonOut)
                {
                    if (harpoonClone.GetComponent<Harpoon>().GetHooked())
                    {
                        harpoonClone.GetComponent<Harpoon>().SlamAttack();
                    }
                }
            }
        }
        else if (selectedMode == 0)
        {
            #region boost what a fucking mess
            //driver's boost
            if (InputManager.GetAxis("Right Trigger", playerID) == 1 && InputManager.GetAxis("Left Trigger", playerID) == 1) //are the boost buttons being pressed?
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
            else if (InputManager.GetAxis("Right Trigger", playerID) != 1 || InputManager.GetAxis("Left Trigger", playerID) != 1)
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
        }
        #region driver controls
        //rotate clockwise
        if (InputManager.GetAxis("Left Stick Vertical", playerID) < 0 && InputManager.GetAxis("Right Stick Vertical", playerID) > 0)
        {
            Quaternion deltaRotation = Quaternion.Euler((new Vector3(0, 1, 0) * (rotateSpeed)) * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }

        //rotate counterclockwise
        if (InputManager.GetAxis("Left Stick Vertical", playerID) > 0 && InputManager.GetAxis("Right Stick Vertical", playerID) < 0)
        {
            Quaternion deltaRotation = Quaternion.Euler((new Vector3(0, -1, 0) * (rotateSpeed)) * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }

        //move forward
        if (InputManager.GetAxis("Left Stick Vertical", playerID) < 0 && InputManager.GetAxis("Right Stick Vertical", playerID) < 0)
        {
            rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
        }

        //move backward
        if (InputManager.GetAxis("Left Stick Vertical", playerID) > 0 && InputManager.GetAxis("Right Stick Vertical", playerID) > 0)
        {
            rb.MovePosition(transform.position + -transform.forward * Time.deltaTime * speed);
        }

        //left stick only
        if (InputManager.GetAxis("Left Stick Vertical", playerID) < 0 && InputManager.GetAxis("Right Stick Vertical", playerID) == 0)
        {
            rotateRigidBodyAroundPointBy(rb, rightTreadPivot.transform.position, rightTreadPivot.transform.up, (rotateSpeed * Time.deltaTime));
        }
        if (InputManager.GetAxis("Left Stick Vertical", playerID) > 0 && InputManager.GetAxis("Right Stick Vertical", playerID) == 0)
        {
            rotateRigidBodyAroundPointBy(rb, rightTreadPivot.transform.position, rightTreadPivot.transform.up, -(rotateSpeed * Time.deltaTime));
        }

        //right stick only
        if (InputManager.GetAxis("Left Stick Vertical", playerID) == 0 && InputManager.GetAxis("Right Stick Vertical", playerID) > 0)
        {
            rotateRigidBodyAroundPointBy(rb, leftTreadPivot.transform.position, leftTreadPivot.transform.up, (rotateSpeed * Time.deltaTime));
        }
        if (InputManager.GetAxis("Left Stick Vertical", playerID) == 0 && InputManager.GetAxis("Right Stick Vertical", playerID) < 0)
        {
            rotateRigidBodyAroundPointBy(rb, leftTreadPivot.transform.position, leftTreadPivot.transform.up, -(rotateSpeed * Time.deltaTime));
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.U))
        {
            flipTank();
        }

        #region weapon swapping
        if (InputManager.GetButtonDown("Left Bumper", playerID))
        {
            // display the options when pushing left bumper
            playerRoles.DisplayPanel(anim, ammoPanel);
            currentCombo = new List<string>();
        }

        else if (InputManager.GetButtonUp("Left Bumper", playerID) || currentCombo.Count == 4)
        {
            if (currentCombo.Count == 4)
            {
                selectedMode = playerRoles.SelectAmmo(currentCombo, ammoCombos);
                abilityIcon.GetComponent<Image>().sprite = abilityIcons[selectedMode];
                currentCombo = new List<string>();
            }

            // display the options when pushing left bumper
            playerRoles.HidePanel(anim, ammoPanel);
            playerRoles.ResetCombo(comboButtons);
        }

        if (InputManager.GetButton("Left Bumper", playerID))
        {
            // Add buttons to the current combo
            if (InputManager.GetButtonDown("Button A", playerID))
            {
                currentCombo.Add("Button A");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button B", playerID))
            {
                currentCombo.Add("Button B");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button X", playerID))
            {
                currentCombo.Add("Button X");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button Y", playerID))
            {
                currentCombo.Add("Button Y");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
        }

        #endregion

        #region role swapping
        if (InputManager.GetAxis("DPAD Vertical", playerID) == 1)
        {
            playerRoles.SwapToGunner(this);
        }
        else if (InputManager.GetAxis("DPAD Horizontal", playerID) == 1)
        {
            playerRoles.SwapToEngineer(this);
        }
        else if (InputManager.GetAxis("DPAD Horizontal", playerID) == -1)
        {
            playerRoles.SwapToCommander(this);
        }
        #endregion
    }

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
