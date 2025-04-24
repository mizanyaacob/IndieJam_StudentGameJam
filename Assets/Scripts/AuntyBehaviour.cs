using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AuntyBehavior : MonoBehaviour
{
    public event Action OnAuntyDespawn;

    [SerializeField] private float activeTime = 10f;
    [SerializeField] private float slapRange = 2f;
    [SerializeField] private float slapCooldown = 2f;

    private Animator animator;
    private NavMeshAgent agent;
    private float lastSlapTime;

    [SerializeField] private GameObject targetPlayer;
    private HashSet<GameObject> slappedPlayers = new HashSet<GameObject>();

    private AuntyStick stick;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stick = GetComponentInChildren<AuntyStick>();

        targetPlayer = FindClosestPlayer();

        if (targetPlayer == null)
        {
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            animator.SetFloat("Speed", 4f);
        }

        StartCoroutine(DespawnAfterTime());
    }

    private bool isInCooldown = false;

    private void Update()
    {
        if (isInCooldown || targetPlayer == null) return;

        agent.SetDestination(targetPlayer.transform.position);

        float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
        if (distance <= slapRange && Time.time - lastSlapTime > slapCooldown)
        {
            lastSlapTime = Time.time;
            stick.SetTarget(targetPlayer); // Pass target to stick
            animator.SetTrigger("Attack");   // Animation event will call SlapCurrentTarget()

            slappedPlayers.Add(targetPlayer);

            StartCoroutine(DelayBeforeNextPlayer());
        }
    }


    private IEnumerator DelayBeforeNextPlayer()
    {
        isInCooldown = true;

        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);

        yield return new WaitForSeconds(2f); // wait time after slap

        targetPlayer = FindClosestPlayer();

        if (targetPlayer != null)
        {
            agent.isStopped = false;
            animator.SetFloat("Speed", 4f);
        }

        isInCooldown = false;
    }


    private GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject player in players)
        {
            if (slappedPlayers.Contains(player)) continue;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player;
            }
        }

        return closest;
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(activeTime);
        OnAuntyDespawn?.Invoke();
        Destroy(gameObject);
    }

    public void OnSlapAnimationEnd()
    {
        // This method can be called via animation event
        stick.SlapCurrentTarget();
    }
}

