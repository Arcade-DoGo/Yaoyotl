using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //public GameObject normalAttack;
    private CharacterStats stats;
    private InputManagement inputManagement;
    private CharacterAnimate anim;
    private Rigidbody rb;
    private FinalSmash fs;
    private int frameCounter;
    private int FRAMES_STRONG = 30;

    private float lightEndLag = 0.3f;
    private float strongEndLag = 0.7f;
    private float finalEndLag = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<ComponentsManager>().characterStats;
        inputManagement = GetComponent<ComponentsManager>().inputManagement;
        anim = GetComponent<ComponentsManager>().charAnim;
        rb = GetComponent<ComponentsManager>().rigidbody;
        fs = GetComponent<ComponentsManager>().finalSmash;
        //normalAttack.SetActive(false);
        stats.isAttacking = false;
        if(stats.NPC) enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManagement.attackInput && !stats.isAttacking && !stats.onLedge)
        {
            stats.isAttacking = true;
        }

        if (inputManagement.attackRelease && stats.isAttacking)
        {
            if (frameCounter < FRAMES_STRONG)
            {
                StartCoroutine(regularAttack());
            }
            else
            {
                StartCoroutine(strongAttack());
            }
        }
        else if (inputManagement.finalAttackInput && stats.canFAM)
        {
            StartCoroutine(finalAttack());
        }

        if (stats.isAttacking)
            frameCounter++;

    }

    IEnumerator regularAttack()
    {
        string attackName = "L" + getAttackDirection() + "Attack";
        anim.sendAnimation(attackName);

        yield return new WaitForSeconds(lightEndLag); // Active Hitbox Duration

        stats.isAttacking = false;
        frameCounter = 0;
    }

    IEnumerator strongAttack()
    {
        string attackName = "S" + getAttackDirection() + "Attack";
        anim.sendAnimation(attackName);

        yield return new WaitForSeconds(strongEndLag);

        stats.isAttacking = false;
        frameCounter = 0;
    }

    IEnumerator finalAttack()
    {
        // Final Smash
        string attackName = "FinalAttack";
        anim.sendAnimation(attackName);
        fs.triggerFinalSmash();

        rb.detectCollisions = false;
        rb.isKinematic = true;

        while (anim.animationState == "FinalAttack") yield return null;

        rb.detectCollisions = true;
        rb.isKinematic = false;

        stats.isAttacking = false;
        frameCounter = 0;
        stats.resetFAM();
    }

    private string getAttackDirection()
    {
        string direction = "F"; // Forward

        if (inputManagement.jumpInput || inputManagement.jumpHold)
        {
            direction = "U"; // Up
        }
        else if (inputManagement.crouchInput || inputManagement.crouchHold)
        {
            direction = "D"; // Down
        }

        return direction;
    }
}
