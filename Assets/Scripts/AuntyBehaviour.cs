using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class AuntyBehavior : MonoBehaviour
{
    public event Action OnAuntyDespawn;

    [SerializeField] private float activeTime = 10f;
    [SerializeField] private float slapRange = 1.5f;
    [SerializeField] private float slapCooldown = 2f;

    private GameObject targetPlayer;
    private NavMeshAgent agent;
    private float lastSlapTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetPlayer = FindClosestPlayer();
        StartCoroutine(DespawnAfterTime());
    }

    private void Update()
    {
        if (targetPlayer)
        {
            agent.SetDestination(targetPlayer.transform.position);

            float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if (distance <= slapRange && Time.time - lastSlapTime > slapCooldown)
            {
                SlapPlayer();
                lastSlapTime = Time.time;
            }
        }
    }

    private GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player;
            }
        }

        return closest;
    }

    private void SlapPlayer()
    {
        Debug.Log($"{gameObject.name} slapped {targetPlayer.name}!");
        // Example punishment:
        PlayerFoodConsumer consumer = targetPlayer.GetComponent<PlayerFoodConsumer>();
        if (consumer != null)
        {
            consumer.SetIsConsuming(false);
            // Add stun or speed penalty here
        }
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(activeTime);
        OnAuntyDespawn?.Invoke();
        Destroy(gameObject);
    }
}
