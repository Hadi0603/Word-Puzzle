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
    private List<int> selectedWordIndex;
    private void Awake()
    {
        if (instance == null) instance = this;
        else
            Destroy(gameObject);

        selectedWordIndex = new List<int>();
    }
    private void Start()
    {
        SetQuestion();
    }
    private void SetQuestion()
    {
        currentAnswerIndex = 0;
        selectedWordIndex.Clear();
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
        if (currentAnswerIndex >= question.answer.Length) return;
        selectedWordIndex.Add(wordData.transform.GetSiblingIndex());
        answerWordArray[currentAnswerIndex].SetChar(wordData.charValue);
        wordData.gameObject.SetActive(false);
        currentAnswerIndex++;
        if(currentAnswerIndex >= question.answer.Length)
        {
            correctAnswer = true;
            for(int i = 0;i < question.answer.Length; i++)
            {
                if(char.ToUpper(question.answer[i]) != char.ToUpper(answerWordArray[i].charValue))
                {
                    correctAnswer = false;
                    break;
                }
            }
            if (correctAnswer)
            {
                Debug.Log("correct answer");
            }
            else if (!correctAnswer)
            {
                Debug.Log("not correct answer");
            }
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
    public void ResetLastWord()
    {
        if (selectedWordIndex.Count > 0)
        {
            int index = selectedWordIndex[selectedWordIndex.Count - 1];
            optionWordArray[index].gameObject.SetActive(true);
            selectedWordIndex.RemoveAt(selectedWordIndex.Count - 1);
            currentAnswerIndex--;
            answerWordArray[currentAnswerIndex].SetChar('_');
        }
    }
}
[System.Serializable]
public class QuestionData
{
    public Sprite questionImage;
    public string answer;
}
