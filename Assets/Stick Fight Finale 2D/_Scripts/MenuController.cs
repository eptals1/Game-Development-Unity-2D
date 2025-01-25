using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // This method will load the SinglePlayer scene
    public void PlayGame()
    {
        SceneManager.LoadScene("SinglePlayer");
    }

    // This method will load the Settings scene
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    // This method will load the MainMenu scene
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // This method will quit the game application
    public void QuitGame()
    {
        Application.Quit();
    }
}
