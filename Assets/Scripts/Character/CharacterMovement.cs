using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterStats stats;
    public bool isGrounded;

    private bool canFastFall;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        bool jumpInput = Input.GetButtonDown("Jump");
        bool crouchInput = Input.GetButtonDown("Crouch");

        move();

        if (isGrounded)
        {
            if (jumpInput) // Jumping
            {
                jump();
            }

            if (crouchInput) // Drop From Platforms
            {
                dropFromPlatform();
            }
        }
        else if (crouchInput && canFastFall) // Fast Fall
        {
            fastFall();
        }
    }

    void move()
    {
        float moveSpeed = stats.groundSpeed;

        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontalInput, 0.0f, 0.0f);
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, 0.0f);
    }

    void jump()
    {
        float jumpForce = stats.jumpForce;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
        canFastFall = true;
    }

    void dropFromPlatform()
    {
        float fallForce = stats.fallForce;

        gameObject.layer = gameObject.layer = LayerMask.NameToLayer("PlatformLayer"); // Prevents collision with platforms
        Vector3 fallDirection = Vector3.down;
        rb.AddForce(fallDirection * fallForce / 2);
    }

    void fastFall()
    {
        float fallForce = stats.fallForce;

        Vector3 fallDirection = Vector3.down;
        rb.AddForce(fallDirection * fallForce);
        canFastFall = false;
    }

}
