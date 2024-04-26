using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int scoreToWin = 5;

    public TextMeshProUGUI scoreTextPlayer1;
    public TextMeshProUGUI scoreTextPlayer2;
    public TextMeshProUGUI scoreTextPlayer1Result;
    public TextMeshProUGUI scoreTextPlayer2Result;
    public GameObject gameStatusUI;
    public GameObject winnerImage1, winnerImage2, buttonAgain, buttonExit;
    public Animator playerimg1, playerimg2, gameoverimg;

    private bool gameStatus = true;

    public UnityEvent onScoreChange;
    public UnityEvent onGameWin;

    private void Start()
    {
        UpdateScoreDisplay();
        UpdateGameStatusUI();
        Time.timeScale = 1f;

    }

    private void UpdateScoreDisplay()
    {
        scoreTextPlayer1.text = scorePlayer1.ToString();
        scoreTextPlayer2.text = scorePlayer2.ToString();
        onScoreChange.Invoke();
    }

    public void UpdateGameStatusUI()
    {
        bool botExists = GameObject.FindGameObjectWithTag("Bot") != null;
        gameStatusUI.SetActive(!botExists);
    }

    public void AddScorePlayer1(int score)
    {
        if (gameStatus)
        {
            scorePlayer1 += score;
            UpdateScoreDisplay();
            CheckWinCondition();
        }
    }

    public void AddScorePlayer2(int score)
    {
        if (gameStatus)
        {
            scorePlayer2 += score;
            UpdateScoreDisplay();
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (scorePlayer1 >= scoreToWin || scorePlayer2 >= scoreToWin)
        {
            gameStatus = false;
            onGameWin.Invoke();
            ShowResultMenu();
        }
    }

    public void ShowResultMenu()
    {
        scoreTextPlayer1Result.text = scorePlayer1.ToString();
        scoreTextPlayer2Result.text = scorePlayer2.ToString();
        gameoverimg.SetBool("shake", true);

        if (scorePlayer1 > scorePlayer2)
        {
            playerimg1.SetBool("win", true);
            playerimg2.SetBool("lose", true);
        }
        else if (scorePlayer1 < scorePlayer2)
        {
            playerimg1.SetBool("lose", true);
            playerimg2.SetBool("win", true);
        }
    }

    public void HandleGameStatus(int playerCount, int botCount)
    {
        if (playerCount == 2 || (playerCount == 1 && botCount == 1))
        {
            gameStatus = true;
        }
        else
        {
            gameStatus = false;
        }
        UpdateGameStatusUI();
    }

    public void GameTimeStop()
    {
        Time.timeScale = 0f;
    }

    public void WinnerImage ()
    {
        if (scorePlayer1 == scorePlayer2)
        {
            winnerImage1.SetActive(false);
            winnerImage2.SetActive(false);
            buttonAgain.SetActive(true);
            buttonExit.SetActive(true);
        }
        else if (scorePlayer1 > scorePlayer2)
        {
            winnerImage1.SetActive(true);
            buttonAgain.SetActive(true);
            buttonExit.SetActive(true);
        }

        else
        {
            winnerImage2.SetActive(true);
            buttonAgain.SetActive(true);
            buttonExit.SetActive(true);
        }
    }
}
