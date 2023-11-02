using UnityEngine;

/// <summary>
/// Description: Manages short and long jump input, as well as fallspeed and long jump multiplier
/// Code based on "Celeste-Movement" by André Cardoso
/// </summary>

public class BetterJumping : MonoBehaviour
{
    [Tooltip("Value determining how fast the player will fall")]
    public float fallMultiplier = 2.5f;
    [Tooltip("Value determining how far will the long jump go")]
    public float lowJumpMultiplier = 2f;

    private InputManagement inputManagement;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Reference to rigidbody
        inputManagement = GetComponent<ComponentsManager>().inputManagement; // Reference to inputs
    }
    
    void Update()
    {
        if(rb.velocity.y < 0) rb.velocity += (fallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector2.up;
        else if(rb.velocity.y > 0 && !inputManagement.jumpInput) rb.velocity += (lowJumpMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector2.up;
    }
}