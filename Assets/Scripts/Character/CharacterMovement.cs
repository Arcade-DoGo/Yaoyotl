using System;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterStats stats;
    private int jumpFramesCounter;
    private bool canFastFall, isJumpPressed;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Reference to rigidbody
        stats = GetComponent<CharacterStats>(); // Reference to stats
        rb.mass = stats.weight; // Sets character weight
    }

    void Update()
    {
        if (stats.inHitStun)
            return;


        move();

        if (InputManagement.jumpInput) isJumpPressed = true; // Start Jump
        else if (InputManagement.jumpRelease) // Perform Jump
        {
            jump();
            isJumpPressed = false;
            InputManagement.jumpRelease = false;
        }

        if (isJumpPressed) jumpFramesCounter++; // Count Jump Frames
        if (InputManagement.crouchInput) // Down input
        {
            if (canFastFall) fastFall(); // Drop From Platforms
            else dropFromPlatform(); // Fast Fall
        }
    }

    void move()
    {
        // Set movement speed grounded or air
        float moveSpeed = stats.isGrounded ? stats.groundSpeed : stats.airSpeed;
        Vector3 movement = new(InputManagement.horizontal, 0.0f, 0.0f);
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0.0f);

    }

    void jump()
    {
        // Set force for short (0-10 frames) or full hop (11+ frames) 
        float jumpForce = jumpFramesCounter > 10 ? stats.jumpForce : stats.shortJumpForce;
        jumpFramesCounter = 0; // Reset frame counter for short/full hop

        if (stats.jumpsUsed < stats.maxJumps)
        {
            if (!stats.isGrounded && !stats.onLedge) // Airborne
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
        gameObject.layer = gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
        rb.velocity = Vector3.down * stats.fallForce;
    }

    void fastFall()
    {
        rb.velocity = Vector3.down * stats.fallForce;
        canFastFall = false;
    }
}