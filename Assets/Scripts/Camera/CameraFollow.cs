using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header ("Public References")]
    public Transform target, blastzone;
    [Header ("Camera speed")]
    public float zoomSpeed = 5f; public float smoothSpeed = 5f;
    [Header ("Camera offset")]
    public float yOffset = 1f; public float zOffset = 0f;
    [Header ("Zoom size limits")]
    public float minSize = 1f; public float maxSize = 6f;
    [Header ("Distance Limits")]
    public float minDistance = 0f; public float maxDistance = 12f;
    [Header ("Camera Z distance limits")]
    public bool useZDistance;
    public float minZDistance = -12f; public float maxZDistance = -1.5f;
    private Camera cam;
    private Vector2 desiredPosition;
    private Vector3 clampedPosition;
    private Transform player1, player2;
    private float limitX, limitY, minX, maxX, minY, maxY, 
    newMinDistance, newMaxDistance, targetSize, minXPlayer, maxXPlayer, interpolatedDistance;

    void Start()
    {
        cam = GetComponent<Camera>();
        minX = maxX = minY = maxY = 0;

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

        CalculateCameraPosition();
        CalculatePlayersDistance();
        SetCameraValues();
    }

    private void CalculateCameraPosition()
    {
        // Follow Players' Position
        desiredPosition = new (target.position.x, target.position.y);

        // Keep Camera Within Limits
        clampedPosition = new (
            Mathf.Clamp(desiredPosition.x, -limitX, limitX),
            Mathf.Clamp(desiredPosition.y, -limitY, limitY) + yOffset,
            transform.position.z
        );
    }

    private void CalculatePlayersDistance()
    {
        // Get players distance
        newMaxDistance = maxDistance;
        newMinDistance = minDistance;
        interpolatedDistance = 0f;
        maxXPlayer = minXPlayer = GameManager.players[0].transform.position.x;
        for (int i = 0; i < GameManager.players.Count; i++)
        {
            player1 = GameManager.players[i].transform;
            maxXPlayer = Mathf.Max(player1.position.x, maxXPlayer);
            minXPlayer = Mathf.Min(player1.position.x, minXPlayer);
            for (int j = i + 1; j < GameManager.players.Count; j++)
            {
                player2 = GameManager.players[j].transform;
                float distance = Vector3.Distance(player1.position, player2.position);
                float proportionValue = (distance - minDistance) / (maxDistance - minDistance);
                interpolatedDistance = (proportionValue * (maxSize - minSize)) + minSize;
                if(interpolatedDistance < newMinDistance) newMinDistance = interpolatedDistance;
                if(interpolatedDistance > newMaxDistance) newMaxDistance = interpolatedDistance;
            }
        }
    }

    private void SetCameraValues()
    {
        // Set Camera Z position
        if(useZDistance) 
        {
            float targetZoomSize = Mathf.Clamp(-(maxXPlayer - minXPlayer), minZDistance, maxZDistance);
            clampedPosition = new(clampedPosition.x, clampedPosition.y, targetZoomSize + zOffset);
        }

        // Set Camera FOV values
        targetSize = Mathf.Clamp(interpolatedDistance, minSize, maxSize);
        if(cam.orthographic) cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        else if (!useZDistance) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetSize, zoomSpeed * Time.deltaTime);

        // Move Camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}