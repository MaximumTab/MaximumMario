using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public PointSpawner pointSpawner; // Reference to the PointSpawner

    private int score = 0;
    private int stompCount = 0; // Track consecutive stomps

    // Score sequence for consecutive stomps
    private readonly int[] stompPoints = { 100, 200, 400, 500, 800, 1000, 2000, 4000, 5000, 8000 };

    public void AddScore(int points, Vector3 worldPosition)
    {
        score += points;
        scoreText.text = score.ToString("D6"); // Formats as 000000

        // Spawn floating points effect
        if (pointSpawner != null)
        {
            pointSpawner.SpawnPoints(worldPosition, points);
        }
    }

    public int GetStompScore()
    {
        int index = Mathf.Min(stompCount, stompPoints.Length - 1);
        return stompPoints[index];
    }

    public void IncrementStompCount()
    {
        stompCount++;
    }

    public void ResetStompCount()
    {
        stompCount = 0;
    }
}
