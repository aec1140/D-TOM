using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadCollide : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerStay(Collider other)
    {
        print("COLLIDING");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        print("COLLIDING");
    }
}
