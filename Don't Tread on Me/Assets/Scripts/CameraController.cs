using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // target of camera
    public Transform target;
    public float smoothTime = 0.3f;
    public float xOffset = 10;
    public float zOffset = 10;

    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // follow code
        Vector3 goalPos = target.position;
        goalPos.x = goalPos.x - xOffset;
        goalPos.z = goalPos.z - xOffset;
        goalPos.y = transform.position.y;
        transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        transform.LookAt(target);
    }
}
