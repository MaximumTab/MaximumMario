using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutoSwitcher : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay(3f)); 
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(2); 
    }
}
