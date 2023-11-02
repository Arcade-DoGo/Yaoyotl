using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header ("Private references")]
    private Rigidbody rb;
    private CharacterStats stats;
    private InputManagement inputManagement;
    private CharacterAnimate anim;
    // private int jumpFramesCounter;
    // private bool isJumpPressed;

    void Start()
    {
        rb = GetComponent<ComponentsManager>().rigidbody; // Reference to rigidbody
        stats = GetComponent<ComponentsManager>().characterStats; // Reference to stats
        inputManagement = GetComponent<ComponentsManager>().inputManagement; // Reference to inputs
        anim = GetComponent<ComponentsManager>().charAnim; // Reference to animations
        rb.mass = stats.weight; // Sets character weight
    }

    void Update()
    {   
        if (stats.inHitStun || stats.isGettingUp)
            return;

        if (stats.isDoubleJumping)
            stats.isDoubleJumping = false;

        move();
        jump();
        // if (inputManagement.jumpInput) isJumpPressed = true; // Start Jump
        // else if (inputManagement.jumpRelease) // Perform Jump
        // {
        //     jump();
        //     isJumpPressed = false;
        //     inputManagement.jumpRelease = false;
        // }
        // if (isJumpPressed) jumpFramesCounter++; // Count Jump Frames

        if (inputManagement.crouchInput) // Down input
        {
            if (stats.canFastFall) fastFall(); // Fast Fall 
            else dropFromPlatform(); // Drop From Platforms
        }


    }

    void move()
    {
        // Set movement speed grounded or air
        float moveSpeed = stats.isGrounded ? stats.groundSpeed : stats.airSpeed;

        Vector3 movement = new(inputManagement.horizontal, 0.0f, 0.0f);
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0.0f);

        if (inputManagement.horizontal < 0f && stats.isFacingRight || // Turn Left
            inputManagement.horizontal > 0f && !stats.isFacingRight) // Turn Right
        {
            flipCharacter();
        }
    }

    void flipCharacter()
    {
        int direction = stats.isFacingRight ? 1 : -1;
        transform.Rotate(new Vector3(0, direction * 90, 0));
        stats.isFacingRight = !stats.isFacingRight;
    }

    void jump()
    {
        // // Set force for short (0-10 frames) or full hop (11+ frames) 
        // float jumpForce = jumpFramesCounter > 10 ? stats.jumpForce : stats.shortJumpForce;
        // jumpFramesCounter = 0; // Reset frame counter for short/full hop
        if(inputManagement.jumpInput && stats.jumpsUsed < stats.maxJumps)
        {
            anim.playAnimation("Jump");

            if (!stats.isGrounded && !stats.onLedge) // Airborne
            {
                // Remove ground jump if in the air (Jumps count as double jumps)
                stats.jumpsUsed = stats.jumpsUsed <= 0 ? 1 : stats.jumpsUsed;
                rb.velocity = Vector3.zero; // Reset speed before double jump (Not grounded jump)
                stats.isDoubleJumping = true;
            }
            rb.velocity += Vector3.up * CalculateJumpForce();
            gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
            stats.canFastFall = true;
            stats.jumpsUsed++;
        }
    }

    float CalculateJumpForce() => (rb.velocity.y < 0 ? stats.fallMultiplier - 1 : stats.lowJumpMultiplier - 1) * Physics.gravity.y * Time.deltaTime;
    void dropFromPlatform()
    {
        gameObject.layer = gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
        rb.velocity = Vector3.down * stats.fallForce;
    }

    void fastFall()
    {
        rb.velocity = Vector3.down * stats.fallForce;
        stats.canFastFall = false;
    }
}