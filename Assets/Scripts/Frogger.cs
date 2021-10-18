using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Frogger : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }

    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;

    private Vector3 spawnPosition;
    private float farthestRow;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Move(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            Move(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Move(Vector3.down);
        }
    }

    private void Move(Vector3 direction)
    {
        Vector3 destination = transform.position + direction;

        // Check for collision at the destination
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));

        // Prevent any movement if there is a barrier
        if (barrier != null) {
            return;
        }

        // Attach/detach frogger from the platform
        if (platform != null) {
            transform.SetParent(platform.transform);
        } else {
            transform.SetParent(null);
        }

        // Frogger dies when it hits an obstacle
        if (obstacle != null && platform == null)
        {
            transform.position = destination;
            Death();
        }
        // Conditions pass, move to the destination
        else
        {
            // Check if we have advanced to a farther row
            if (destination.y > farthestRow)
            {
                farthestRow = destination.y;
                FindObjectOfType<GameManager>().AdvancedRow();
            }

            // Start leap animation
            StopAllCoroutines();
            StartCoroutine(Leap(destination));
        }
    }

    private IEnumerator Leap(Vector3 destination)
    {
        Vector3 startPosition = transform.position;

        float elapsed = 0f;
        float duration = 0.125f;

        // Set initial state
        spriteRenderer.sprite = leapSprite;

        while (elapsed < duration)
        {
            // Move towards the destination over time
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final state
        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
    }

    public void Respawn()
    {
        // Stop animations
        StopAllCoroutines();

        // Reset transform to spawn
        transform.rotation = Quaternion.identity;
        transform.position = spawnPosition;
        farthestRow = spawnPosition.y;

        // Reset sprite
        spriteRenderer.sprite = idleSprite;

        // Enable control
        gameObject.SetActive(true);
        enabled = true;
    }

    public void Death()
    {
        // Stop animations
        StopAllCoroutines();

        // Disable control
        enabled = false;

        // Display death sprite
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;

        // Update game state
        FindObjectOfType<GameManager>().Died();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool hitObstacle = other.gameObject.layer == LayerMask.NameToLayer("Obstacle");
        bool onPlatform = transform.parent != null;

        if (enabled && hitObstacle && !onPlatform) {
            Death();
        }
    }

}
