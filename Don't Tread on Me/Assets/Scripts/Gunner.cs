using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class Gunner : MonoBehaviour {
    public GameObject cannon;
    public GameObject cannonPivot;
    private int angleCurrent;

    // for gunner rotation
    Quaternion oldRotation;
    public float gunnerRotateSpeed = 1.0f;

    // for main gunner and active reload
    public GameObject tankTop;
    public GameObject launcher;

    private Rockets rockets = null;

    public GameObject marker;
    public GameObject sweetSpot;
    public GameObject lineRail;
    public GameObject reloadPanel;

    // Weapon Swapping
    public GameObject ammoPanel;

    private bool reloading = false;
    private bool attemptedReload = false;
    private Vector3 initialPos;

    // for ammo
    private float reloadSpeed = 50.0f;
    private List<string> currentCombo = new List<string>();

    private List<string> standard_shot_combo = new List<string> { "Button A", "Button Y", "Button X", "Button X" };
    public GameObject[] standard_shot_buttons = new GameObject[4];

    private List<string> he_shot_combo = new List<string> { "Button Y", "Button B", "Button X", "Button A" };
    public GameObject[] he_shot_buttons = new GameObject[4];

    private List<string> ap_shot_combo = new List<string> { "Button A", "Button A", "Button X", "Button Y" };
    public GameObject[] ap_shot_buttons = new GameObject[4];

    private Dictionary<string, List<string>> ammoCombos = new Dictionary<string, List<string>>();
    private Dictionary<List<string>, GameObject[]> comboButtons = new Dictionary<List<string>, GameObject[]>();

    // for UI
    // http://www.thegamecontriver.com/2014/10/create-sliding-pause-menu-unity-46-gui.html
    public GameObject gunnerPanel;
    private Animator anim;

    private GameObject inputMngr;
    private PlayerRoles playerRoles;
    public PlayerID playerID;

    // Use this for initialization
    void Start () {
        oldRotation = tankTop.transform.rotation;

        // pull in rockets script
        rockets = GetComponent<Rockets>();

        // init active reload
        initialPos = marker.transform.position;

        // init ammo swap
        ammoCombos.Add("default", standard_shot_combo);
        ammoCombos.Add("he", he_shot_combo);
        ammoCombos.Add("ap", ap_shot_combo);

        comboButtons.Add(standard_shot_combo, standard_shot_buttons);
        comboButtons.Add(ap_shot_combo, ap_shot_buttons);
        comboButtons.Add(he_shot_combo, he_shot_buttons);

        // init
        anim = gunnerPanel.GetComponent<Animator>();
        anim.enabled = true;

        inputMngr = GameObject.Find("InputManager");
        playerRoles = inputMngr.GetComponent<PlayerRoles>();

        playerRoles.HidePanel(anim, ammoPanel, reloadPanel);
        playerRoles.SetComboTextures(comboButtons);

        playerID = inputMngr.GetComponent<PlayerRoles>().gunner;
    }
	
	// Update is called once per frame
	void Update () {

        if (playerID != playerRoles.gunner) return;

        #region tank top rotation
        //turn Top of Tank to the right

        if (InputManager.GetAxis("Right Stick Horizontal", playerID) != 0.0f)
        {
            tankTop.transform.Rotate(0f, (InputManager.GetAxis("Right Stick Horizontal", playerID) * gunnerRotateSpeed), 0f);
            oldRotation = tankTop.transform.rotation;
        }
        #endregion

        #region cannon angle
        //point cannon up
        if (InputManager.GetAxis("Left Stick Vertical", playerID) < 0)
        {
            if (angleCurrent < 100)
            {
                cannon.transform.RotateAround(cannonPivot.transform.position, cannonPivot.transform.right, -10 * Time.deltaTime);
                angleCurrent++;
            }
        }

        //point cannon down
        if (InputManager.GetAxis("Left Stick Vertical", playerID) > 0)
        {
            if (angleCurrent > -80)
            {
                cannon.transform.RotateAround(cannonPivot.transform.position, cannonPivot.transform.right, 10 * Time.deltaTime);
                angleCurrent--;
            }
        }
        #endregion

        #region main gun
        if (!reloading)
        {
            // draw line
            // ToDo: this should be a raycast to help see what it is aiming at
            Vector3 forward = launcher.transform.TransformDirection(Vector3.forward) * 20;
            Debug.DrawRay(launcher.transform.position, forward, Color.red);

            if (InputManager.GetAxis("Right Trigger", playerID) == 1)
            {
                // ToDo: ammotype needs to be implemented
                rockets.FireProjectile(0);
                reloading = true;

                playerRoles.DisplayPanel(anim, reloadPanel);
            }
        }

        else if (reloading)
        {
            marker.transform.Translate(Vector3.right * Time.deltaTime * reloadSpeed);

            if (InputManager.GetAxis("Left Trigger", playerID) > 0 && !attemptedReload)
            {

                if (marker.transform.position.x < sweetSpot.transform.position.x + 20.0f - 10 &&
                    marker.transform.position.x + 7.0f > sweetSpot.transform.position.x)
                {
                    marker.transform.position = initialPos;
                    reloading = false;

                    playerRoles.HidePanel(anim, reloadPanel);
                }
                else
                {
                    attemptedReload = true;
                    reloadSpeed = 25;
                }
            }
            if (marker.transform.position.x >= (lineRail.transform.position.x + 109)) // adding half the width of line rail should make this dynamic
            {
                marker.transform.position = initialPos;
                reloading = false;
                attemptedReload = false;
                reloadSpeed = 50;

                playerRoles.HidePanel(anim, reloadPanel);
            }
        }

        #endregion

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
                print(playerRoles.SelectAmmo(currentCombo, ammoCombos)); // right now this just prints but it should be passed into something that changes explosion properties
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

        if (InputManager.GetAxis("DPAD Horizontal", playerID) == 1)
        {
            Commander commander = GetComponent<Commander>();
            commander.playerID = playerID;
            playerID = playerRoles.commander;

            playerRoles.commander = commander.playerID;
            playerRoles.gunner = playerID;
        }
        else  if (InputManager.GetAxis("DPAD Vertical", playerID) == -1)
        {
            Driver driver = GetComponent<Driver>();
            driver.playerID = playerID;
            playerID = playerRoles.driver;

            playerRoles.driver = driver.playerID;
            playerRoles.gunner = playerID;
        }
        else if (InputManager.GetAxis("DPAD Horizontal", playerID) == -1)
        {
            Engineer engineer = GetComponent<Engineer>();
            engineer.playerID = playerID;
            playerID = playerRoles.engineer;

            playerRoles.engineer = engineer.playerID;
            playerRoles.gunner = playerID;
        }
    }
}
