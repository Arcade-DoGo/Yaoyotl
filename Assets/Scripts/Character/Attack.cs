using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [NonSerialized] public bool isAttacking;

    public GameObject normalAttack;
    private GameObject otherPlayer;
    private CharacterStats otherStats;
    private CharacterMovement otherMovement;
    private int frameCounter;
    private int FRAMES_STRONG = 30;

    // Start is called before the first frame update
    void Start()
    {
        normalAttack.SetActive(false);
        isAttacking = false;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != gameObject)
                otherPlayer = player;
        }

        otherStats = otherPlayer.GetComponent<CharacterStats>();
        otherMovement = otherPlayer.GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManagement.attackInput && !isAttacking)
        {
            isAttacking = true;
        }

        if (InputManagement.attackRelease && isAttacking)
        {
            if (frameCounter < FRAMES_STRONG)
            {
                StartCoroutine(regularAttack());
            }
            else
            {
                StartCoroutine(strongAttack());
            }
            isAttacking = false;
            frameCounter = 0;
        }
        else if (InputManagement.finalAttackInput)
        {
            StartCoroutine(finalAttack());
        }

        if (isAttacking)
            frameCounter++;

    }

    IEnumerator regularAttack()
    {
        print("Regular Attack!");
        normalAttack.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        normalAttack.SetActive(false);
    }

    IEnumerator strongAttack()
    {
        // Strong Attack
        print("Strong Attack!");
        yield return null;
    }

    IEnumerator finalAttack()
    {
        // Final Smash
        print("Final Attack!");
        yield return null;
    }
}
