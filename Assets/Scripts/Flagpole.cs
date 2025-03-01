using UnityEngine;
using System.Collections;

public class Flagpole : MonoBehaviour
{
    public Transform flagBottomPosition;
    public Transform castlePosition;

    public float slideSpeed = 3f;
    public float walkSpeed = 2f;

    private bool gameFinished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameFinished || !other.CompareTag("Player")) return;

        Debug.Log("Player touched the flagpole!");

        AudioManager.Instance.musicSource.Stop();

        TimeManager timeManager = FindAnyObjectByType<TimeManager>();
        timeManager?.StopTimer();

        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        StartCoroutine(FlagpoleSequence(other.GetComponent<Rigidbody2D>()));
    }

    private IEnumerator FlagpoleSequence(Rigidbody2D playerRb)
    {
        if (playerRb == null) yield break;

        playerRb.linearVelocity = Vector2.zero;
        playerRb.gravityScale = 0;

        Vector3 startPos = new Vector3(transform.position.x - 0.5f, playerRb.transform.position.y);
        Vector3 endPos = new Vector3(startPos.x, flagBottomPosition.position.y);
        playerRb.transform.position = startPos;

        AudioManager.Instance.PlaySFX("Flagpole");

        while (playerRb.transform.position.y > flagBottomPosition.position.y)
        {
            playerRb.transform.position = Vector3.MoveTowards(playerRb.transform.position, endPos, slideSpeed * Time.deltaTime);
            yield return null;
        }

        playerRb.transform.position = endPos;
        AudioManager.Instance.PlayConditionSound("LevelClear");

        playerRb.gravityScale = 5f;

        while (playerRb.transform.position.x < castlePosition.position.x)
        {
            playerRb.transform.position = Vector3.MoveTowards(playerRb.transform.position, castlePosition.position, walkSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Player reached the castle.");
        PlayerReachedCastle();
    }

    private void PlayerReachedCastle()
    {
        Debug.Log("Player finished walk. Converting time to score...");
        TimeManager timeManager = FindAnyObjectByType<TimeManager>();
        timeManager?.ConvertTimeToScore();
        gameFinished = true;
    }
}
