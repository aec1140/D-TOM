using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class rockets : MonoBehaviour {

    // Variable Declaration
    public Rigidbody Projectile = null;
    public Transform Launcher = null;
    private const float SPAWN_DISTANCE = 1.0f;
    public int power = 50;
	public bool rocketTrue;

    public float reloadTime = 0.5f;
    private float timeLast = 0.0f;

    GameObject gunner;
    GameObject rld;
    ActiveReload activeReload;

    // For changing ammo
    public int currentAmmoType = 1;                         // Ammo being Used
    public int selectedAmmoType = 1;                        // Shows what ammo type is highlighted
    private int maxTypes = 4;                               // How many Types of ammo
    private bool isDown = false;                             // NOT NEEDED
    private float timePressed, upTime, pressTime = 0.0f;
    public float ammoButtonHold = 1.0f;                     // How long the button should be held for
    private int p = 0;                                      // Number of inputs done

    private int[] ammoDDR = new int[4];
    private int[] ammo1 = new int[4] { 1, 4, 2, 4 };
    private int[] ammo2 = new int[4] { 2, 3, 4, 1 };
    private int[] ammo3 = new int[4] { 3, 2, 1, 4 };
    private int[] ammo4 = new int[4] { 4, 1, 3, 2 };


    // Use this for initialization
    void Start()
    {
		rocketTrue = true;
        // player = GameObject.FindGameObjectWithTag("Player");
        // playerTank = player.GetComponent<PlayerTank>();

        rld = GameObject.Find("ActiveReload");
        activeReload = rld.GetComponent<ActiveReload>();
    }

    void Update()
    {
        if (true) // we have to fix how this works yo
        {
            if (Input.GetMouseButton(0) || Input.GetAxis("RightTrigger") > 0)
            {
                if (rocketTrue)//(Time.time - timeLast > reloadTime)//(rocketTrue)
                {
                    //rocketTrue = true;
                    FireProjectile(currentAmmoType);
                    rocketTrue = false;
                    //timeLast = Time.time;
                } else if (!rocketTrue) {
                    rld.SetActive(true);
                    activeReload.reloading = true;
                }
                //reload time
            }
            if (Input.GetMouseButton(1))
            {
                if (Time.time - timeLast > reloadTime)
                {
                    rocketTrue = false;
                    FireProjectile(currentAmmoType);
                    timeLast = Time.time;
                }//reload time
            }
        }

        if (Input.GetKeyDown("f") || Input.GetButtonDown("B"))
        {
            print("Current Ammo Type: " + currentAmmoType);
        }

        if (Input.GetKey("f") || Input.GetButton("B"))
        {
            CheckInputs();
        }
        if (Input.GetKeyUp("f") || Input.GetButtonUp("B"))
        {
            p = 0;
            print("Canceled Ammo Change");
        }

    }
	
	void FireProjectile(int type)
	{
		Rigidbody clone;
		if (type == 0) 
		{
			clone = Instantiate (Projectile, Launcher.transform.position + (SPAWN_DISTANCE * Launcher.transform.forward), Launcher.transform.rotation) as Rigidbody;
		} 
		else 
		{
			clone = Instantiate (Projectile, Launcher.transform.position + (SPAWN_DISTANCE * Launcher.transform.forward), Launcher.transform.rotation) as Rigidbody;
		}
        clone.velocity = transform.TransformDirection(Vector3.forward * power);
        Explode explo = (Explode)clone.gameObject.AddComponent(typeof(Explode)); //clone.AddComponent<Explode>();
        //Destroy(clone);
    }


    // Changes ammo types
    void CheckInputs()
    {

        if (Input.GetKeyDown("g"))   //Input.GetAxis("DPadX") == -1)
        { // Left
            ammoDDR[p] = 1;
            print("1");
            p++;
        }
        else if (Input.GetKeyDown("y"))   //Input.GetAxis("DPadY") == -1)
        { // Up
            ammoDDR[p] = 2;
            print("2");
            p++;
        }
        else if (Input.GetKeyDown("j"))   //Input.GetAxis("DPadX") == 1)
        { // Right
            ammoDDR[p] = 3;
            print("3");
            p++;
        }
        else if (Input.GetKeyDown("h"))   //Input.GetAxis("DPadY") == 1)
        { // Down
            ammoDDR[p] = 4;
            print("4");
            p++;
        }

        // if (4) inputs check if DDR Array equals one of the Ammo Arrays
        if (p == 4)
        {
            if (ammoDDR.SequenceEqual(ammo1))           // Check if changed to Ammo Type: 
            {
                // change to ammo type 1
                currentAmmoType = 1;
                print("Changed to AmmoType 1");
            }
            else if (ammoDDR.SequenceEqual(ammo2))    // Check if changed to Ammo Type:
            {
                // change to ammo type 2
                currentAmmoType = 1;
                print("Changed to AmmoType 2");
            }
            else if (ammoDDR.SequenceEqual(ammo3))    // Check if changed to Ammo Type:
            {
                // change to ammo type 3
                currentAmmoType = 1;
                print("Changed to AmmoType 3");
            }
            else if (ammoDDR.SequenceEqual(ammo4))    // Check if changed to Ammo Type:
            {
                // change to ammo type 4
                currentAmmoType = 1;
                print("Changed to AmmoType 4");
            }
            else                                      // Not valid Input Code
            {
                print("Not valid Input Code");
                printArray();
            }
            p = 0;
        }

    }

    // Prints DDR Array
    void printArray()
    {
        for (int i = 0; i < ammoDDR.Length; i++)
        {
            print("Input " + i + ": " + ammoDDR[i]);
        }
    }

}
