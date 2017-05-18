using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    public GameObject player;
    GameObject hit;

    LineRenderer line;
    private bool hooked;
    public bool GetHooked()
    {
        return hooked;
    }

    float timeAlive;
    float startTime;

    float lineLength;
    float lineLengthGivePlus;
    float lineLengthGiveMinus;
    bool SlamActive;
    public bool GetSlamActive()
    {
        return SlamActive;
    }
    public void SetSlamActive(bool s)
    {
        SlamActive = s;
    }

    void Start()
    {
        player = GameObject.Find("Player");
        line = GetComponent<LineRenderer>();

        startTime = Time.time;
        SlamActive = false;
    }

    void Update()
    {
        //if hooked, display the line between tank and harpoon
        if (hooked)
        {
            line.SetPosition(0, player.transform.position + (1 * new Vector3(0,1,0)));
            line.SetPosition(1, transform.position + (1 * -transform.up));

            //simulates a towing effect if the distance between the hook and player is bigger than the 110% of the original distance
            if (Vector3.Distance(this.transform.position, player.transform.position) > lineLengthGivePlus)
            {
                this.gameObject.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position) * 50, ForceMode.Force); //force is best
            }
        }

        //auto destroy in 5 seconds if you haven't hit anything
        timeAlive = Time.time - startTime;
        if (timeAlive > 5 && !hooked)
        {
            print("no target, destroying harpoon");
            Destroy(this.gameObject);
            player.gameObject.GetComponent<Driver>().SetHarpoonOut(false);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        hit = other.gameObject;
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        //does it have a rigidbody
        if (rb != null)
        {
            //attach harpoon to object
            var joint = AddFixedJoint();
            joint.connectedBody = hit.GetComponent<Rigidbody>();

            //harpoon is in something, so turn off its collider
            hooked = true;
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

            line.widthMultiplier = 0.1f;
        }

        //length of line connecting the hook and the player
        lineLength = Vector3.Distance(this.transform.position, player.transform.position);
        //get the slack in the line
        lineLengthGivePlus = lineLength * 1.1f;
        lineLengthGiveMinus = lineLength * .9f;

        //if the collider's gameobject has the script HP
        if ((hit.gameObject.GetComponent("HP") as HP))
        {
            hit.gameObject.GetComponent<HP>().SetHookedHP(true);
            //hit.gameObject.GetComponent<HP>().TakeDamage(damage);
        }
    }

    //call when you want to destroy the hook + release the hooked object
    public void ReleaseHook()
    {
        //destroy harpoon
        Destroy(this.gameObject);
        //set driver's harpoon bool to false
        //player.gameObject.GetComponent<Driver>().SetHarpoonOut(false);
        //if the object had HP, set the HP's hooked bool to false, and reset its script
        if (hit)
        {
            if ((hit.gameObject.GetComponent("HP") as HP))
            {
                hit.gameObject.GetComponent<HP>().SetHookedHP(false);
                hit.gameObject.GetComponent<HP>().UnhookEnemy();
            }
        }
    }

    public void SlamAttack()
    {
        SlamActive = true;
        this.gameObject.GetComponent<Rigidbody>().AddForce((player.transform.position - this.transform.position) * 100, ForceMode.Impulse);
        player.gameObject.GetComponent<Rigidbody>().AddForce((this.transform.position - player.transform.position) * 100, ForceMode.Impulse);
        //ReleaseHook();
    }

    //helper will attach fixed joints to stuff
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
}
