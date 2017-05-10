using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private int score = 0;
    public GameObject scoreText;
    public GameObject mainUi;

    public GameObject gameOver;
    public Text finalScore;

    private GameObject player;

    void Start ()
    {
        mainUi.SetActive(true);
        gameOver.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
    }

	// Update is called once per frame
	void Update ()
    {
        if (!player) EndGame();

        scoreText.GetComponent<Text>().text = "Score: " + score;
	}

    public void SetScore(int points)
    {
        score += points;
    }

    public void EndGame()
    {
        gameOver.SetActive(true);
        mainUi.SetActive(false);

        finalScore.text = "Final Score: " + score;
    }
}