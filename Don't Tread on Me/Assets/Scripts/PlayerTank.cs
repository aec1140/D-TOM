using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class PlayerTank : MonoBehaviour {
    public GameObject HPbar;
    private float currentHP;
    private float totalHP;
    private float percentHP;

    void Start () {
        totalHP = this.gameObject.GetComponent<HP>().MaxHP;
        for (int e = 0; e < 10; e++)
        {
            HPbar.transform.GetChild(e).gameObject.SetActive(true);
        }
    }
	
	void FixedUpdate () {
        currentHP = this.gameObject.GetComponent<HP>().getCurrHP();
        percentHP = currentHP / totalHP;
        CalcHPBar(percentHP);
    }

    //method to calculate what pips to show in HP; probably not the most efficient solution
    //nested ifs would probably be better, but wouldnt look as nice
    public void CalcHPBar(float percentage)
    {
        int hidePipsStart = 11;
        if (percentage < 1 && percentage > 0.9)
        {
            hidePipsStart = 9;
        }
        else if (percentage < 0.9 && percentage > 0.8)
        {
            hidePipsStart = 8;
        }
        else if (percentage < 0.8 && percentage > 0.7)
        {
            hidePipsStart = 7;
        }
        else if (percentage < 0.7 && percentage > 0.6)
        {
            hidePipsStart = 6;
        }
        else if (percentage < 0.6 && percentage > 0.5)
        {
            hidePipsStart = 5;
        }
        else if (percentage < 0.5 && percentage > 0.4)
        {
            hidePipsStart = 4;
        }
        else if (percentage < 0.4 && percentage > 0.3)
        {
            hidePipsStart = 3;
        }
        else if (percentage < 0.3 && percentage > 0.2)
        {
            hidePipsStart = 2;
        }
        else if (percentage < 0.2 && percentage > 0.1)
        {
            hidePipsStart = 1;
        }
        else if (percentage < 0.1 && percentage > 0.0)
        {
            hidePipsStart = 0;
        }
        else if (percentage <= 0.0)
        {
            hidePipsStart = 0;
        }
        //iterates through the pips, starting at the value dictated by the series of ifs, and turns them off
        for (int e = hidePipsStart; e < 10; e++)
        {
            HPbar.transform.GetChild(e).gameObject.SetActive(false);
        }
    }
}
