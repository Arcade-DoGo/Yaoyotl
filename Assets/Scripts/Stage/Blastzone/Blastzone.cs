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
        leftWall.transform.position = new Vector3(-width, 0f, 0f);
        rightWall.transform.position = new Vector3(width, 0f, 0f);
        ceiling.transform.position = new Vector3(0f, height, 0f);
        floor.transform.position = new Vector3(0f, -height, 0f);
    }

    public void respawnCharacter(GameObject character)
    {
        CharacterStats stats = character.GetComponent<ComponentsManager>().characterStats;
        Rigidbody rb = character.GetComponent<ComponentsManager>().rigidbody;

        stats.inHitStun = false;
        character.transform.position = new Vector3(0.0f, 5.0f, 0.0f);
        rb.velocity = Vector3.zero;
    }
}
