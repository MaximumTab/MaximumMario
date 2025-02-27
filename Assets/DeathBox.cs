using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathBox : MonoBehaviour
{
    public int Respawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            AudioManager.Instance.PlayConditionSound("Mario Dies");
            SceneManager.LoadScene(Respawn);
        }
    }
} 

