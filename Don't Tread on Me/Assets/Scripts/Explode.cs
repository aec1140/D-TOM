using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public float force = 15; // for AddExplosionForce - explosionForce
    Vector3 pos; //// for AddExplosionForce - explosionPosition
    public float radius = 8; // for AddExplosionForce - explosionRadius
    public float upMod = 1; // for AddExplosionForce - upwardsModifier - leaving this at zero, so that the explosion force will be easier to control and utilize
    public ForceMode fMode = ForceMode.Impulse; // for AddExplosionForce - ForceMode - 4 options: Force, Acceleration, Impulse and VelocityChange, no idea which is best

    public float damageDropoff = 14;

     public ParticleSystem explosion;
    
    void Start () {

	}
	
	void Update () {

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
             //if the collider's gameobject has the script HP
            if ((hit.gameObject.GetComponent("HP") as HP) != null)
            {
                //calculate individual damage
                float damage = force - (damageDropoff * ((Vector3.Distance(hit.gameObject.transform.position, pos)) / radius));

                //call the objects takeDamage method
                hit.gameObject.GetComponent<HP>().TakeDamage(damage);
            }
        }//foreach

        BlowTheFuckUp();
    }

    void BlowTheFuckUp()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
