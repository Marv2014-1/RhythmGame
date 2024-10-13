using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Movement speed of the player

    // Rigidbody component for physics-based movement
    private Rigidbody2D rb;
    public Animator anim;
    public static bool canMove = true;

    // Store player movement vector
    private Vector2 movement;

    void Start()
    {
        // Get the Rigidbody2D and animator components from the game object
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame which is good for user input
    void Update()
    {
        // Check if the player is able to move
        if (canMove)
        {
            processInput();
            // Switch between idle and running animations as needed
            if (movement != Vector2.zero)
            {
                anim.SetBool("IsMoving", true);
            }
            else
            {
                anim.SetBool("IsMoving", false);
            }
        }
        
    }

    // FixedUpdate is called at a fixed interval which is good for physics
    void FixedUpdate()
    {
        Move();
    }

    // Process the player input
    void processInput()
    {
        // Capture the input from the player
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
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
}