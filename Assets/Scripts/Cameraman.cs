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
    private float rotationTimer;

    void Start()
    {
        initialRotation = transform.rotation;

        if (visionCone != null)
        {
            visionCone.angle = coneAngle;
            visionCone.range = photoRange;
        }

        StartCoroutine(PhotoBehaviorRoutine());
    }

    private IEnumerator PhotoBehaviorRoutine()
    {
        // Rotate left-right like scanning
        float timer = 0f;
        while (timer < photoDelay)
        {
            timer += Time.deltaTime;

            float yRotation = Mathf.Sin(Time.time * rotateSpeed) * rotateRange;
            transform.rotation = initialRotation * Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }

        // Flash
        if (flashEffect != null)
            Instantiate(flashEffect, transform.position + transform.forward * 1f, Quaternion.identity);

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
}
