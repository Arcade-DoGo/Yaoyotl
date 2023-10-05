using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject normalAttack;
    private GameObject otherPlayer;

    // Start is called before the first frame update
    void Start()
    {
        normalAttack.SetActive(false);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != gameObject)
                otherPlayer = player;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
