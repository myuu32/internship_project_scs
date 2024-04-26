using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused = false;

    void Start()
    {
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
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenu != null)
        {
            new WaitForSeconds(0.5f);
            pauseMenu.SetActive(false);
        }
    }

    public void Disconnect()
    {
        Time.timeScale = 0f;
    }

    public void Reconnect()
    {
        Time.timeScale = 1f;
    }
}
