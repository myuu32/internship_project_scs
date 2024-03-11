using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int scoreToWin = 5;

    // TextMeshProのUIコンポーネントへの参照
    public TextMeshProUGUI scoreTextPlayer1;
    public TextMeshProUGUI scoreTextPlayer2;

    // スコア変更時とゲーム勝利時にトリガーされるイベント
    public UnityEvent onScoreChange;
    public UnityEvent onGameWin;

    private void Start()
    {
        UpdateScoreDisplay(); // ゲーム開始時にスコア表示を更新
    }

    private void UpdateScoreDisplay()
    {
        // TextMeshProテキストを更新
        scoreTextPlayer1.text = scorePlayer1.ToString();
        scoreTextPlayer2.text = scorePlayer2.ToString();
        onScoreChange.Invoke();
    }

    public void AddScorePlayer1(int score)
    {
        scorePlayer1 += score;
        UpdateScoreDisplay();
        CheckWinCondition();
    }

    public void AddScorePlayer2(int score)
    {
        scorePlayer2 += score;
        UpdateScoreDisplay(); 
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (scorePlayer1 >= scoreToWin || scorePlayer2 >= scoreToWin)
        {
            onGameWin.Invoke(); 
            ShowResultMenu();
        }
    }

    public void ShowResultMenu()
    {
        Debug.Log("Show Result Menu");
    }
}
