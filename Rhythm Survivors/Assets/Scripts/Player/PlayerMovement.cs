using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Movement speed of the player

    private Rigidbody2D rb;  // Rigidbody component for physics-based movement
    private Animator animator;  // Reference to the Animator component in the child object

    private Vector2 movement;  // Store player movement vector

    private bool isTurning = false;
    private bool facingRight = true;
    private bool bowFacingRight = true; // Tracks the bow's facing direction
    private float previousXDirection = 0f;

    private Transform bow;  // Reference to the Bow object

    void Start()
    {
        // Get the Rigidbody2D component from the game object
        rb = GetComponent<Rigidbody2D>();

        // Get the Animator component from the child object named "Animation"
        animator = transform.Find("Animation").GetComponent<Animator>();

        // Find the Bow in the BowContainer
        Transform bowContainer = transform.Find("BowContainer");
        if (bowContainer != null)
        {
            bow = bowContainer.Find("Bow");
        }
        else
        {
            Debug.LogError("BowContainer not found!");
        }

        if (bow == null)
        {
            Debug.LogError("Bow not found in BowContainer!");
        }

        // Ensure the bow starts facing the correct direction
        if (bow != null)
        {
            Vector3 bowScale = bow.localScale;
            bowScale.x = bowFacingRight ? Mathf.Abs(bowScale.x) : -Mathf.Abs(bowScale.x);
            bow.localScale = bowScale;
        }
    }

    // Update is called once per frame which is good for user input
    void Update()
    {
        ProcessInput();

        // Calculate player speed based on movement
        float playerSpeed = movement.magnitude;

        // Update the Animator's speed parameter
        animator.SetFloat("Speed", playerSpeed);
    }

    // FixedUpdate is called at a fixed interval which is good for physics
    void FixedUpdate()
    {
        Move();
    }

    // Process the player input
    void ProcessInput()
    {
        // Capture the input from the player
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        float currentXDirection = movement.x;

        // Detect if the player has changed horizontal direction
        if (currentXDirection != 0 && currentXDirection != previousXDirection)
        {
            if ((currentXDirection > 0 && !facingRight) || (currentXDirection < 0 && facingRight))
            {
                if (!isTurning)
                {
                    StartCoroutine(Flip());
                }
            }
        }

        previousXDirection = currentXDirection;
    }

    // Move the player
    void Move()
    {
        // Normalize the movement vector to keep the speed consistent in all directions
        if (movement != Vector2.zero)
        {
            movement.Normalize();
        }

        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // Coroutine to handle flip animation
    IEnumerator Flip()
    {
        isTurning = true;
        float time = 0f;
        float duration = 0.15f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);

        // Flip the bow independently
        FlipBow();

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        facingRight = !facingRight;

        isTurning = false;
    }

    // Method to flip the bow's direction
    void FlipBow()
    {
        if (bow != null)
        {
            Vector3 bowScale = bow.localScale;
            bowScale.x = bowFacingRight ? -Mathf.Abs(bowScale.x) : Mathf.Abs(bowScale.x);
            bow.localScale = bowScale;
            bowFacingRight = !bowFacingRight; // Toggle the bow's facing direction
        }
    }
}
