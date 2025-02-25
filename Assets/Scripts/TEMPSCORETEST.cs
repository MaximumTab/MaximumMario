using UnityEngine;

public class TEMPSCORETEST : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindAnyObjectByType<ScoreManager>().AddScore(100);
        }
    }
}
