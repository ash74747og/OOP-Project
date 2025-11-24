using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    // Function to start the game (Loads Scene 1)
    public void StartGame()
    {
        Debug.Log("Starting Game... Loading Scene 1.");
        SceneManager.LoadScene(1);
    }

    // Function to exit the game
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        
        #if UNITY_EDITOR
            // If running in the Editor, stop playing
            EditorApplication.isPlaying = false;
        #else
            // If running in a Build, quit the application
            Application.Quit();
        #endif
    }
}
