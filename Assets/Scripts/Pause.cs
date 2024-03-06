using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu; // Reference to the pause menu object

    private bool isPaused = false;

    void Start()
    {
        // Disable the pause menu at the beginning of the game
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
        // Check if the "Pause" key is pressed (e.g., the "P" key)
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Toggle between pause and resume
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
        Time.timeScale = 0f; // Set the time scale to 0 to pause the game
        isPaused = true;
        Debug.Log("Game paused.");

        // Enable the pause menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // Set the time scale back to 1 to resume the game
        isPaused = false;
        Debug.Log("Game resumed.");

        // Disable the pause menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }
}
