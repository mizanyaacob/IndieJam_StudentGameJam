using System.Collections;
using UnityEngine;

public class Cameraman : MonoBehaviour
{
    [Header("Photo Settings")]
    public float photoDelay = 1f;
    public float photoRange = 10f;
    public float coneAngle = 45f;
    public float stayDuration = 5f;
    public float dizzyDuration = 3f;

    [Header("Rotation Settings")]
    public float rotateSpeed = 20f;
    public float rotateRange = 30f;

    [Header("Effects")]
    public GameObject flashEffect;
    public VisionCone visionCone; // Reference to VisionCone script

    private Quaternion initialRotation;
    private MeshRenderer coneRenderer;

    public Material visionMat;
    public Material before;
    public Material after;

    public Transform grounChecker;
    public GameObject Cone;

    void Start()
    {
        Cone = GameObject.Find("VisionCone");
        StartCoroutine(WaitForGroundedAndStartRoutine());

        coneRenderer = Cone.gameObject.GetComponent<MeshRenderer>();
        coneRenderer.material = before;
    }

    private IEnumerator WaitForGroundedAndStartRoutine()
    {
        // Wait until grounded
        while (!IsGrounded())
        {
            yield return null;
        }

        // Once grounded, initialize and start behavior
        initialRotation = transform.rotation;

        if (visionCone != null)
        {
            visionCone.angle = coneAngle;
            visionCone.range = photoRange;
        }

        StartCoroutine(PhotoBehaviorRoutine());
    }

    private bool IsGrounded()
    {
        // Cast ray down to check if grounded (adjust 0.3f based on your model size)
        return Physics.Raycast(grounChecker.position, Vector3.down, 0.3f);
    }

    private IEnumerator PhotoBehaviorRoutine()
    {
        float timer = 0f;
        float rotationTimer = 0f;

        while (timer < photoDelay)
        {
            timer += Time.deltaTime;
            rotationTimer += Time.deltaTime;

            float yRotation = Mathf.Sin(rotationTimer * rotateSpeed) * rotateRange;
            transform.rotation = Quaternion.Euler(0f, initialRotation.eulerAngles.y + yRotation, 0f);

            coneRenderer.material = before;
            yield return null;
        }

        // === Take the photo ===
        coneRenderer.material = after;

        if (flashEffect != null)
        {
            Vector3 flashPos = transform.position + transform.forward * 1f + Vector3.up * 1.5f;
            Instantiate(flashEffect, flashPos, Quaternion.identity);
        }

        // Detect players in vision cone
        Vector3 photoDirection = transform.forward;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            float angle = Vector3.Angle(photoDirection, toPlayer);

            if (angle <= coneAngle && toPlayer.magnitude <= photoRange)
            {
                Debug.Log($"😵 {player.name} got caught in the photo!");

                KnockbackReceiver dizzy = player.GetComponent<KnockbackReceiver>();
                if (dizzy != null)
                    dizzy.ForceDizzy(dizzyDuration);
            }
        }

        yield return new WaitForSeconds(stayDuration);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.25f); // Yellowish cone

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // Draw the vision cone using rays
        int segments = 30;
        float angleStep = coneAngle / segments;

        for (int i = -segments; i <= segments; i++)
        {
            float currentAngle = angleStep * i;
            Quaternion rot = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Vector3 dir = rot * forward;

            Gizmos.DrawRay(origin, dir.normalized * photoRange);
        }

        // Draw arc only in editor
#if UNITY_EDITOR
    UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.5f);
    UnityEditor.Handles.DrawSolidArc(origin, Vector3.up, Quaternion.Euler(0, -coneAngle, 0) * forward, 2 * coneAngle, photoRange);
#endif
    }


}
