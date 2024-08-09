using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of camera movement
    public float zoomSpeed = 5f; // Speed of zooming in/out
    public float minZoom = 5f;   // Minimum zoom level (orthographic size)
    public float maxZoom = 20f;  // Maximum zoom level (orthographic size)

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0) * panSpeed * Time.deltaTime;

        transform.position = newPosition;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        float newSize = Camera.main.orthographicSize - scroll * zoomSpeed;

        newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

        Camera.main.orthographicSize = newSize;
    }
}
