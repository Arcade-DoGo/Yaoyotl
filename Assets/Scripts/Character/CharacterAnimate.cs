using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterAnimate : MonoBehaviour
{

    public GameObject character;
    private Animator animator;
    private CharacterStats stats;
    private Rigidbody rb;

    // Idle, Fall, FinalAttack, GetHit,
    // GrabLedge, Jump, LDAttack,
    // LFAttack, LUAttack, SDAttack,
    // SFAttack, SUAttack, RecoverFromLedge, RunCycle,
    // WalkStartUp, EndRunCycle

    private string animationState = "Idle";
    private string[] interruptable = { "Finished", "Fall" };
    private string[] uninterruptable = { "GrabLedge", "RecoverFromLedge" };

    // Start is called before the first frame update
    void Start()
    {
        ComponentsManager cm = character.GetComponent<ComponentsManager>();
        animator = cm.animator;
        stats = cm.characterStats;
        rb = cm.rigidbody;
    }

    void Update()
    {
        if (uninterruptable.Contains(animationState)) return;

        float movement = Mathf.Abs(rb.velocity.x);
        bool isFalling = rb.velocity.y < 0f && !stats.isGrounded;
        bool reset = !isFalling && interruptable.Contains(animationState);

        animator.SetFloat("movement", movement);
        if (isFalling && !stats.inHitStun) playAnimation("Fall");
        if (reset){
            if (movement < 0.1f) playAnimation("Idle");
            else playAnimation("RunCycle");
        }

    }

    public void setAnimationState(string animation) { animationState = animation; }
    public void finishAnimation() { animationState = "Finished"; }

    public void playAnimation(string animationName)
    {
        if (!animator) animator = GetComponent<ComponentsManager>().animator;

        animator.Play(animationName);
        animationState = animationName;

    }

}
