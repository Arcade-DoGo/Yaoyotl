using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject normalAttack;
    private GameObject otherPlayer;
    private CharacterStats stats;
    private CharacterStats otherStats;
    private int frameCounter;
    private int FRAMES_STRONG = 30;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != gameObject)
                otherPlayer = player;
        }

        stats = GetComponent<CharacterStats>();
        otherStats = otherPlayer.GetComponent<CharacterStats>();

        normalAttack.SetActive(false);
        stats.isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManagement.attackInput && !stats.isAttacking)
        {
            stats.isAttacking = true;
        }

        if (InputManagement.attackRelease && stats.isAttacking)
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
        else if (InputManagement.finalAttackInput && stats.canFAM)
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
