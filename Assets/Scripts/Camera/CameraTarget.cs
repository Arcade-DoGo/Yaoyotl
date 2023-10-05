using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{

    private GameObject[] players = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position1 = players[0].transform.position;
        Vector3 position2 = players[1].transform.position;
        Vector3 centerPosition = new Vector3((position1.x + position2.x) / 2,
         (position1.y + position2.y) / 2,
          0);
        transform.position = centerPosition;
    }
}
