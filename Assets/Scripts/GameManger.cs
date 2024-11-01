using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] private GameObject pauseUI;
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

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }
}