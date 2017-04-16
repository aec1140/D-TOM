using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour {
    private ParticleSystem psystem;
	// Use this for initialization
	void Start () {
        psystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (psystem)
        {
            if (!psystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
	}
}
