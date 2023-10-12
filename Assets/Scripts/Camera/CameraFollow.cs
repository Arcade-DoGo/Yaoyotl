using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float zoomSpeed = 5f;
    public float smoothSpeed = 5f; 
    public Transform blastzone;
    private Camera cam;
    // private List<GameObject> players;
    private float limitX;
    private float limitY;
    public float minSize;
    public float maxSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        // players = GameManager.FindPlayers();
        float minX = 0;
        float maxX = 0;
        float minY = 0;
        float maxY = 0;

        foreach (Transform child in blastzone)
        {
            minX = child.position.x < minX ? child.position.x : minX;
            maxX = child.position.x > maxX ? child.position.x : maxX;
            minY = child.position.y < minY ? child.position.y : minY;
            maxY = child.position.y > maxY ? child.position.y : maxY;
        }

        limitX = Mathf.Abs(maxX - minX) / 5f;
        limitY = Mathf.Abs(maxY - minY) / 5f;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + Vector3.back * 10; // Follow Players' Position

        // Keep Camera Within Limits
        Vector3 clampedPosition = new (
            Mathf.Clamp(desiredPosition.x, -limitX, limitX),
            Mathf.Clamp(desiredPosition.y, -limitY, limitY),
            desiredPosition.z
        );

        // Move Camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Ajust Camera's Field of View
        Transform player1 = GameManager.players[0].transform;
        Transform player2 = GameManager.players[1].transform;
        float distance = Vector3.Distance(player1.position, player2.position);
        float targetSize = Mathf.Clamp(distance, minSize, maxSize);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }
}