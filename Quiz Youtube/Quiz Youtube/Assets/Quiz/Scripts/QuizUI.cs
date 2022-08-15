using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizUI : MonoBehaviour
{
    [SerializeField] private QuizManager quizManager;
    [SerializeField] private Text questionText, scoreText, timerText, s1, s2, s3;
    [SerializeField] private List<Image> lifeImageList;
    [SerializeField] private AudioSource questionAudio;
    [SerializeField] private GameObject gameOverPanel, mainMenuPanel, gameMenuPanel, allRight, allWrong, countdown;
    [SerializeField] private Image questionImage;
    [SerializeField] private List<Button> options, uiButtons;
    [SerializeField] private Color correctCol, wrongCol, normalCol;

    

    private Question question; //store select question
    private bool answered; //keep tack of answer/options
    private float audioLength;

    public Text ScoreText {get{return scoreText;}}
    public Text TimerText {get{return timerText;}}
    public Text St1 {get{return s1;}}
    public Text St2 {get{return s2;}}
    public Text St3 {get{return s3;}}
    
    public GameObject GameOverPanel {get{return gameOverPanel;}}
    public GameObject AllRight {get{return allRight;}}
    public GameObject AllWrong {get{return allWrong;}}
    public GameObject CountDown {get{return countdown;}}
    public GameObject GameMenu {get{return gameMenuPanel;}}
    
    void Awake()
    {
         for (int i=0; i<options.Count; i++)
        {
            Button localBtn = options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

        for (int i=0; i<uiButtons.Count; i++)
        {
            Button localBtn = uiButtons[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

      
    }

    public void SetQuestion(Question question)
    {
        this.question = question;

        switch (question.questionType)
        {
            case QuestionType.TEXT: //to show question text
            questionImage.transform.parent.gameObject.SetActive(false);
            break; 

            case QuestionType.IMAGE: //to show question image
            ImageHolder();
            questionImage.transform.gameObject.SetActive(true);
            questionImage.sprite = question.qustionImg;
            break;
 
            case QuestionType.AUDIO:
            ImageHolder();
            questionAudio.transform.gameObject.SetActive(true);
            audioLength = question.qustionClip.length;
            StartCoroutine(PlayAudio());
            break;
        }

        questionText.text = question.questionInfo;
        List<string> answerList = ShuffleList.ShuffleListItems<string>(question.options);

        for (int i=0; i<options.Count; i++) //options button
        {
            options[i].GetComponentInChildren<Text>().text = answerList[i];
            options[i].name = answerList[i]; //send name button
            options[i].image.color = normalCol;
        }
   
        answered = false;
    }

    IEnumerator PlayAudio()
    {
        if(question.questionType == QuestionType.AUDIO)
        {
            questionAudio.PlayOneShot(question.qustionClip);
            yield return new WaitForSeconds(audioLength + 0.5f);
            StartCoroutine(PlayAudio());
        }

        else
        {
            StopCoroutine(PlayAudio());
            yield return null;
        }
    }

     void ImageHolder()
    {
        questionImage.transform.parent.gameObject.SetActive(true); //activate the parent
        questionImage.transform.gameObject.SetActive(false);
        questionAudio.transform.gameObject.SetActive(false);
    }

    private void OnClick(Button btn) //buttton
    {
        if(quizManager.GameStatus == GameStatus.Playing)
        {
            
            if (!answered)
            {
                 answered = true;
                 bool val = quizManager.Answer(btn.name);

                 if(val) //value true
                 {
                    FindObjectOfType<AudioManager>().Play("Right Answer");
                    btn.image.color = correctCol;
                 }

                 else
                 {
                    FindObjectOfType<AudioManager>().Play("Wrong Answer");
                    btn.image.color = wrongCol;

                 }
            }
        } 

        switch (btn.name)
        {
            case "Play":
             
             quizManager.StartGame(0);
             mainMenuPanel.SetActive(false);
             CountDown.SetActive(true);
             FindObjectOfType<AudioManager>().Play("Count");
             break;

            case "Settings":
             
             quizManager.StartGame(1);
             mainMenuPanel.SetActive(false);
             CountDown.SetActive(true);
             FindObjectOfType<AudioManager>().Play("Count");
             break;            
        }
    }

    public void RetryButton()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReduceLife(int index)
    {
        lifeImageList[index].color = wrongCol;
    }
}
