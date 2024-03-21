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
    public GameObject gameStatusUI;

    private bool gameStatus = true;

    public UnityEvent onScoreChange;
    public UnityEvent onGameWin;

    private void Start()
    {
        UpdateScoreDisplay();
        UpdateGameStatusUI();
    }

    private void UpdateScoreDisplay()
    {
        scoreTextPlayer1.text = scorePlayer1.ToString();
        scoreTextPlayer2.text = scorePlayer2.ToString();
        onScoreChange.Invoke();
    }

    private void UpdateGameStatusUI()
    {
        if (gameStatus)
        {
            gameStatusUI.SetActive(true);
        }
        else
        {
            gameStatusUI.SetActive(false);
        }
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
        Debug.Log("Show Result Menu");
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
}
