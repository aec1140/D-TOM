using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : MonoBehaviour {

    public bool amEngineer;

    public GameObject Hull;
    public Rigidbody Projectile = null;
    Camera mainCamera;
    private const float SPAWN_DISTANCE = 2f;

    float throwPower;
    public float throwRate;
    float maxPower;

    HP hp;
    GameObject pTnk;
    bool repairing;
    int repairAmount = 1;

	// Use this for initialization
	void Start () {
        throwRate = 8f;
        maxPower = 8f;
        repairing = false;

        pTnk = GameObject.Find("Player");
        hp = pTnk.GetComponent<HP>();
	}
	
	// Update is called once per frame
	void Update () {

        if (amEngineer)
        {

            if (!repairing)
            {
                if (Input.GetAxis("RightThumbStick") != 0.0f)
                {
                    this.transform.Rotate(0f, Input.GetAxis("RightThumbStick"), 0f);
                }
        
                //print(Input.GetAxis("LeftTrigger"));
                if (Input.GetButton("B"))
                {
                    if (throwPower <= maxPower)
                    {
                        throwPower += Time.deltaTime * throwRate;
                    }
                    print("Current PWR: " + throwPower);
                    //print(Input.GetAxis("LeftTrigger"));
                }
                else if (Input.GetButtonUp("B"))
                {
                    ThrowGrenade(throwPower);
                    print("FINALE PWR: " + throwPower);
                    throwPower = 0f;
                }
            }

            // Hold to repair
            if(Input.GetAxis("LeftTrigger") > 0)
            {
                repairing = true;
                //playerTank.HP += Time.deltaTime * 2f;
                //print(playerTank.HP);
            }  
            // OR Mash a button to repair (while holding LeftTrigger to disable use of other actions)
            /*if (Input.GetButtonDown("A") || repairing)
            {
                playerTank.HP += repairAmount;
                print(playerTank.HP);
            }*/
        }

	}


    void ThrowGrenade(float tPWR)
    {
        Rigidbody clone = Instantiate(Projectile, Hull.transform.position + (SPAWN_DISTANCE * Hull.transform.forward), this.transform.rotation) as Rigidbody;  //Hull.transform.rotation) as Rigidbody;

        Vector3 temp = new Vector3(0, 1, 1);

        clone.velocity = transform.TransformDirection( temp  * tPWR);

        Explode explo = (Explode)clone.gameObject.AddComponent(typeof(Explode));
    }


}
