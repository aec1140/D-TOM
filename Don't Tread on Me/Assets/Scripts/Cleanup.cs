using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleanup : MonoBehaviour {

    public float lifeTime = 3.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
}
