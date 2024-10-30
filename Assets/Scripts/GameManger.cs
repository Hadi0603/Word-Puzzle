using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    [SerializeField] Text scoreText;
    public static int levelToLoad;
    private void Awake()
    {
        int savedScore = PlayerPrefs.GetInt("Score", 0);
        QuizManager.score = savedScore;
        scoreText.text = "Score: " + QuizManager.score;
    }

    public void Start()
    {
        levelToLoad = PlayerPrefs.GetInt("levelToLoad", 1);
    }

    public void PlayBtn()
    {
        SceneManager.LoadScene(levelToLoad);
    }
    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Restart()
    {
        SceneManager.LoadScene(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex);
    }
}
