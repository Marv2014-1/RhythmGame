using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Movement speed of the player

    private Rigidbody2D rb;  // Rigidbody component for physics-based movement
    private Animator animator;  // Reference to the Animator component in the child object
    public static bool canMove;

    private Vector2 movement;  // Store player movement vector

    private bool isTurning = false;
    private bool facingRight = true;
    private float previousXDirection = 0f;

    void Start()
    {
        // Allow player movement on game start
        canMove = true;

        // Get the Rigidbody2D and animator components from the game object
        rb = GetComponent<Rigidbody2D>();

        // Get the Animator component from the child object named "Animation"
        animator = transform.Find("Animation").GetComponent<Animator>();
    }

    // Update is called once per frame which is good for user input
    void Update()
    {
        // Check if the player is able to move
        if (canMove)
        {
            ProcessInput();
            // Switch between idle and running animations as needed
            if (movement != Vector2.zero)
            {
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
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

        // Move the player if able
        if (canMove)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Coroutine to handle flip animation
    IEnumerator Flip()
    {
        isTurning = true;
        float time = 0f;
        float duration = 0.15f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);

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
}
