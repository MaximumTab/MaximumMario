using UnityEngine;

public class PointSpawner : MonoBehaviour
{
    public GameObject floatingPointsPrefab; 
    public Sprite[] pointSprites; 

    public void SpawnPoints(Vector3 position, int points)
    {
        Vector3 spawnPosition = position + new Vector3(0, 1.5f, 0);

        GameObject newPoints = Instantiate(floatingPointsPrefab, spawnPosition, Quaternion.identity);
        SpriteRenderer spriteRenderer = newPoints.GetComponent<SpriteRenderer>();

        switch (points)
        {
            case 50:
                return; // Do nothing (makes 50 points invisible)
            case 100:
                spriteRenderer.sprite = pointSprites[0];
                break;
            case 200:
                spriteRenderer.sprite = pointSprites[1];
                break;
            case 400:
                spriteRenderer.sprite = pointSprites[2];
                break;
            case 500:
                spriteRenderer.sprite = pointSprites[3];
                break;
            case 800:
                spriteRenderer.sprite = pointSprites[4];
                break;
            case 1000:
                spriteRenderer.sprite = pointSprites[5];
                break;
            case 2000:
                spriteRenderer.sprite = pointSprites[6];
                break;
            case 4000:
                spriteRenderer.sprite = pointSprites[7];
                break;
            case 5000:
                spriteRenderer.sprite = pointSprites[8];
                break;
            case 8000:
                spriteRenderer.sprite = pointSprites[9];
                break;
            default:
                Debug.LogWarning($"No sprite found for points: {points}");
                return;
        }
    }

}
