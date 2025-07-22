using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float mouseSensitivity = 100f;
    public float pitchMin = -35f;
    public float pitchMax = 60f;

    private float yaw;
    private float pitch;

    public WeaponController weaponController;

    void Start()
    {
        yaw = target.eulerAngles.y;
        pitch = 15f; // Initial pitch to look slightly downward

        // Hide and lock the cursor at the start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Toggle cursor visibility and lock state with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void LateUpdate()
    {
        // Only rotate the camera if the cursor is locked
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }

        // Apply rotation to the camera
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Position the camera behind the target
        Vector3 desiredPosition = target.position - transform.forward * distance + Vector3.up * 1.2f + weaponController.aimOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 20f);
    }
}