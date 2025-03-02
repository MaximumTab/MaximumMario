using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    private Transform player;
    private float leftScreenBound;
    public float height = 6.5f;

    [Header("Underground")]
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;
    
    [Header("AboveGround")]

    public float AbovegroundHeight = -9.5f;
    public float AbovegroundThreshold = 0f;

    private void Awake()
    {
        // Try to find "Big Mario", if not found, try "Small Mario"
        GameObject playerObj = GameObject.FindWithTag("BigMario") ?? GameObject.FindWithTag("SmallMario");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("No player found! Ensure the player has either 'Big Mario' or 'Small Mario' tag.");
        }
    }

    private void LateUpdate()
    {
        if (player == null) return; // Avoid errors if player isn't found

        leftScreenBound = Camera.main.ViewportToWorldPoint(Vector3.zero).x;

        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
        transform.position = cameraPosition;

        Vector3 playerPosition = player.position;
        playerPosition.x = Mathf.Max(playerPosition.x, leftScreenBound);
        player.position = playerPosition;
    }

    public void SetUnderground(bool underground)
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
        
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic("UGmusic");
    }

public void SetRightwardPipeTransition(bool rightPipe)
{
    Vector3 cameraPosition = transform.position;
    cameraPosition.y = rightPipe ? AbovegroundHeight : height; 
    transform.position = cameraPosition;
    
    AudioManager.Instance.StopMusic();
    AudioManager.Instance.PlayMusic("BGM"); 
}
}