using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public GameObject[] enemies = new GameObject[3];
    public float respawnTime = 5.0f;
    public int health = 10;

    private GameObject player;
    private float timeLast = 0.0f;
    private bool shouldSpawn = true;
    private Vector3 downPosition;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        downPosition = this.gameObject.transform.position;
        downPosition.y -= 5;
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time - timeLast > respawnTime && player &&shouldSpawn)
        {
            Instantiate(enemies[Random.Range(0, 2)], this.gameObject.transform.position, Quaternion.identity);
            timeLast = Time.time;
        }

        if(health <= 0)
        {
            shouldSpawn = false;
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, downPosition, Time.deltaTime);
        }

        if(this.gameObject.transform.position == downPosition)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }

        if(collision.gameObject.tag == "Projectile")
        {
            health -= 1;
        }
    }
}
