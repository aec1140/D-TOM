using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public GameObject Projectile = null;
    public float force = 15; // for AddExplosionForce - explosionForce
    Vector3 pos; //// for AddExplosionForce - explosionPosition
    public float radius = 10; // for AddExplosionForce - explosionRadius
    public float upMod = 1; // for AddExplosionForce - upwardsModifier - leaving this at zero, so that the explosion force will be easier to control and utilize
    public ForceMode fMode = ForceMode.Impulse; // for AddExplosionForce - ForceMode - 4 options: Force, Acceleration, Impulse and VelocityChange, no idea which is best

    public float damageDropoff = 14;

    //public float forceV = 30; // for AddExplosionForce - explosionForce
    //public float radiusV = 15; // for AddExplosionForce - explosionRadius
    //public ForceMode fModeV = ForceMode.VelocityChange;

    //Vector3 TossDirection;
    //public float speed = 10;

    //GameObject thing;
    //public rockets rocketa;

    GameObject player;
    PlayerTank playertank;

     public ParticleSystem explosion;
    
	// Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playertank = player.GetComponent<PlayerTank>();

        // explosion = GetComponent<ParticleSystem>();
        // explosion.playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update () {
        //OnCollisionEnter();
        //Destroy(gameObject, 5);

	}

    //collision - rocket with anything else
    void OnCollisionEnter(Collision other)
    {

        pos = transform.position; //should be the projectile itself
		Collider[] colliders = Physics.OverlapSphere(pos, radius);
		foreach (Collider hit in colliders)
		{
            Rigidbody rb = hit.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.AddExplosionForce(force, pos, radius, upMod, fMode);

			}//if
            #region old hp method
            //this method sucks. find a better way to do it
            //if the thing it hit was a tank
            //if (hit.gameObject.tag == "Player")
            //{
            //    //calculate damage based on distance to explosion epicenter
            //    float damage = 15 - (damageDropoff * ((Vector3.Distance(hit.gameObject.transform.position, pos))/radius));
            //    print("Damage: " + damage);
            //    //I have no idea how to actually make the hit.gameObject take the damage
            //    if (playertank.HP > 0) {
            //        playertank.TakeDamage(damage);
            //    }
            //}
            #endregion
            //if the collider's gameobject has the script HP
            if ((hit.gameObject.GetComponent("HP") as HP) != null)
            {
                //calculate individual damage
                float damage = 15 - (damageDropoff * ((Vector3.Distance(hit.gameObject.transform.position, pos)) / radius));

                //call the objects takeDamage method
                hit.gameObject.GetComponent<HP>().TakeDamage(damage);
            }

                    /*
				else if (hit.transform.gameObject.tag == "Player")
                {
                    TossDirection = hit.transform.position - pos;
                    //hit.transform.gameObject.GetComponent<CharacterController>().Move(TossDirection * speed); //error - script is trying to access character controller of capsule which doesn't exist
                    //hit.gameObject.GetComponent<CharacterController>().Move(TossDirection * speed); //error - script is trying to access character controller of capsule which doesn't exist
                    //hit.transform.parent.gameObject.GetComponent<CharacterController>().Move(TossDirection * speed); //error - no character controller attached to firstpersoncontroller. We have to go deeper.
                    hit.transform.parent.parent.gameObject.GetComponent<CharacterController>().Move(TossDirection * speed); //holy shit it actually works
                    //hit.transform.parent.transform.parent.gameObject.GetComponent<CharacterController>().Move(TossDirection * speed);
                }*/
           }//foreach

        BlowTheFuckUp();
        //GameObject aftermath = (GameObject)Instantiate(explosion, transform.position, transform.rotation);
        //explosion.Play();
        //Destroy(gameObject);
        //Destroy(gameObject); // destroys the projectile after impact

    }

    void BlowTheFuckUp()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
