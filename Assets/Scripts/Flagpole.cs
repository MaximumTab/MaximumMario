using UnityEngine;
using System.Collections;

public class Flagpole : MonoBehaviour
{
    [SerializeField] private float slideSpeed = 3f;            // Speed of sliding down the pole
    [SerializeField] private Transform slideBottomPosition;      // Bottom position of the flagpole
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
        if (!flagpoleSequenceActive && (collision.CompareTag("Big Mario") || collision.CompareTag("Small Mario")))
        {
            flagpoleSequenceActive = true;
            
            AudioManager.Instance.musicSource.Stop();
            
            TimeManager timeManager = FindAnyObjectByType<TimeManager>();
            timeManager?.StopTimer();

            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            
            playerRb = collision.GetComponent<Rigidbody2D>();
            playerTransform = collision.transform;
            
            if (playerRb != null)
            {

                playerRb.linearVelocity = Vector2.zero;
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            StartCoroutine(FlagpoleSequence());
        }
    }

    private IEnumerator FlagpoleSequence()
    {
        playerRb.gravityScale = 0f;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1f);
        
        AudioManager.Instance.PlaySFX("Flagpole");

        // Slide down the pole while maintaining X position
        float currentX = playerTransform.position.x;
        while (playerTransform.position.y > slideBottomPosition.position.y)
        {
            playerTransform.position = new Vector3(
                currentX,
                Mathf.MoveTowards(playerTransform.position.y, slideBottomPosition.position.y, slideSpeed * Time.deltaTime),
                playerTransform.position.z
            );
            yield return null;
        }

        yield return new WaitForSeconds(.5f);
        
        // Snap to exact bottom position
        playerTransform.position = new Vector3(
            slideBottomPosition.position.x,
            slideBottomPosition.position.y,
            playerTransform.position.z
        );

        yield return new WaitForSeconds(.25f);

        AudioManager.Instance.PlayConditionSound("LevelClear");

        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRb.gravityScale = 1f;
        playerRb.linearVelocity = Vector2.zero; 
        playerRb.AddForce(new Vector2(0, hopForceY), ForceMode2D.Impulse);

        // Start pushing the player right continuously
        StartCoroutine(PushPlayerRight());

        while (Mathf.Abs(playerTransform.position.x - hidePoint.position.x) > 0.1f)
        {
            yield return null;
        }

        // Stop all movement and hide player
        playerRb.linearVelocity = Vector2.zero;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        StopCoroutine(PushPlayerRight());
        playerTransform.gameObject.SetActive(false);

        TimeManager timeManager = FindAnyObjectByType<TimeManager>();
        timeManager?.ConvertTimeToScore();
    }

    private IEnumerator PushPlayerRight()
    {
        while (true)
        {
            playerRb.AddForce(Vector2.right * 5f, ForceMode2D.Force); 
            yield return null;
        }
    }
}
