using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // Reference to the Game Over UI
    public PlayerHealth playerHealth; // Reference to PlayerHealth script

    void Start()
    {
        gameOverUI.SetActive(false); // Ensure Game Over UI is inactive at start
    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        if (playerHealth.playerMovement != null)
        {
            playerHealth.playerMovement.enabled = false;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue()
    {
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
        playerHealth.currentHealth = playerHealth.maxHealth;
        playerHealth.UpdateHealthBar();
        if (playerHealth.playerMovement != null)
        {
            playerHealth.playerMovement.enabled = true;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
