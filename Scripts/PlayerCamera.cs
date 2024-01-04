using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float sensX;
    [SerializeField] float sensY;
    [SerializeField] float yRotationClamp = 89f;
    [SerializeField] Transform orientation;
    [SerializeField] Transform cameraHolder;

    float xRotation;
    float yRotation;

    // Hide and lock the cursor on start
    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Always call the ManageCamera function
    private void Update()
    {
        ManageCamera();
    }

    // Set where the player look via their mouse movements
    private void ManageCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -yRotationClamp, yRotationClamp);

        cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // Changes the camera FOV
    public void ChangeFOV(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    // Tilts the camera
    public void TiltCamera(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
