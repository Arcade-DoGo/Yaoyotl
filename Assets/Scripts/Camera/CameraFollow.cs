using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float zoomSpeed = 5f, smoothSpeed = 5f, minSize, maxSize;
    private float limitX, limitY;
    public Transform blastzone;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
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
        if (target == null) return;
        Vector3 desiredPosition = new (target.position.x, target.position.y, transform.position.z); // Follow Players' Position
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
        float minDistance = maxSize;
        for (int i = 0; i < GameManager.players.Count; i++)
        {
            for (int j = i + 1; j < GameManager.players.Count; j++)
            {
                Transform player1 = GameManager.players[i].transform;
                Transform player2 = GameManager.players[j].transform;
                float _distance = Vector3.Distance(player1.position, player2.position);
                if(_distance < minDistance) minDistance = _distance;
            }
        }
        float targetSize = Mathf.Clamp(minDistance, minSize, maxSize);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }
}