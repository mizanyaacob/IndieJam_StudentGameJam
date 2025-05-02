using UnityEngine;
using StarterAssets;

public class KuihProperties : MonoBehaviour
{
    public enum KuihState { Good, Bad, Explode }

    [Header("Kuih Settings")]
    public string kuihName = "Kuih";
    public int scoreValue = 1;
    public float consumeTime = 1f;

    [Header("Kuih State Settings")]
    public float goodDuration = 5f;
    public float badDuration = 5f;

    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 5f;
    public float knockbackDuration = 0.5f;
    public GameObject explosionEffect;
    public GameObject badFoodEffect;
    public GameObject HighlightEffect; // Prefab for bad food effect


    private KuihState currentState = KuihState.Good;
    private float stateTimer;

    private ImpulseTrigger impulseTrigger;
    private GameObject badFoodEffectInstance;
    public KuihState CurrentState => currentState;

    private AudioManager audioManager;

    private void Awake()
    {
        impulseTrigger = FindAnyObjectByType<ImpulseTrigger>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    void Start()
    {
        currentState = KuihState.Good;
        stateTimer = goodDuration;
        GameObject highlight = Instantiate(HighlightEffect, transform.position, Quaternion.identity);
        highlight.transform.SetParent(transform);
        highlight.transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case KuihState.Good:
                if (stateTimer <= 0f)
                    SwitchState(KuihState.Bad, badDuration);
                break;

            case KuihState.Bad:
                if (stateTimer <= 0f)
                    SwitchState(KuihState.Explode, 0f);
                break;

            case KuihState.Explode:
                Explode();
                break;
        }
    }

    private void SwitchState(KuihState newState, float duration)
    {
        currentState = newState;
        stateTimer = duration;

        Debug.Log($"{kuihName} switched to {newState} state!");

        if (newState == KuihState.Bad)
        {
            GetComponent<Renderer>().material.color = Color.red;
            badFoodEffectInstance = Instantiate(badFoodEffect, transform.position, Quaternion.identity);
            badFoodEffectInstance.transform.SetParent(transform);
        }
    }

    private void Explode()
    {
        Debug.Log($"{kuihName} exploded!");

        // Knockback effect on players within range
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            KnockbackReceiver receiver = hit.GetComponent<KnockbackReceiver>();
            if (receiver != null)
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                direction.y = 1.5f; // Keep Y direction zero for horizontal knockback
                receiver.ApplyKnockback(direction, explosionForce, knockbackDuration, false);
            }
        }

        // Optional: add explosion effect
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);
        impulseTrigger.TriggerImpulse();
        Destroy(badFoodEffectInstance);
        audioManager.PlaySFX(audioManager.Explosion);
        GameObject busuk = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        busuk.transform.SetParent(transform);
        busuk.transform.localPosition= Vector3.zero;
        Vector3 pos = busuk.transform.position;
        pos.y = 1f;
        busuk.transform.position = pos;
        Destroy(gameObject);
    }

    // Optional: visualize explosion range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
