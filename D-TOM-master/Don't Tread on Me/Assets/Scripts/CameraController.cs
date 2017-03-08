using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // target of camera
    public Transform target;
    public float smoothTime = 0.3f;
    public float xOffset = 10f;
    public float zOffset = 10f;
    public float cameraSpeed = 10f;
    public float distanceCap = 100f;
    public Camera mainCamera;
    public GameObject waypoint;
    public GameObject marker;
    public bool amCommander = true;
    public float reloadTime = 1.0f;

    private GameObject currentWaypoint;
    private float timeLast = 0.0f;
    private Vector3 oldPosition;
    private Vector3 oldTargetPosition;
    private float inputStrength = 1.0f;
    private const float INPUT_STRENGTH = 1.0f;

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

        // only take input if commander
        if (amCommander)
        {
            // apple input
            if (Input.GetAxis("LeftThumbStick") != 0.0f)
            {
                transform.Translate(new Vector3(1, 0, 1) * -Input.GetAxis("LeftThumbStick") * cameraSpeed * inputStrength * Time.deltaTime, Space.World);
            }
            if (Input.GetAxis("LeftThumbHorizontal") != 0.0f)
            {
                transform.Translate(new Vector3(-1, 0, 1) * -Input.GetAxis("LeftThumbHorizontal") * cameraSpeed * inputStrength * Time.deltaTime, Space.World);
            }
        }

        // check to see if too far away
        Vector3 a = transform.position;
        a.y = 0;

        Vector3 b = target.transform.position;
        b.y = 0;
        b.x = b.x - xOffset;
        b.z = b.z - xOffset;

        Vector3 c = oldPosition;
        c.y = 0;

        // because you cant just add staight to the x and z value of transform position
        Vector3 correction = transform.position;

        /*
        // Method 1 (rectangular bounds)
        if (Mathf.Abs((a.x - a.z) - (b.x - b.z)) > distanceCap)
        {
            correction.x = oldPosition.x;
            correction.z = oldPosition.z;
        }

        if (Mathf.Abs((a.x + a.z) - (b.x + b.z)) > distanceCap)
        {
            correction.x = oldPosition.x;
            correction.z = oldPosition.z;
        }
        */

        /*
        if(Vector3.Distance(a, b) > distanceCap)
        {
            inputStrength = inputStrength * 0.90f;

            if (Vector3.Distance(a, b) < Vector3.Distance(c, b))
            {
                inputStrength = INPUT_STRENGTH;
            }

        }
        */

        transform.position = correction;

        // update tank's old position
        oldTargetPosition = target.transform.position;

        // create waypoints - in progress
        if (amCommander)
        {
            if (Input.GetAxis("LeftTrigger") > 0)
            {
                if (Time.time - timeLast > reloadTime)
                {
                    CreateWaypoint();
                    timeLast = Time.time;
                }
            }

            if (Input.GetAxis("RightTrigger") > 0)
            {
                if (Time.time - timeLast > reloadTime)
                {
                    MarkTarget();
                    timeLast = Time.time;
                }
            }
        }
    }

    // waypoint creator
    void CreateWaypoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject.tag == "Plane")
            {
                currentWaypoint = Instantiate(waypoint, hit.point, Quaternion.identity);
            }
        }
    }

    void MarkTarget()
    {
        RaycastHit hit;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward, Color.green);

        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject.tag != "Plane")
            {
                currentWaypoint = Instantiate(marker, hit.point, Quaternion.identity);
                currentWaypoint.transform.SetParent(hit.transform);
            }
        }
    }
}