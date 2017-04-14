using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TeamUtility.IO;

public class PlayerRoles : MonoBehaviour {

    public PlayerID commander;
    public PlayerID gunner;
    public PlayerID driver;
    public PlayerID engineer;

    public Texture a;
    public Texture b;
    public Texture x;
    public Texture y;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetComboTextures(Dictionary<List<string>, GameObject[]> combos)
    {
        foreach (KeyValuePair<List<string>, GameObject[]> combo in combos)
        {
            if (combo.Key.Count != combo.Value.Length)
            {
                print("YOU DONE FUCKED UP!! Size of button collection and combo collection do not match");
                return;
            }

            for (int i = 0; i < combo.Value.Length; i++)
            {
                switch (combo.Key[i])
                {
                    case "Button A":
                        combo.Value[i].GetComponent<RawImage>().texture = a;
                        break;
                    case "Button B":
                        combo.Value[i].GetComponent<RawImage>().texture = b;
                        break;
                    case "Button X":
                        combo.Value[i].GetComponent<RawImage>().texture = x;
                        break;
                    case "Button Y":
                        combo.Value[i].GetComponent<RawImage>().texture = y;
                        break;
                    default:
                        print("Check the values of your ability combo, unrecognized type found");
                        break;
                }
            }
        }
    }

    public void DisplayPanel(Animator anim, params GameObject[] panels)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].gameObject.SetActive(true);
        }
        anim.Play("panelSlideIn");
    }

    public void HidePanel(Animator anim, params GameObject[] panels)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].gameObject.SetActive(false);
        }
        anim.Play("panelSlideOut");
    }

    public string SelectAmmo(List<string> currentCombo, Dictionary<string, List<string>> collection)
    {
        foreach (KeyValuePair<string, List<string>> combo in collection)
        {
            if (combo.Value.SequenceEqual(currentCombo))
            {
                return combo.Key;
            }
        }

        return "Failed to Select";
    }

    public void DisplayCombo(List<string> currentCombo, Dictionary<List<string>, GameObject[]> collection)
    {
        foreach (KeyValuePair<List<string>, GameObject[]> buttonCombo in collection)
        {
            for (int i = 0; i < currentCombo.Count; i++)
            {
                if (currentCombo[i] == buttonCombo.Key[i])
                {
                    Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    buttonCombo.Value[i].GetComponent<RawImage>().color = color;
                }
                else
                {
                    buttonCombo.Value[i].transform.parent.gameObject.SetActive(false); // need to clean this
                }
            }
        }
    }

    public void ResetCombo(Dictionary<List<string>, GameObject[]> collection)
    {
        Color color = new Color(1.0f, 1.0f, 1.0f, 0.39f);
        foreach (KeyValuePair<List<string>, GameObject[]> buttonCombo in collection)
        {
            buttonCombo.Value[0].transform.parent.gameObject.SetActive(true); // only need to do this once but should clean up

            for (int i = 0; i < buttonCombo.Value.Length; i++)
            {
                buttonCombo.Value[i].GetComponent<RawImage>().color = color;
            }
        }
    }
}
