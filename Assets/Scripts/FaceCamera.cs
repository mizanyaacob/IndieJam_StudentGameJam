using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Make the canvas face the camera
            transform.forward = mainCamera.transform.forward;
        }
    }
}
