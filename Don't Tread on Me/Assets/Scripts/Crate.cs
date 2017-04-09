using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {

    public float parachuteDrag = 8.0f;
    public float deploymentHeight = 6.0f;
    public float windForce = 1.0f;

    private bool deployed = false;
    private Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
        rigidBody = this.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray landingRay = new Ray(transform.position, Vector3.down);

        if(!deployed)
        {
            if(Physics.Raycast(landingRay, out hit, deploymentHeight))
            {
                deployed = true;
                rigidBody.drag = parachuteDrag;
            }
        }

        Vector3 wind = Vector3.left * windForce;
        rigidBody.AddForce(wind);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else
        {
            rigidBody.drag = 0.0f;
        }
    }
}
