using UnityEngine;

public class StarPowerSuperStars : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If we collide with the player on any prior level,
        // we upgrade them to Star (Level4_Star).
        if (collision.gameObject.CompareTag("SmallMario") 
            || collision.gameObject.CompareTag("BigMario")
            || collision.gameObject.CompareTag("FireMario"))
        {
            // Grab the PlayerController from the collided object
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                // Set them to Level4_Star
                pc.UpdatePlayerLevel(PlayerController.PlayerLevel.Level4_Star);
                Debug.Log("Collided with player -> changed to Level4_Star");
                
                // Destroy the star
                Destroy(gameObject);
            }
        }
    }
}
