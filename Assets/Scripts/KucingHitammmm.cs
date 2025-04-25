using UnityEngine;
using UnityEngine.AI;

public class KucingHitammmm : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float moveInterval = 5f;     // time between moves
    public float waitAtPoint = 3f;      // how long to pause at a point

    private Transform _transform;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _timer;
    private bool isWaiting;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _transform = transform;
        _timer = moveInterval;
        isWaiting = false;
    }

    void Update()
    {
        // Check if arrived at destination
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance && !_agent.hasPath)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                _timer = 0f;
            }

            _timer += Time.deltaTime;

            if (_timer >= waitAtPoint)
            {
                // Done waiting, go to next point
                Vector3 newPos = RandomNavSphere(_transform.position, wanderRadius, -1);
                _agent.SetDestination(newPos);
                _timer = 0f;
                isWaiting = false;
            }
        }

        // Animation logic
        bool isWalking = _agent.velocity.magnitude > 0.1f && !_agent.isStopped;
        _animator.SetBool("IsWalking", isWalking);
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        if (NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask))
        {
            return navHit.position;
        }

        return origin; // fallback
    }
}
