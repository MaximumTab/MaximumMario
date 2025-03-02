using UnityEngine;
using System.Collections;

public class Flagpole : MonoBehaviour
{
    [SerializeField] private float slideSpeed = 3f;            // Speed of sliding down the pole
    [SerializeField] private Transform slideBottomPosition;    // Bottom position of the flagpole
    [SerializeField] private float hopForceY = 5f;             // Vertical hop force
    [SerializeField] private Transform hidePoint;              // Point where the player stops and hides
    [SerializeField] private Transform flagSpriteTransform;    // Flag sprite that moves down
    [SerializeField] private Transform flagTopPosition;        // Top position of the flagpole

    private bool flagpoleSequenceActive = false;
    private Rigidbody2D playerRb;
    private Transform playerTransform;
    private Collider2D flagpoleCollider;
    private ScoreManager scoreManager;

    private void Awake()
    {
        flagpoleCollider = GetComponent<Collider2D>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flagpoleSequenceActive && (collision.CompareTag("BigMario") || collision.CompareTag("SmallMario")))
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

            AwardFlagpolePoints(playerTransform.position.y);
            StartCoroutine(FlagpoleSequence());
        }
    }

    private void AwardFlagpolePoints(float playerY)
    {
        float flagHeight = flagTopPosition.position.y - slideBottomPosition.position.y;
        float relativePosition = (playerY - slideBottomPosition.position.y) / flagHeight;

        int points = 100;
        if (relativePosition >= 0.8f) points = 5000;
        else if (relativePosition >= 0.6f) points = 2000;
        else if (relativePosition >= 0.4f) points = 800;
        else if (relativePosition >= 0.2f) points = 400;
        
        scoreManager?.AddScore(points, playerTransform.position + new Vector3(2f, -2f, 0));
    }

    private IEnumerator FlagpoleSequence()
    {
        playerRb.gravityScale = 0f;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1f);
        
        AudioManager.Instance.PlaySFX("Flagpole");

        float currentX = playerTransform.position.x;
        bool playerReachedBottom = false;
        bool flagReachedBottom = false;

        // Start both the player and flag movement at the same time
        StartCoroutine(MoveFlagDown(() => flagReachedBottom = true));
        StartCoroutine(MovePlayerDown(currentX, () => playerReachedBottom = true));

        // Wait until both player and flag have reached the bottom
        yield return new WaitUntil(() => playerReachedBottom && flagReachedBottom);

        yield return new WaitForSeconds(0.5f);

        // Snap player to exact bottom position
        playerTransform.position = new Vector3(slideBottomPosition.position.x, slideBottomPosition.position.y, playerTransform.position.z);

        yield return new WaitForSeconds(0.25f);

        AudioManager.Instance.PlayConditionSound("LevelClear");

        // Now hop off
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

    private IEnumerator MoveFlagDown(System.Action flagDoneCallback)
    {
        float flagEndY = slideBottomPosition.position.y;

        while (flagSpriteTransform.position.y > flagEndY)
        {
            flagSpriteTransform.position = new Vector3(
                flagSpriteTransform.position.x,
                Mathf.MoveTowards(flagSpriteTransform.position.y, flagEndY, slideSpeed * Time.deltaTime),
                flagSpriteTransform.position.z
            );
            yield return null;
        }

        flagDoneCallback.Invoke();
    }

    private IEnumerator MovePlayerDown(float currentX, System.Action playerDoneCallback)
    {
        while (playerTransform.position.y > slideBottomPosition.position.y)
        {
            playerTransform.position = new Vector3(
                currentX,
                Mathf.MoveTowards(playerTransform.position.y, slideBottomPosition.position.y, slideSpeed * Time.deltaTime),
                playerTransform.position.z
            );
            yield return null;
        }

        playerDoneCallback.Invoke();
    }

    private IEnumerator PushPlayerRight()
    {
        while (true)
        {
            playerRb.AddForce(Vector2.right * 15f, ForceMode2D.Force);
            yield return null;
        }
    }
}
