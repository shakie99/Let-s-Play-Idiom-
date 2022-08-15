using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private QuizUI quizUI;
    [SerializeField] private List<QuizDataScriptable> quizData;
    [SerializeField] private float timeLimit = 30f;
    [SerializeField] Text countdownText;


    private List<Question> questions;
    private Question selectedQuestion;
    private int scoreCount = 0;
    private float currentTime;
    private int lifeRemaining = 3;
    
    public float cTime;
    public float startingTime = 5f;
   
    private GameStatus gameStatus = GameStatus.Next;
    public GameStatus GameStatus {get{return gameStatus;}}

    
    public List<QuestionAndAnswers> QnA;

    public Text ScoreTxt;
    public Text st1;
    public Text st2;
    public Text st3;
    public Text CountdownText;

    int totalQuestions = 0;
    public int score;

    
    public void StartGame(int index)
    {
        
        totalQuestions = QnA.Count;
        
        scoreCount = 0;
        currentTime = timeLimit;
        lifeRemaining = 3;
        questions = new List<Question>();
        cTime = startingTime;
     
        for (int i=0; i<quizData[index].questions.Count; i++)
        {
            questions.Add(quizData[index].questions[i]);
        }

        

        SelectQuestion();
        gameStatus = GameStatus.Playing;
        FindObjectOfType<AudioManager>().Play("Part 2");
    }


    void SelectQuestion() // select question randomly
    {
        int val = UnityEngine.Random.Range (0,questions.Count);
        selectedQuestion = questions[val];
        quizUI.SetQuestion(selectedQuestion);

        questions.RemoveAt(val);
    }

    private void Update()
    {
        cTime -= 1*Time.deltaTime;
        countdownText.text = cTime.ToString("0");

        if(cTime <= 0)
        {
            cTime = 0;
            quizUI.CountDown.SetActive(false);
            quizUI.GameMenu.SetActive(true);
            
            
        }

        if (gameStatus == GameStatus.Playing)
        {
            currentTime -= Time.deltaTime;
            SetTimer(currentTime);
            
        }
    }

    private void SetTimer(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(value);
        quizUI.TimerText.text = "Time: " + time.ToString("mm':'ss");

        if (currentTime <= 0)
        {
           if(score<10) 
           {
             gameStatus = GameStatus.Next;
             quizUI.GameOverPanel.SetActive(true);
             quizUI.AllRight.SetActive(false);
             quizUI.AllWrong.SetActive(false);
             quizUI.St1.text = score + "/10";
           } 

            if(score==10)
            { 
             gameStatus = GameStatus.Next;
             quizUI.AllRight.SetActive(true);
             quizUI.GameOverPanel.SetActive(false);
             quizUI.AllWrong.SetActive(false);
             quizUI.St2.text = score + "/10";
            } 

            if(score<=3)
            {
             gameStatus = GameStatus.Next;
             quizUI.AllWrong.SetActive(true);
             quizUI.AllRight.SetActive(false);
             quizUI.GameOverPanel.SetActive(false);
             quizUI.St3.text = score + "/10"; 
            }
        }
    }


    public bool Answer(string answered) //wther the ans is right or wrong
    {
       bool correctAns = false;

       if (answered == selectedQuestion.correctAns)
       {
           //yes
           correctAns = true;
           scoreCount += 1;
           score += 1;
           quizUI.ScoreText.text = "Score: " + scoreCount;
       }

       else
       {
           //no
           lifeRemaining--;
           quizUI.ReduceLife(lifeRemaining);

           if (lifeRemaining <= 0)
           {
             
            if(score<10) 
            {
             gameStatus = GameStatus.Next;
             quizUI.GameOverPanel.SetActive(true);
             quizUI.AllRight.SetActive(false);
             quizUI.AllWrong.SetActive(false);
             
             quizUI.St1.text = score + "/10";
            }

            if(score==10)
            {
             gameStatus = GameStatus.Next;
             quizUI.AllRight.SetActive(true);
             quizUI.GameOverPanel.SetActive(false);
             quizUI.AllWrong.SetActive(false);
             quizUI.St2.text = score + "/10";
            }

            if(score<=3)
            {
             gameStatus = GameStatus.Next;
             quizUI.AllWrong.SetActive(true);
             quizUI.AllRight.SetActive(false);
             quizUI.GameOverPanel.SetActive(false);
             quizUI.St3.text = score + "/10";
            }  
        
           }
       }

       if (gameStatus == GameStatus.Playing)
       {
        
        if (questions.Count > 0)
        {
          Invoke("SelectQuestion",0.4f); //callSQ
        }

        else
        {
            
             if(score<10) 
             {
               gameStatus = GameStatus.Next;
               quizUI.GameOverPanel.SetActive(true);
               quizUI.AllRight.SetActive(false);
               quizUI.AllWrong.SetActive(false);
               quizUI.St1.text = score + "/10";
             }

            if(score==10)
            {
              gameStatus = GameStatus.Next;
              quizUI.AllRight.SetActive(true);
              quizUI.GameOverPanel.SetActive(false);
              quizUI.AllWrong.SetActive(false);
              quizUI.St2.text = score + "/10";
            }

            if(score<=3)
            {
             gameStatus = GameStatus.Next;
             quizUI.AllWrong.SetActive(true);
             quizUI.AllRight.SetActive(false);
             quizUI.GameOverPanel.SetActive(false);
             quizUI.St3.text = score + "/10";
            } 
            

        }
       } 

       return correctAns;
    }
}



    [System.Serializable]
    public class Question
    {
        public string questionInfo;
        public QuestionType questionType; //to identify type of question
        public Sprite qustionImg;
        public AudioClip qustionClip;
        public List<string> options;
        public string correctAns;
    }
    
    [System.Serializable]
    public enum QuestionType
    {
        TEXT,
        IMAGE,
        AUDIO
    }

    [System.Serializable]
    public enum GameStatus
    {
        Next,
        Playing
    }
