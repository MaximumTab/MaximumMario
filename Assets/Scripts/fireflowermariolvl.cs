using UnityEngine;

public class FireFlowerMarioLevel : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If we collide with the player (small or big Mario),
        // we upgrade them to Fire level.
        if (collision.gameObject.CompareTag("SmallMario") 
            || collision.gameObject.CompareTag("BigMario"))
        {
            // Grab the PlayerController from the collided object
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                // Set them to Level3_Fire
                pc.UpdatePlayerLevel(PlayerController.PlayerLevel.Level3_Fire);
                Debug.Log("Collided with player -> changed to Level3_Fire");
                
                // Destroy the fire flower
                Destroy(gameObject);
            }
        }
    }
}
