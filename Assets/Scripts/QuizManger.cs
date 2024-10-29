using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private QuizDataScriptable questionData;
    [SerializeField] private WordData[] answerWordArray;
    [SerializeField] private WordData[] optionWordArray;
    [SerializeField]
    private char[] charArray = new char[6];
    private int currentAnswerIndex = 0;
    private bool correctAnswer = true;
    private List<int> selectedWordIndex;
    private int currentQuestionIndex = 0;
    private GameStatus gameStatus = GameStatus.Playing;
    private string answerWord;
    private string answerWord1;
    private int number;
    public static int score = 0;
    private int wordsRemaining = 2;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

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
        answerWord = questionData.questions[currentQuestionIndex].answer;
        ResetQuestion();

        // Populate charArray with the answer characters and random letters
        for (int i = 0; i < answerWord.Length; i++)
        {
            charArray[i] = char.ToUpper(answerWord[i]);
        }
        for (int i = answerWord.Length; i < optionWordArray.Length; i++)
        {
            charArray[i] = (char)UnityEngine.Random.Range(65, 91);
        }
        charArray = ShuffleList.ShuffleListItems<char>(charArray.ToList()).ToArray();

        // Set each optionWordArray element to a character in charArray
        for (int i = 0; i < optionWordArray.Length; i++)
        {
            optionWordArray[i].SetChar(charArray[i]);
        }

        
        currentQuestionIndex++;
        if (wordsRemaining > 1)
        {
            answerWord1 = questionData.questions[currentQuestionIndex].answer;
        }
        gameStatus = GameStatus.Playing;
    }

    public void SelectedOption(WordData wordData)
{
    if (gameStatus == GameStatus.Next || currentAnswerIndex >= answerWord.Length) return;

    selectedWordIndex.Add(wordData.transform.GetSiblingIndex());
    answerWordArray[currentAnswerIndex].SetChar(wordData.charValue);

    currentAnswerIndex++;

    if (currentAnswerIndex >= answerWord.Length)
    {
        correctAnswer = true;
        for (int i = 0; i < answerWord.Length; i++)
        {
            if (char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue))
            {
                // Attempt to match with answerWord1 if answerWord is incorrect
                if (char.ToUpper(answerWord1[i]) != char.ToUpper(answerWordArray[i].charValue))
                {
                    correctAnswer = false;
                    break;
                }
            }
        }

        if (correctAnswer)
        {
            if (answerWord.Equals(answerWord1))
            {
                questionData.questions.RemoveAt(currentQuestionIndex);
            }
            else
            {
                questionData.questions.RemoveAt(currentQuestionIndex-1);
            }

            wordsRemaining--;
            score += 50;
            Debug.Log("Correct answer! Score: " + score);
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();

            gameStatus = GameStatus.Next;

            if (wordsRemaining > 0)
            {
                // Update answerWord and answerWord1 with the next available question answers
                answerWord = questionData.questions[0].answer;
                if (questionData.questions.Count > 1)
                {
                    answerWord1 = questionData.questions[1].answer;
                }
                Invoke("SetQuestion", 1f);
            }
            else
            {
                gameOver.SetActive(true);
            }
        }
        else
        {
            WrongAnswer();
        }
    }
}


    public void ResetQuestion()
    {
        FindObjectOfType<WordSelectionManager>().ResetSelections();
        for (int i = 0; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(true);
            answerWordArray[i].SetChar('_');
        }
        for (int i = answerWord.Length; i < answerWordArray.Length; i++)
        {
            answerWordArray[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < optionWordArray.Length; i++)
        {
            optionWordArray[i].gameObject.SetActive(true);
        }

        for (int i = answerWord.Length; i < optionWordArray.Length; i++)
        {
            optionWordArray[i].gameObject.SetActive(false);
        }
    }
    public void WrongAnswer()
    {
        StartCoroutine(WrongAnswerCoroutine());
    }
    private IEnumerator WrongAnswerCoroutine()
    {
        
        Debug.Log("Incorrect answer.");
        yield return new WaitForSeconds(0.3f);
        FindObjectOfType<WordSelectionManager>().ResetSelections();
        for(int i = 0;i < answerWordArray.Length; i++)
        {
            ResetLastWord();
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
            FindObjectOfType<WordSelectionManager>().ResetSelections();
        }
    }
}

[System.Serializable]
public class QuestionData
{
    public string answer;
}

public enum GameStatus
{
    Playing,
    Next
}
