using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeMagnet : MonoBehaviour {



    public Rigidbody Projectile = null;

    float radius = 8f;
    Vector3 pos;

    public float grav = 5f;

    public AudioClip inspiration;
    private AudioSource source;

    float upTime = 3f;
    private float timeLast = 0.0f;

    private Collider[] colliders;

    bool activation;

    public ParticleSystem explosion;

    // Use this for initialization
    void Start () {
        activation = false;
        timeLast = Time.time;

        source = GetComponent<AudioSource>();

        source.PlayOneShot(inspiration, 1F);

    }
	
	// Update is called once per frame
	void Update () {

        if (activation)
        {        

            foreach (Collider hit in colliders)
            {
                if ((hit.gameObject.GetComponent("Rigidbody") as Rigidbody) != null)
                {
                    hit.transform.position = Vector3.MoveTowards(hit.transform.position, transform.position, 5f * Time.deltaTime);
                }

                if(Time.time - timeLast > upTime)
                {
                    Destroy(this.gameObject);
                }

            }

            //BlowTheFuckUp();
        }


	}


    private void OnCollisionEnter(Collision collision)
    {
        
        colliders = Physics.OverlapSphere(transform.position, radius);

        Projectile.constraints = RigidbodyConstraints.FreezePosition;

        activation = true;

        
    }


    void BlowTheFuckUp()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        //Destroy(this.gameObject);
    }

}
