using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private List<GameObject> players;
    void Start() => players = GameManager.FindPlayers();
    void Update()
    {
        if(players.Count == 0) players = GameManager.FindPlayers();
        Vector3 position1 = players[0].transform.position;
        Vector3 position2 = players[1].transform.position;
        Vector3 centerPosition = new ((position1.x + position2.x) / 2,
                                    (position1.y + position2.y) / 2, 0);
        transform.position = centerPosition;
    }
}
