using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;
    [FormerlySerializedAs("gameOver")] [SerializeField] public GameObject levelComplete;
    [SerializeField] private GameObject levelLostUI;      // UI to show when time runs out
    [SerializeField] private Text timerText;              // UI text for the countdown timer
    [SerializeField] private int levelTime = 60; 
    [SerializeField] private QuizDataScriptable questionData;
    [SerializeField] private WordData[] answerWordArray;
    [SerializeField] private WordData[] optionWordArray;
    [SerializeField] private Text completedWordText;
    [SerializeField] private Text completedWordText1;
    [SerializeField] private Text wrongWordText;
    [SerializeField]
    private char[] charArray = new char[6];

    [SerializeField] private GameObject pauseBtn;
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
    private bool firstWordCompleted = false;
    private int timeRemaining;
    private bool isLevelLost = false;
    private int levelCompleted = 0;
    private bool isAnswerWordCompleted = false;
    private bool isAnswerWord1Completed = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        selectedWordIndex = new List<int>();
    }

    private void Start()
    {
        timeRemaining = levelTime;
        UpdateTimerUI();
        StartCoroutine(LevelTimer());
        levelComplete.SetActive(false);
        pauseBtn.SetActive(true);
        wrongWordText.gameObject.SetActive(false);
        SetQuestion();
        completedWordText.gameObject.SetActive(false);
        
        FindObjectOfType<WordSelectionManager>().enabled = true;
    }
    private IEnumerator LevelTimer()
    {
        
        while (timeRemaining > 0 && !isLevelLost&&levelCompleted==0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            UpdateTimerUI();

            if (timeRemaining <= 0)
            {
                ShowLevelLost();
            }
        }
    }
    private void UpdateTimerUI()
    {
        timerText.text = $"00:{timeRemaining:D2}";
        timerText.color = timeRemaining <= 10 ? Color.red : Color.black;
    }
    private void ShowLevelLost()
    {
        WrongAnswer();
        pauseBtn.SetActive(false);
        isLevelLost = true;
        levelLostUI.SetActive(true);
        FindObjectOfType<WordSelectionManager>().enabled = false;
    }
    private void SetQuestion()
    {
        currentAnswerIndex = 0;
        selectedWordIndex.Clear();

        if (questionData.questions.Count <= currentQuestionIndex)
        {
            levelComplete.SetActive(true);
            return;
        }

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

        for (int i = 0; i < optionWordArray.Length; i++)
        {
            optionWordArray[i].SetChar(charArray[i]);
        }
        if (questionData.questions.Count > currentQuestionIndex + 1)
        {
            answerWord1 = questionData.questions[currentQuestionIndex + 1].answer;
        }
        else
        {
            answerWord1 = null;
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
        bool isPrimaryWordCompleted = true;

        for (int i = 0; i < answerWord.Length; i++)
        {
            if (char.ToUpper(answerWord[i]) != char.ToUpper(answerWordArray[i].charValue))
            {
                if (answerWord1 != null && char.ToUpper(answerWord1[i]) == char.ToUpper(answerWordArray[i].charValue))
                {
                    isPrimaryWordCompleted = false;
                }
                else
                {
                    correctAnswer = false;
                    break;
                }
            }
        }

        if (correctAnswer)
        {
            if (isPrimaryWordCompleted && !isAnswerWordCompleted)
            {
                isAnswerWordCompleted = true; // Mark first word as completed
                StartCoroutine(ShowCompletedWord(answerWord));
            }
            else if (!isPrimaryWordCompleted && !isAnswerWord1Completed)
            {
                isAnswerWord1Completed = true; // Mark second word as completed
                StartCoroutine(ShowCompletedWord(answerWord1));
            }

            wordsRemaining--;
            score += 50;
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();

            gameStatus = GameStatus.Next;

            // Check if both words are completed
            if (isAnswerWordCompleted && isAnswerWord1Completed)
            {
                levelComplete.SetActive(true);
                levelCompleted = 1;
                FindObjectOfType<WordSelectionManager>().ResetSelections();
                FindObjectOfType<WordSelectionManager>().enabled = false;
                pauseBtn.SetActive(false);
                PlayerPrefs.SetInt("levelToLoad", ++GameManger.levelToLoad);
                PlayerPrefs.Save();
            }
            else
            {
                Invoke("SetQuestion", 0.2f);
            }
        }
        else
        {
            WrongAnswer();
        }
    }
}


    private IEnumerator ShowCompletedWord(string word)
    {
        if (!firstWordCompleted)
        {
            completedWordText.text = word;
            completedWordText.gameObject.SetActive(true);
            firstWordCompleted = true;  // Mark the first word as completed
        }
        else
        {
            completedWordText1.text = word;
            completedWordText1.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);

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
    }

    public void WrongAnswer()
    {
        StartCoroutine(WrongAnswerCoroutine());
    }

    private IEnumerator WrongAnswerCoroutine()
    {
        Debug.Log("Incorrect answer.");
        wrongWordText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        FindObjectOfType<WordSelectionManager>().ResetSelections();
        for(int i = 0;i < answerWordArray.Length; i++)
        {
            ResetLastWord();
        }
        yield return new WaitForSeconds(1f);
        wrongWordText.gameObject.SetActive(false);
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
