using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

public class Commander : MonoBehaviour {

    public GameObject follow;
    public GameObject crate;
    public GameObject shield;
    public GameObject mark;
    public GameObject airstrike;
    public GameObject target;
    public GameObject reticle;
    public GameObject reticleChild1;
    public GameObject reticleChild2;

    public float translateSpeed = 10.0f;
    public float reticleRotateSpeed = 2.0f;
    public float crateHeight = 100.0f;
    public float timerCap = 1.0f;
    public float airstrikeHeight = 100.0f;

    // Private Properties
    public float rotateSpeed = 1;
    public float deadZone = 0.1f;
    public GameObject bullet;
    public GameObject tracer;
    public GameObject firePoint;
    public float bulletSpeed = 20.0f;
    public float reloadTime = 0.5f;
    public int tracerCap = 4;
    public GameObject gun;

    private float timeLast = 0.0f;
    private int tracerCounter = 0;

    private float oldRetX = 0;
    private float oldRetY = 0;

    private float oldRotation;
    private Vector3 oldTargetPosition;
    private float timerLast = 0.0f;

    private LineRenderer myLineRenderer;

    // mode switching
    public GameObject abilityIcon;
    public Sprite[] abilityIcons = new Sprite[3];


    public enum AmmoTypes { CarePackage, Shield, Airstrike };
    private int selectedMode = (int)AmmoTypes.CarePackage;

    public GameObject commanderPanel;
    public GameObject ammoPanel;
    private Animator anim;

    private List<string> currentCombo = new List<string>();

    private List<string> care_package_combo = new List<string> { "Button A", "Button B", "Button X", "Button X" };
    public GameObject[] care_package_buttons = new GameObject[4];

    private List<string> shield_combo = new List<string> { "Button Y", "Button B", "Button X", "Button A" };
    public GameObject[] shield_buttons = new GameObject[4];

    private List<string> airstrike_combo = new List<string> { "Button A", "Button A", "Button X", "Button Y" };
    public GameObject[] airstrike_buttons = new GameObject[4];

    private Dictionary<AmmoTypes, List<string>> ammoCombos = new Dictionary<AmmoTypes, List<string>>();
    private Dictionary<List<string>, GameObject[]> comboButtons = new Dictionary<List<string>, GameObject[]>();

    // player role swapping
    private GameObject inputMngr;
    private PlayerRoles playerRoles;
    public PlayerID playerID;

    public GameObject playerIcon;

    // Use this for initialization
    void Start () {

        oldTargetPosition = follow.transform.position;
        oldRotation = gun.transform.parent.transform.localEulerAngles.y;
        myLineRenderer = reticle.GetComponent<LineRenderer>();

        // init ammo swap
        ammoCombos.Add(AmmoTypes.CarePackage, care_package_combo);
        ammoCombos.Add(AmmoTypes.Shield, shield_combo);
        ammoCombos.Add(AmmoTypes.Airstrike, airstrike_combo);

        comboButtons.Add(care_package_combo, care_package_buttons);
        comboButtons.Add(shield_combo, shield_buttons);
        comboButtons.Add(airstrike_combo, airstrike_buttons);

        // Init
        anim = commanderPanel.GetComponent<Animator>();
        anim.enabled = true;

        inputMngr = GameObject.Find("InputManager");
      
        playerRoles = inputMngr.GetComponent<PlayerRoles>();
        playerRoles.HidePanel(anim);
        playerRoles.SetComboTextures(comboButtons);

        playerID = inputMngr.GetComponent<PlayerRoles>().commander;

        abilityIcon.GetComponent<Image>().sprite = abilityIcons[selectedMode];
    }
	
