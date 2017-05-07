using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private int score = 0;
    public GameObject scoreText;
	
	// Update is called once per frame
	void Update ()
    {
        scoreText.GetComponent<Text>().text = "Score: " + score;
	}

    public void SetScore(int points)
    {
        score += points;
    }
}
