using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float aimFOV;
    [SerializeField] private float normalFOV = 60f;
    public Vector3 originalAimOffset = new Vector3(0.5f, 0, 0);
    [HideInInspector] public Vector3 aimOffset = new Vector3(0.5f, 0, 0);
    private Vector3 originalPosition;
    private Transform playerTransform;

    public GameObject crossAir;

    public float bulletImpactForce = 1000f; // Force applied to the rigidbody on hit
    float currentFOV;
    public LayerMask layerMask;
    void Start()
    {
        playerTransform = transform;
        originalPosition = playerTransform.localPosition;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentFOV, Time.deltaTime * 20f);//
        }
    }

    public void StartAiming()
    {
        if (mainCamera != null)
        {
            currentFOV = aimFOV;
            aimOffset = originalAimOffset; // Adjust aim offset if needed
            // crossAir.SetActive(true);
        }
    }
    public void StopAiming()
    {
        if (mainCamera != null)
        {
            currentFOV = normalFOV;
            aimOffset = Vector3.zero; // Reset aim offset
            // crossAir.SetActive(false);
        }
    }

    public void FireBullet()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForceAtPosition(ray.direction * bulletImpactForce, hit.point,ForceMode.Impulse);
            }
            else
            {
                Debug.Log("Hit object has no Rigidbody");
            }
        }
        else
        {
            Debug.Log("Missed");
        }
    }
}
