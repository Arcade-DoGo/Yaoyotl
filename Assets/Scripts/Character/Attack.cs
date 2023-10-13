using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //public GameObject normalAttack;
    private CharacterStats stats;
    private InputManagement inputManagement;
    private int frameCounter;
    private int FRAMES_STRONG = 30;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<ComponentsManager>().characterStats;
        inputManagement = GetComponent<ComponentsManager>().inputManagement;
        //normalAttack.SetActive(false);
        stats.setIsAttacking(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManagement.attackInput && !stats.isAttacking)
        {
            stats.setIsAttacking(true);
        }

        if (inputManagement.attackRelease && stats.isAttacking)
        {
            stats.setAttackDirection(getAttackDirection());
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
        print("Regular Attack!");
        stats.setAttackStrength(true);
        //normalAttack.SetActive(true);

        yield return new WaitForSeconds(0.5f); // Active Hitbox Duration
        //normalAttack.SetActive(false);
        stats.setIsAttacking(false);
        frameCounter = 0;
    }

    IEnumerator strongAttack()
    {
        // Strong Attack
        print("Strong Attack!");
        stats.setAttackStrength(false);
        yield return null;
        stats.setIsAttacking(false);
        frameCounter = 0;
    }

    IEnumerator finalAttack()
    {
        // Final Smash
        print("Final Attack!");
        stats.animateFinalAttack(true);
        yield return null;
        stats.setIsAttacking(false);
        frameCounter = 0;
        stats.animateFinalAttack(false);
        stats.resetFAM();
    }

    private string getAttackDirection()
    {
        string direction = "forward";

        if (inputManagement.jumpInput || inputManagement.jumpHold)
        {
            direction = "up";
        }
        else if (inputManagement.crouchInput || inputManagement.crouchHold)
        {
            direction = "down";
        }

        return direction;
    }
}
