using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterStats stats;
    private InputManagement inputManagement;
    private int jumpFramesCounter;
    private bool isJumpPressed, isFacingRight = true;

    void Start()
    {
        rb = GetComponent<ComponentsManager>().rigidbody; // Reference to rigidbody
        stats = GetComponent<ComponentsManager>().characterStats; // Reference to stats
        inputManagement = GetComponent<ComponentsManager>().inputManagement; // Reference to inputs
        rb.mass = stats.weight; // Sets character weight
    }

    void Update()
    {
        if (stats.inHitStun)
            return;

        move();

        if (inputManagement.jumpInput) isJumpPressed = true; // Start Jump
        else if (inputManagement.jumpRelease) // Perform Jump
        {
            jump();
            isJumpPressed = false;
            inputManagement.jumpRelease = false;
        }

        if (isJumpPressed) jumpFramesCounter++; // Count Jump Frames
        if (inputManagement.crouchInput) // Down input
        {
            if (stats.canFastFall) fastFall(); // Fast Fall 
            else dropFromPlatform(); // Drop From Platforms

            stats.setDirectionalInput("down"); // Set Last Input for attack direction
        }

        
    }

    void move()
    {
        // Set movement speed grounded or air
        float moveSpeed = stats.isGrounded ? stats.groundSpeed : stats.airSpeed;
        Vector3 movement = new(inputManagement.horizontal, 0.0f, 0.0f);
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0.0f);

        if (inputManagement.horizontal < 0f && isFacingRight || // Turn Left
            inputManagement.horizontal > 0f && !isFacingRight) // Turn Right
        {
            flipCharacter();
        }

        stats.setMovement(Mathf.Abs(inputManagement.horizontal));

        if (movement != Vector3.zero) // Set Last Input for attack direction
            stats.setDirectionalInput("forward");
    }

    void flipCharacter()
    {
        int direction = isFacingRight ? 1 : -1;
        transform.Rotate(new Vector3(0, direction * 90, 0));
        isFacingRight = !isFacingRight;
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
            stats.setCanFastFall(true);
            stats.jumpsUsed++;

            stats.setDirectionalInput("up"); // Set Last Input for attack direction
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
        stats.setCanFastFall(false);
    }
}