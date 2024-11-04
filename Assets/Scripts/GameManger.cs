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
        Time.timeScale = 1f;
        FindObjectOfType<WordSelectionManager>().enabled = true;
        SceneManager.LoadScene("Menu");
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        FindObjectOfType<WordSelectionManager>().enabled = false;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        FindObjectOfType<WordSelectionManager>().enabled = true;
        pauseUI.SetActive(false);
    }
}