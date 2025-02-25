using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int score = 0;

    public void AddScore(int points)
    {
        score += points;
        scoreText.text =score.ToString("D6"); // Formats as 000000
    }
}
