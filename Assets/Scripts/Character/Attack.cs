using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject normalAttack;
    private CharacterStats stats;
    private InputManagement inputManagement;
    private int frameCounter;
    private int FRAMES_STRONG = 30;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<ComponentsManager>().characterStats;
        inputManagement = GetComponent<ComponentsManager>().inputManagement;
        normalAttack.SetActive(false);
        stats.isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManagement.attackInput && !stats.isAttacking)
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
        print("Regular Attack!");
        normalAttack.SetActive(true);
        yield return new WaitForSeconds(0.5f); // Active Hitbox Duration
        normalAttack.SetActive(false);
        stats.isAttacking = false;
        frameCounter = 0;
    }

    IEnumerator strongAttack()
    {
        // Strong Attack
        print("Strong Attack!");
        yield return null;
        stats.isAttacking = false;
        frameCounter = 0;
    }

    IEnumerator finalAttack()
    {
        // Final Smash
        print("Final Attack!");
        yield return null;
        stats.isAttacking = false;
        frameCounter = 0;

        stats.resetFAM();
    }
}
