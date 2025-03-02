using UnityEngine;
using System.Collections;

public class Flagpole : MonoBehaviour
{
    [SerializeField] private float slideSpeed = 3f;            // Speed of sliding down the pole
    [SerializeField] private Transform slideBottomPosition;      // Bottom position of the flagpole
    [SerializeField] private float hopForceX = 3f;                 // Horizontal hop force
    [SerializeField] private float hopForceY = 5f;                 // Vertical hop force
    [SerializeField] private Transform hidePoint;                // Point where the player stops and hides

    private bool flagpoleSequenceActive = false;
    private Rigidbody2D playerRb;
    private Transform playerTransform;
    private Collider2D flagpoleCollider;

    private void Awake()
    {
        flagpoleCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flagpoleSequenceActive && collision.CompareTag("Player"))
        {
            flagpoleSequenceActive = true;
            
            AudioManager.Instance.musicSource.Stop();
            
            TimeManager timeManager = FindAnyObjectByType<TimeManager>();
            timeManager?.StopTimer();

            // Disable any player control script if one exists.
            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            
            playerRb = collision.GetComponent<Rigidbody2D>();
            playerTransform = collision.transform;
            
            if (playerRb != null)
            {
                // Reset velocity and ensure only rotation is frozen,
                // so we can slide the player down the pole.
                playerRb.linearVelocity = Vector2.zero;
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            StartCoroutine(FlagpoleSequence());
        }
    }

    private IEnumerator FlagpoleSequence()
    {
        AudioManager.Instance.PlaySFX("Flagpole");

        // Slide the player down until they reach the bottom of the flagpole.
        while (playerTransform.position.y > slideBottomPosition.position.y)
        {
            Vector3 newPos = playerTransform.position;
            newPos.y = Mathf.MoveTowards(playerTransform.position.y, slideBottomPosition.position.y, slideSpeed * Time.deltaTime);
            playerTransform.position = newPos;
            yield return null;
        }
        
        // Snap the player exactly to the bottom position.
        playerTransform.position = new Vector3(playerTransform.position.x, slideBottomPosition.position.y, playerTransform.position.z);
        
        // Short pause to simulate finishing the slide.
        yield return new WaitForSeconds(1f);
        
        AudioManager.Instance.PlayConditionSound("LevelClear");

        // Allow horizontal movement by making sure only rotation is frozen.
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Reset velocity and apply the hop force as an impulse.
        playerRb.linearVelocity = Vector2.zero;
        playerRb.AddForce(new Vector2(hopForceX, hopForceY), ForceMode2D.Impulse);
        
        // Optional pause after hop.
        yield return new WaitForSeconds(0.2f);
        
        // Wait until the player's x position reaches or exceeds the hide point's x coordinate.
        while (playerTransform.position.x < hidePoint.position.x)
        {
            yield return null;
        }
        
        // Snap the player to the hide point.
        playerTransform.position = hidePoint.position;
        
        // Stop the player completely.
        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;  // Disable physics simulation.
        
        // Hide the player.
        playerTransform.gameObject.SetActive(false);

        TimeManager timeManager = FindAnyObjectByType<TimeManager>();
        timeManager?.ConvertTimeToScore();
    }
}
