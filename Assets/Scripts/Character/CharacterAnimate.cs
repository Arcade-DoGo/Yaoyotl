using UnityEngine;
using System.Linq;
using Photon.Pun;
using System;

public class CharacterAnimate : MonoBehaviour
{
    public GameObject character;
    [HideInInspector] public string animationState;

    private CharacterController controller;
    private CharacterStats stats;
    private Animator animator;
    private PhotonView view;
    private Rigidbody rb;

    private readonly string[] interruptable = { "Finished", "Fall" },
    uninterruptable = { "GrabLedge", "RecoverFromLedge", "FinalAttack",
                        "LDAttack", "LFAttack", "LUAttack",
                        "SDAttack", "SFAttack", "SUAttack" };

    void Awake()
    {
        ComponentsManager cm = character.GetComponent<ComponentsManager>();
        controller = cm.characterController;
        stats = cm.characterStats;
        animator = cm.animator;
        view = cm.photonView;
        rb = cm.rigidbody;
        
        animationState = "Idle";
        if (PhotonNetwork.IsConnected && !character.name.Contains("Offline")) 
        {
            cm.photonAnimatorView.SetLayerSynchronized(0, PhotonAnimatorView.SynchronizeType.Continuous);
            cm.photonAnimatorView.SetLayerSynchronized(1, PhotonAnimatorView.SynchronizeType.Disabled);
        }
    }

    void Update()
    {
        if (uninterruptable.Contains(animationState)) return;

        float movement = Mathf.Abs(rb.velocity.x);
        bool isFalling = rb.velocity.y < 0f && !stats.isGrounded;
        bool reset = !isFalling && interruptable.Contains(animationState);

        animator.SetFloat("movement", movement);
        if (isFalling && !stats.inHitStun) sendAnimation("Fall");
        if (reset)
        {
            if (movement < 0.1f) sendAnimation("Idle");
            else sendAnimation("RunCycle");
        }
    }
    public void setAnimationState(string animation) => animationState = animation;
    public void finishAnimation() => animationState = "Finished";
    public void sendAnimation(string animationName)
    {
        if(PhotonNetwork.IsConnected && !character.name.Contains("Offline")) view.RPC("playAnimation", RpcTarget.All, animationName);
        else controller.playAnimation(animationName);
    }
}