using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public float yOffset;
    private float xCenter, yCenter;
    void Update()
    {
        xCenter = yCenter = 0f;
        foreach (CharacterStats player in GameManager.players)
        {
            xCenter += player.transform.position.x;
            yCenter += player.transform.position.y;
        }
        transform.position = new (xCenter / GameManager.players.Count,
                        yCenter / GameManager.players.Count + yOffset, 0);
    }
}
