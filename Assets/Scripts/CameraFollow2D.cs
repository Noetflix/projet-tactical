using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Cible à suivre")]
    public Transform player;

    [Header("Paramètres de suivi")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Zoom")]
    public float zoomSize = 3f;

    [Header("Tilemap à utiliser comme limites")]
    public Tilemap tilemap;

    private Camera cam;
    private Vector3 minBounds;
    private Vector3 maxBounds;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = zoomSize;

        if (tilemap != null)
        {
            // On prend les bounds du tilemap
            Bounds localBounds = tilemap.localBounds;

            // Convertir en coordonnées monde
            minBounds = tilemap.transform.position + localBounds.min;
            maxBounds = tilemap.transform.position + localBounds.max;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Taille de la caméra en unités monde
            float camHeight = cam.orthographicSize;
            float camWidth = cam.orthographicSize * cam.aspect;

            // Clamp par rapport à la tilemap
            float clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y + camHeight, maxBounds.y - camHeight);

            transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
        }
    }
}
