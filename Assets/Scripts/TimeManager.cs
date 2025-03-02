using UnityEngine;
using TMPro;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public float timeRemaining = 400f;
    private bool timerIsRunning = true;
    private bool isConvertingToScore = false;

    public void StopTimer()
    {
        if (timerIsRunning)
        {
            timerIsRunning = false;
            Debug.Log("Timer stopped at: " + Mathf.CeilToInt(timeRemaining));
        }
    }

    void Update()
    {
        if (timerIsRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int seconds = Mathf.CeilToInt(timeToDisplay);
        timeText.text = seconds.ToString();
    }

    public void ConvertTimeToScore()
    {
        if (!isConvertingToScore)
        {
            StartCoroutine(ConvertTimeCoroutine());
        }
    }

    IEnumerator ConvertTimeCoroutine()
    {
        isConvertingToScore = true;
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();

        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found in the scene!");
            yield break;
        }

        while (timeRemaining > 0)
        {
            scoreManager.AddScore(50, Vector3.zero, false); // Prevent visual point spawning
            timeRemaining--;
            DisplayTime(timeRemaining);
            yield return new WaitForSeconds(0.02f);
        }

        isConvertingToScore = false;
        Debug.Log("Time converted to score.");

        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}
