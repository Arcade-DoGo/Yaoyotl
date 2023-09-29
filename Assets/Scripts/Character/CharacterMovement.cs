using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    private Rigidbody rb;
    private CharacterStats stats;
    public bool isGrounded;
    public bool onLedge;

    private bool canFastFall;

    private bool isJumpPressed;
    private int jumpFramesCounter;


    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Reference to rigidbody
        stats = GetComponent<CharacterStats>(); // Reference to stats
        rb.mass = stats.weight; // Sets character weight
    }

    void Update()
    {
        bool jumpInput = Input.GetButtonDown("Jump"); // Press Jump (Up)
        bool jumpRelease = Input.GetButtonUp("Jump"); // Release Jump (Up)
        bool crouchInput = Input.GetButtonDown("Crouch"); // Press Crouch (Down)

        move();

        if (jumpInput) // Start Jump
        {
            isJumpPressed = true;
        }
        else if (jumpRelease) // Perform Jump
        {
            Debug.Log("Jump button was pressed for " + jumpFramesCounter + " frames");
            isJumpPressed = false;
            jump();
        }

        if (isJumpPressed) // Count Jump Frames
        {
            jumpFramesCounter++;
        }

        if (crouchInput) // Drop From Platforms
        {
            dropFromPlatform();
        }

        if (crouchInput && canFastFall) // Fast Fall
        {
            fastFall();
        }
    }

    void move()
    {
        // Set movement speed grounded or air
        float moveSpeed = isGrounded ? stats.groundSpeed : stats.airSpeed;

        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontalInput, 0.0f, 0.0f);
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0.0f);
    }

    void jump()
    {
        // Set force for short (0-10 frames) or full hop (11+ frames) 
        float jumpForce = jumpFramesCounter > 10 ? stats.jumpForce : stats.shortJumpForce;
        jumpFramesCounter = 0; // Reset frame counter for short/full hop

        if (stats.jumpsUsed < stats.maxJumps)
        {
            if (!isGrounded && !onLedge) // Airborne
            {
                // Remove ground jump if in the air (Jumps count as double jumps)
                stats.jumpsUsed = stats.jumpsUsed <= 0 ? 1 : stats.jumpsUsed;
                rb.velocity = Vector3.zero; // Reset speed before double jump (Not grounded jump)
            }
            rb.velocity = Vector3.up * jumpForce;
            gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
            canFastFall = true;
            stats.jumpsUsed++;
        }
    }

    void dropFromPlatform()
    {
        float fallForce = stats.fallForce;

        gameObject.layer = gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
        Vector3 fallDirection = Vector3.down;
        rb.velocity = (fallDirection * fallForce);
    }

    void fastFall()
    {
        float fallForce = stats.fallForce;

        Vector3 fallDirection = Vector3.down;
        rb.velocity = (fallDirection * fallForce);
        canFastFall = false;
    }

}
