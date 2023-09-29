using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blastzone : MonoBehaviour
{

    public float height = 15;
    public float width = 15;
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject ceiling;
    public GameObject floor;

    void Start()
    {
        leftWall.transform.localPosition = new Vector3(-width, 0f, 0f);
        rightWall.transform.localPosition = new Vector3(width, 0f, 0f);
        ceiling.transform.localPosition = new Vector3(0f, height, 0f);
        floor.transform.localPosition = new Vector3(0f, -height, 0f);
    }

    public void respawnCharacter(GameObject character)
    {
        CharacterStats stats = character.GetComponent<CharacterStats>();
        Rigidbody rb = character.GetComponent<Rigidbody>();

        stats.damage = 0;
        stats.stocks--;
        character.transform.position = new Vector3(0.0f, 5.0f, 0.0f);
        rb.velocity = Vector3.zero;
    }

    public void gameover()
    {
        Debug.Log("GAME!");
    }
}
