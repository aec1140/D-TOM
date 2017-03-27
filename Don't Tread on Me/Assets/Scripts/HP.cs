using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour {

    public float MaxHP;
    private float CurrHP;

	// Use this for initialization
	void Start () {
        CurrHP = MaxHP;
	}

    public void TakeDamage(float damage)
    {
        //subtract damage from HP
        CurrHP -= damage;
        print(this.gameObject.name + " current HP: " + CurrHP);

        //if no HP
        if (CurrHP <= 0)
        {
            //destroy the tank
            print("She's dead, Jim");
        }
    }//take damage
}
