using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public GameObject[] enemies = new GameObject[3];
    public float respawnTime = 5.0f;

    private float timeLast = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time - timeLast > respawnTime)
        {
            Instantiate(enemies[Random.Range(0, 2)], this.gameObject.transform.position, Quaternion.identity);
            timeLast = Time.time;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}
