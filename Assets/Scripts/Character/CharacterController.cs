using Photon.Pun;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Animator animator;
    private CharacterAnimate anim;
    private void Awake()
    {
        ComponentsManager cm = GetComponent<ComponentsManager>();
        animator = cm.animator; // Reference to Animator
        anim = cm.charAnim; // Reference to animations
    }
    
    [PunRPC]
    public void playAnimation(string animationName)
    {
        if(anim == null || animator == null)
        {
            ComponentsManager cm = GetComponent<ComponentsManager>();
            animator = cm.animator;
            anim = cm.charAnim;
        }

        anim.animationState = animationName;
        animator.Play(animationName);
    }
}