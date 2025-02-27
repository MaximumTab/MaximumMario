using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathBox : MonoBehaviour
{
    public int Respawn;
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            AudioManager.Instance.PlayConditionSound("Mario Dies");
            SceneManager.LoadScene(Respawn);
        }
    }
} 

