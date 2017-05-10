using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TeamUtility.IO;

public class Rockets : MonoBehaviour {

    private PlayerID gunner;

    // Variable Declaration
    public Rigidbody projectile = null;
    public Transform launcher = null;
    public GameObject tankTop;
    private const float SPAWN_DISTANCE = 1.0f;
    public int power = 50;

    public Rigidbody[] ammoTypes = new Rigidbody[3];

    public float reloadTime = 0.5f;
    private float timeLast = 0.0f;

    // Use this for initialization
    void Start()
    {
        // this code is for managing player roles
        GameObject inputMngr = GameObject.Find("InputManager");
        gunner = inputMngr.GetComponent<PlayerRoles>().gunner;
    }

    void Update()
    {      

    }
	
	public void FireProjectile(int type)
	{
		Rigidbody clone;

        Rigidbody selectedAmmo = ammoTypes[type];
    
        clone = Instantiate(selectedAmmo, launcher.transform.position + (SPAWN_DISTANCE * launcher.transform.forward), launcher.transform.rotation) as Rigidbody;
        clone.velocity = tankTop.transform.TransformDirection(Vector3.forward * power);
        //Explode explo = (Explode)clone.gameObject.AddComponent(typeof(Explode)); //clone.AddComponent<Explode>();
        //Destroy(clone);
    }
}
