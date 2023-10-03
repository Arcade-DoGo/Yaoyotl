using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public float damage = 0.0f; // Unused
    public float weight = 1.0f;
    public float groundSpeed = 10.0f;
    public float airSpeed = 7.5f; 
    public float jumpForce = 13.0f;
    public float shortJumpForce = 10.0f;
    public float fallForce = 10f;
    public int maxJumps = 2; 
    public int jumpsUsed = 0;
    public int stocks = 3;
}
