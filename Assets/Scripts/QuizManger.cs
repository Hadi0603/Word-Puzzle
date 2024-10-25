using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizManger : MonoBehaviour
{
    public static QuizManger instance;
    [SerializeField]
    private QuestionData question;
    [SerializeField]
    private Image questionImage;
    [SerializeField]
    private WordData[] answerWordArray;
    [SerializeField]
    private WordData[] optionWordArray;
    private char[] charArray = new char[12];
    private int currentAnswerIndex = 0;
    private bool correctAnswer = true;
    private void Awake()
    {
        if (instance == null) instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        SetQuestion();
    }
    private void SetQuestion()
    {
        currentAnswerIndex = 0;
        ResetQuestion();
        questionImage.sprite = question.questionImage;
        for(int i = 0; i < question.answer.Length; i++)
        {
            charArray[i] = char.ToUpper(question.answer[i]);
        }
        for(int i = question.answer.Length; i < optionWordArray.Length; i++)
        {
            charArray[i] = (char)UnityEngine.Random.Range(65, 91);
        }
        charArray = ShuffleList.ShuffleListItems<char>(charArray.ToList()).ToArray();
        for(int i = 0; i < optionWordArray.Length; i++)
        {
            optionWordArray[i].SetChar(charArray[i]);
        }
    }
    public void SelectedOption(WordData wordData)
    {
        if (currentAnswerIndex >= answerWordArray.Length) return;
        answerWordArray[currentAnswerIndex].SetChar(wordData.charValue);
        wordData.gameObject.SetActive(false);
        currentAnswerIndex++;
        if(currentAnswerIndex >= question.answer.Length)
        {

        }
    }
    private void ResetQuestion()
    {
        for(int i = 0; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(true);
            answerWordArray[i].SetChar('_');
        }
        for(int i = question.answer.Length; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(false);
        }  
    }
}
[System.Serializable]
public class QuestionData
{
    public Sprite questionImage;
    public string answer;
}