	// Update is called once per frame
	void Update () {

        if (playerID != inputMngr.GetComponent<PlayerRoles>().commander) return;

        playerIcon.GetComponent<Image>().sprite = playerRoles.helmets[(int)playerID];

        // follow the tank
        Vector3 temp = follow.transform.position - oldTargetPosition;
        target.transform.Translate(temp, Space.World);

        // movement input
        if (InputManager.GetAxis("Left Stick Vertical", playerID) != 0.0f)
        {
            target.transform.Translate(new Vector3(1, 0, 1) * -InputManager.GetAxis("Left Stick Vertical", playerID) * translateSpeed  * Time.deltaTime, Space.World);
        }
        if (InputManager.GetAxis("Left Stick Horizontal", playerID) != 0.0f)
        {
            target.transform.Translate(new Vector3(-1, 0, 1) * -InputManager.GetAxis("Left Stick Horizontal", playerID) * translateSpeed * Time.deltaTime, Space.World);
        }

        // update tank's old position
        oldTargetPosition = follow.transform.position;

        // update position of commander target
        int layerMask = 1 << 8;
        RaycastHit hit;

        Ray ray = new Ray(target.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            reticle.transform.position = hit.point;
        }

        AdjustReticles();

        // manage trigger input
        if (InputManager.GetAxis("Left Trigger", playerID) > 0)
        {
            Activate();
        }
        else
        {
            UpdateGun();
            Mark();
        }

        // switch current mode
        #region weapon swapping
        if (InputManager.GetButtonDown("Left Bumper", playerID))
        {
            // display the options when pushing left bumper
            playerRoles.DisplayPanel(anim, ammoPanel);
            currentCombo = new List<string>();
        }

        else if (InputManager.GetButtonUp("Left Bumper", playerID) || currentCombo.Count == 4)
        {
            if (currentCombo.Count == 4)
            {
                selectedMode = playerRoles.SelectAmmo(currentCombo, ammoCombos);
                abilityIcon.GetComponent<Image>().sprite = abilityIcons[selectedMode];
                currentCombo = new List<string>();
            }

            // display the options when pushing left bumper
            playerRoles.HidePanel(anim, ammoPanel);
            playerRoles.ResetCombo(comboButtons);
        }

        if (InputManager.GetButton("Left Bumper", playerID))
        {
            // Add buttons to the current combo
            if (InputManager.GetButtonDown("Button A", playerID))
            {
                currentCombo.Add("Button A");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button B", playerID))
            {
                currentCombo.Add("Button B");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button X", playerID))
            {
                currentCombo.Add("Button X");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
            else if (InputManager.GetButtonDown("Button Y", playerID))
            {
                currentCombo.Add("Button Y");
                playerRoles.DisplayCombo(currentCombo, comboButtons);
            }
        }

        #endregion

        if (InputManager.GetAxis("DPAD Vertical", playerID) == 1)
        {
            playerRoles.SwapToGunner(this);
        }
        else if (InputManager.GetAxis("DPAD Horizontal", playerID) == 1)
        {
            playerRoles.SwapToEngineer(this);
        }
        else if (InputManager.GetAxis("DPAD Vertical", playerID) == -1)
        {
            playerRoles.SwapToDriver(this);
        }
    }

    // Update Commander Gun
    void UpdateGun()
    {
        float x = InputManager.GetAxis("Right Stick Horizontal", playerID);
        float y = InputManager.GetAxis("Right Stick Vertical", playerID);

        float newRotation = gun.transform.parent.transform.localEulerAngles.y;
        float test = oldRotation - newRotation;

        float angle = 0f;

        if (Mathf.Abs(x) >= deadZone || Mathf.Abs(y) >= deadZone)
        {
            angle = Mathf.Atan2(x, -y);
            Fire();

            Quaternion destination = Quaternion.EulerAngles(0, angle, 0) * Quaternion.AngleAxis(45, gun.transform.parent.transform.up);
            gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, destination, Time.deltaTime * rotateSpeed);
        }

        gun.transform.Rotate(0f, test, 0f);

        oldRotation = gun.transform.parent.transform.localEulerAngles.y;
    }

    // Fire function
    void Fire()
    {
        if (Time.time - timeLast > reloadTime)
        {
            GameObject newBullet;

            if (tracerCounter == tracerCap)
            {
                newBullet = Instantiate(tracer, firePoint.transform.position, gun.transform.rotation);
                tracerCounter = 0;
            }
            else
            {
                newBullet = Instantiate(bullet, firePoint.transform.position, gun.transform.rotation);
                tracerCounter++;
            }

            newBullet.GetComponent<Rigidbody>().AddForce(gun.transform.forward * bulletSpeed);
            timeLast = Time.time;
        }
    }

    // Manages right trigger activation
    void Activate()
    {
        /*
        float x = InputManager.GetAxis("Right Stick Horizontal", playerID);
        float y = InputManager.GetAxis("Right Stick Vertical", playerID);

        float angle;

        if (Mathf.Abs(x) < deadZone && Mathf.Abs(y) < deadZone)
        {
            angle = Mathf.Atan2(oldRetX, -oldRetY);
        }
        else
        {
            oldRetX = x;
            oldRetY = y;
            angle = Mathf.Atan2(x, -y);
        }

        Quaternion destination = Quaternion.EulerAngles(0, angle, 0) * Quaternion.AngleAxis(45, Vector3.down);
        reticle.transform.rotation = Quaternion.Lerp(reticle.transform.rotation, destination, Time.deltaTime * rotateSpeed);
        */

        reticle.transform.Rotate(0f, (InputManager.GetAxis("Right Stick Horizontal", playerID) * reticleRotateSpeed), 0f);

        if (InputManager.GetAxis("Right Trigger", playerID) > 0)
        {
            if (Time.time - timerLast > timerCap)
            {
                if (selectedMode == (int)AmmoTypes.CarePackage)
                {
                    Vector3 crateStart = target.transform.position;
                    crateStart.y = crateHeight;
                    Instantiate(crate, crateStart, reticle.transform.rotation);
                    timerLast = Time.time;
                }
                if(selectedMode == (int)AmmoTypes.Shield)
                {
                    Instantiate(shield, reticle.transform.position, reticle.transform.rotation);
                    timerLast = Time.time;
                }
                if(selectedMode == (int)AmmoTypes.Airstrike)
                {
                    Vector3 strikeStart = target.transform.position;
                    strikeStart.y = airstrikeHeight;
                    Instantiate(airstrike, strikeStart, reticle.transform.rotation);
                    timerLast = Time.time;
                }
            }
        }
    }

    // mark the target
    void Mark()
    {
        if (InputManager.GetAxis("Right Trigger", playerID) > 0)
        {
            if (Time.time - timerLast > timerCap)
            {
                RaycastHit hit;

                Ray ray = new Ray(target.transform.position, Vector3.down);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.tag != "Plane")
                    {
                        GameObject marker = Instantiate(mark, hit.point, Quaternion.identity);
                        marker.transform.SetParent(hit.transform);
                        marker.transform.localPosition = new Vector3(0, 2 * (1 / hit.transform.localScale.y), 0);
                    }
                }
                timerLast = Time.time;
            }
        }
    }

    // adjust position of reticles
    void AdjustReticles()
    {
        float x = 0.0f;

        if (InputManager.GetAxis("Left Trigger", playerID) > 0.0f)
        {
            if (selectedMode == (int)AmmoTypes.CarePackage)
            {
                x = 0.25f;
            }
            if (selectedMode == (int)AmmoTypes.Shield)
            {
                x = 0.5f;
            }
            if (selectedMode == (int)AmmoTypes.Airstrike)
            {
                x = 3.0f;
            }
        }

        reticleChild1.transform.localPosition = Vector3.Lerp(reticleChild1.transform.localPosition, new Vector3(-x, 0, 0), Time.deltaTime * 3);
        reticleChild2.transform.localPosition = Vector3.Lerp(reticleChild2.transform.localPosition, new Vector3(x, 0, 0), Time.deltaTime * 3);
        myLineRenderer.SetPosition(0, reticleChild1.transform.localPosition);
        myLineRenderer.SetPosition(1, reticleChild2.transform.localPosition);
    }
}
