using UnityEngine;
using TMPro;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    public float timeRemaining = 400f; // Starting time
    private bool timerIsRunning = true;
    private bool isConvertingToScore = false;

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Ran out of Time");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }

        // Simulate flagpole touch with 'P' key for testing
        if (Input.GetKeyDown(KeyCode.P) && !isConvertingToScore)
        {
            StartCoroutine(ConvertTimeToScore());
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int seconds = Mathf.CeilToInt(timeToDisplay);
        timeText.text = seconds.ToString();
    }

    IEnumerator ConvertTimeToScore()
    {
        timerIsRunning = false;
        isConvertingToScore = true;

        while (timeRemaining > 0)
        {
            FindAnyObjectByType<ScoreManager>().AddScore(50);
            timeRemaining--;
            DisplayTime(timeRemaining);
            yield return new WaitForSeconds(0.025f); // Adjust speed of countdown
        }

        isConvertingToScore = false;
    }
}
