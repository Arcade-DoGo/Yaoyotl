using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private float xCenter, yCenter;
    void Update()
    {
        xCenter = yCenter = 0f;
        foreach (CharacterStats player in GameManager.players)
        {
            xCenter += player.transform.position.x;
            yCenter += player.transform.position.y;
        }
        Vector3 centerPosition = new (xCenter / GameManager.players.Count,
                                    yCenter / GameManager.players.Count, 0);
        transform.position = centerPosition;
    }
}
