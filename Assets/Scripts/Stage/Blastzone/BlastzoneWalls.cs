using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastzoneWalls : MonoBehaviour
{

    Blastzone blastzone;
    public GameObject deathEffect;

    void Start()
    {
        blastzone = transform.parent.GetComponent<Blastzone>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        string tag = otherObject.tag;


        if (tag == "Player")
        {
            CharacterStats stats = otherObject.GetComponent<ComponentsManager>().characterStats;
            Instantiate(deathEffect, other.transform.position, Quaternion.identity);
            stats.loseStock();
            if (stats.stocks > 0)
            {
                blastzone.respawnCharacter(otherObject);
            }
            else
            {
                // Game Over: Show winner
                GameManager.instance.GameOver(GameManager.players.Find(player => player != stats));
            }
        }

    }
}
