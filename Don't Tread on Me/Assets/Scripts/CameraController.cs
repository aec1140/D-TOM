using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // target of camera
    public Transform target;
    public float smoothTime = 0.3f;
    public float xOffset = 10;
    public float zOffset = 10;
    public int cameraSpeed = 10;
    public float distanceCap = 100;
    public Camera camera;
    public GameObject waypoint;

    private Vector3 oldPosition;
    private Vector3 oldTargetPosition;

    // Use this for initialization
    void Start()
    {
        oldPosition = transform.position;
        oldTargetPosition = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // follow the tank
        Vector3 follow = target.transform.position - oldTargetPosition;
        transform.Translate(follow, Space.World);

        // old position
        oldPosition = transform.position;

        // apple input
        if (Input.GetKey(KeyCode.I) || Input.GetButton("Y"))
        {
            transform.Translate(new Vector3(1, 0, 1) * cameraSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.K) || Input.GetButton("A"))
        {
            transform.Translate(new Vector3(-1, 0, -1) * cameraSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.J) || Input.GetButton("X"))
        {
            transform.Translate(new Vector3(-1, 0, 1) * cameraSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.L) || Input.GetButton("B"))
        {
            transform.Translate(new Vector3(1, 0, -1) * cameraSpeed * Time.deltaTime, Space.World);
        }

        // check to see if too far away
        Vector3 a = transform.position;
        a.y = 0;

        Vector3 b = target.transform.position;
        b.y = 0;
        b.x = b.x - xOffset;
        b.z = b.z - xOffset;

        // because you cant just add staight to the x and z value of transform position
        Vector3 correction = transform.position;

        // math and stuff

        // Method 1 (rectangular bounds)
        if(Mathf.Abs((a.x - a.z) - (b.x - b.z)) > distanceCap)
        {
            correction.x = oldPosition.x;
            correction.z = oldPosition.z;
        }

        if (Mathf.Abs((a.x + a.z) - (b.x + b.z)) > distanceCap)
        {
            correction.x = oldPosition.x;
            correction.z = oldPosition.z;
        }

        /* Method 2 (circular bounds)
        if(Vector3.Distance(a, b) > distanceCap)
        {
            correction.x = oldPosition.x;
            correction.z = oldPosition.z;
        }
        */

        transform.position = correction;

        // update tank's old position
        oldTargetPosition = target.transform.position;

        // create waypoints
        if (Input.GetButton("LT"))
        {
            CreateWaypoint();
        }
    }

    // waypoint creator
    void CreateWaypoint()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(camera.transform.forward);
        
        if(Physics.Raycast(ray, out hit))
        {
            Instantiate(waypoint, hit.transform);
        }
    }
}
