using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header ("Private references")]
    private Rigidbody rb;
    private CharacterStats stats;
    private InputManagement inputManagement;
    private CharacterAnimate anim;

    void Start()
    {
        rb = GetComponent<ComponentsManager>().rigidbody; // Reference to rigidbody
        anim = GetComponent<ComponentsManager>().charAnim; // Reference to animations
        stats = GetComponent<ComponentsManager>().characterStats; // Reference to stats
        inputManagement = GetComponent<ComponentsManager>().inputManagement; // Reference to inputs
        rb.mass = stats.weight; // Sets character weight
    }

    void Update()
    {   
        if (stats.inHitStun || stats.isGettingUp) return;
        if (stats.isDoubleJumping) stats.isDoubleJumping = false;

        TrackMovement();
        TrackJumping();

        if (inputManagement.crouchInput) // Down input
        {
            if (stats.canFastFall) FastFall(); // Fast Fall 
            else DropFromPlatform(); // Drop From Platforms
        }


    }

    void TrackMovement()
    {
        // Set movement speed grounded or air
        float moveSpeed = stats.isGrounded ? stats.groundSpeed : stats.airSpeed;

        Vector3 movement = new(inputManagement.horizontal, 0.0f, 0.0f);
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0.0f);
        if (inputManagement.horizontal < 0f && stats.isFacingRight || // Turn Left
            inputManagement.horizontal > 0f && !stats.isFacingRight) // Turn Right
        {
            FlipCharacter();
        }
    }

    void FlipCharacter()
    {
        int direction = stats.isFacingRight ? 1 : -1;
        transform.Rotate(90 * direction * Vector3.up);
        stats.isFacingRight = !stats.isFacingRight;
    }

    void TrackJumping()
    {
        if(inputManagement.jumpInput && stats.jumpsUsed < stats.maxJumps)
        {
            anim.playAnimation("Jump");
            if (!stats.isGrounded && !stats.onLedge) // Airborne
            {
                stats.jumpsUsed = stats.jumpsUsed <= 0 ? 1 : stats.jumpsUsed; // Remove ground jump if in the air (Jumps count as double jumps)
                rb.velocity = Vector3.zero; // Reset speed before double jump (Not grounded jump)
                stats.isDoubleJumping = true;
            }
            rb.velocity += Vector3.up * stats.jumpForce; // Set initial jump force
            gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
            stats.canFastFall = true;
            stats.jumpsUsed++;
        }

        if(rb.velocity.y < 0) // Add fall speed when free falling
            rb.velocity += stats.fallMultiplier * Physics.gravity.y * Time.deltaTime * Vector3.up;
        else if(rb.velocity.y > 0 && !inputManagement.jumpHold) // Reduce player velocity when going up if jump is not being pressed (allow long/short jump)
            rb.velocity += stats.lowJumpMultiplier * Physics.gravity.y * Time.deltaTime * Vector3.up;
    }
    void DropFromPlatform()
    {
        gameObject.layer = gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
        rb.velocity = Vector3.down * stats.fallForce;
    }

    void FastFall()
    {
        rb.velocity = Vector3.down * stats.fallForce;
        stats.canFastFall = false;
    }
}