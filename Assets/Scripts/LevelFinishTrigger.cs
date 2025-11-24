using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinishTrigger : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("If true, automatically loads the next scene in Build Settings.")]
    [SerializeField] private bool autoLoadNextScene = false;

    [Tooltip("Name of the scene to load (Priority over Index). Leave empty to use Index.")]
    [SerializeField] private string sceneNameToCheck = "";

#if UNITY_EDITOR
    [Tooltip("Drag a Scene asset here to auto-fill the Name field above.")]
    [SerializeField] private Object sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneNameToCheck = sceneAsset.name;
        }
    }
#endif

    [Tooltip("Index of the scene to load (Used if Scene Name is empty).")]
    [SerializeField] private int sceneIndexToLoad = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadLevel();
        }
    }

    private void LoadLevel()
    {
        if (autoLoadNextScene)
        {
            // Auto Logic: Next Index or Loop to 0
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                nextSceneIndex = 0; // Loop to Menu
            }
            SceneManager.LoadScene(nextSceneIndex);
        }
        else if (!string.IsNullOrEmpty(sceneNameToCheck))
        {
            // Load by Name
            Debug.Log($"Loading Scene by Name: {sceneNameToCheck}");
            SceneManager.LoadScene(sceneNameToCheck);
        }
        else
        {
            // Load by Index
            Debug.Log($"Loading Scene by Index: {sceneIndexToLoad}");
            SceneManager.LoadScene(sceneIndexToLoad);
        }
    }
}
