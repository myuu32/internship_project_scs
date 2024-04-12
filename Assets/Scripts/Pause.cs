using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; // ポーズメニューオブジェクトへの参照

    private bool isPaused = false;

    void Start()
    {
        // ゲームの開始時にポーズメニューを無効にする
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Pause menu object not assigned.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ポーズと再開を切り替える
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // ゲームを一時停止するために時間の尺度を0に設定
        isPaused = true;
        Debug.Log("Game paused.");

        // ポーズメニューを有効にする
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // ゲームを再開するために時間の尺度を1に設定
        isPaused = false;
        Debug.Log("Game resumed.");

        // ポーズメニューを無効にする
        if (pauseMenu != null)
        {
            new WaitForSeconds(0.5f);
            pauseMenu.SetActive(false);
        }
    }

    public void Disconnect()
    {
        Time.timeScale = 0f;
        Debug.Log("Game paused.");
    }

    public void Reconnect()
    {
        Time.timeScale = 1f;
        Debug.Log("Game resumed.");
    }
}
