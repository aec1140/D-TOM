using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TeamUtility.IO;

public class rockets : MonoBehaviour {

    private PlayerID gunner;

    // Variable Declaration
    public Rigidbody projectile = null;
    public Transform launcher = null;
    public GameObject tankTop;
    private const float SPAWN_DISTANCE = 1.0f;
    public int power = 50;
	public bool rocketTrue;

    public float reloadTime = 0.5f;
    private float timeLast = 0.0f;

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
        // this code is for managing player roles
        GameObject inputMngr = GameObject.Find("InputManager");
        gunner = inputMngr.GetComponent<PlayerRoles>().gunner;
    }

    void Update()
    {      
        if (InputManager.GetButtonDown("Button B", gunner))
        {
            print("Current Ammo Type: " + currentAmmoType);
        }

        if (InputManager.GetButton("Button B", gunner))
        {
            CheckInputs();
        }
        if (InputManager.GetButtonUp("Button B", gunner))
        {
            p = 0;
            print("Canceled Ammo Change");
        }

    }
	
	public void FireProjectile(int type)
	{
		Rigidbody clone;
		if (type == 0) 
		{
			clone = Instantiate (projectile, launcher.transform.position + (SPAWN_DISTANCE * launcher.transform.forward), launcher.transform.rotation) as Rigidbody;
		} 
		else 
		{
			clone = Instantiate (projectile, launcher.transform.position + (SPAWN_DISTANCE * launcher.transform.forward), launcher.transform.rotation) as Rigidbody;
		}
        clone.velocity = tankTop.transform.TransformDirection(Vector3.forward * power);
        //Explode explo = (Explode)clone.gameObject.AddComponent(typeof(Explode)); //clone.AddComponent<Explode>();
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
