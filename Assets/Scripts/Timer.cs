using UnityEngine;
using TMPro;     

public class Timer : MonoBehaviour
{

    public TextMeshProUGUI timeText;

    public float timeRemaining = 400f;
    private bool timerIsRunning = true;

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
                Debug.Log("GAME OVER");
                timeRemaining = 0;
                timerIsRunning = false;
                
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int seconds = Mathf.CeilToInt(timeToDisplay);
        timeText.text = seconds.ToString();
    }
}
