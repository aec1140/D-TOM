using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour {

    public float MaxHP;
    private float CurrHP;

    //is the attached object hooked
    private bool hookedHP;
    public void SetHookedHP(bool h)
    {
        hookedHP = h;
    }

    public float getCurrHP()
    { return CurrHP; }

	// Use this for initialization
	void Start () {
        CurrHP = MaxHP;
	}

    //if hooked & an enemy, disable enemy script 
    void OnCollisionEnter(Collision other)
    {
        if (hookedHP && this.gameObject.GetComponent("EnemyTank") as EnemyTank)
        {
            this.gameObject.GetComponent<EnemyTank>().enabled = false;
        }
    }

    //enable enemy script
    public void UnhookTank()
    {
        this.gameObject.GetComponent<EnemyTank>().enabled = true;
    }

    public void TakeDamage(float damage)
    {
        //subtract damage from HP
        CurrHP -= damage;

        //if no HP
        if (CurrHP <= 0)
        {
            //destroy the tank
            if (tag == "Player") Destroy(this.gameObject);
        }
    }//take damage

    public void Repair(float heal)
    {
        CurrHP += heal;
        if (CurrHP > MaxHP)
        {
            CurrHP = MaxHP;
        }
    }
}
