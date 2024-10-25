using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    [SerializeField] Text scoreText;
    public void Awake()
    {
        scoreText.text = "Score: " + QuizManger.score;
    }
    public void PlayBtn()
    {
        SceneManager.LoadScene("GamePlay");
    }
    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Restart()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
