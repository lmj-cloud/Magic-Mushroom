using UnityEngine;
using UnityEngine.AI; // 이거 무조건 상단에 추가해야 함!

public class Noum : MonoBehaviour
{
    public float pickupRange = 1.5f;
    public Vector3 holdOffset = new Vector3(0f, 1.1f, 0f);

    [Header("Flee")]
    public float fleeDistance = 8f;
    public float dangerRange = 7f;
    public float repathInterval = 0.4f;

    [Header("Wander")]
    public float wanderRadius = 10f;
    public float wanderWaitMin = 0.5f;
    public float wanderWaitMax = 2f;

    // 플레이어 정반대(0도)를 우선으로, 좌우로 벌려가며 검사할 후보 각도
    static readonly float[] fleeAngles = { 0f, 30f, -30f, 60f, -60f, 90f, -90f, 120f, -120f };

    NavMeshAgent agent;
    public Transform held;
    Transform player;
    NavMeshPath path;
    float repathTimer;
    float wanderTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.autoBraking = false;

        path = new NavMeshPath();

        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        if (held != null)
        {
            if (IsPlayerNear())
                FleeFromPlayer();
            else
                Wander();

            return;
        }

        Transform mushroom = FindClosestMushroom();
        if (mushroom == null)
        {
            Wander();
            return;
        }

        agent.SetDestination(mushroom.position);

        if (Vector3.Distance(transform.position, mushroom.position) <= pickupRange)
            PickUp(mushroom);
    }

    void PickUp(Transform item)
    {
        held = item;

        var rb = held.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        var col = held.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        held.SetParent(transform);
        held.localPosition = holdOffset;
        held.localRotation = Quaternion.identity;

        var heldAgent = held.GetComponent<NavMeshAgent>();
        if (heldAgent != null)
            heldAgent.enabled = false;

        agent.ResetPath();
        agent.isStopped = false;

        repathTimer = 0f;
        wanderTimer = 0f;
    }

    Transform FindClosestMushroom()
    {
        GameObject[] mushrooms = GameObject.FindGameObjectsWithTag("Pickup");
        Transform closest = null;
        float closestSqr = float.MaxValue;

        for (int i = 0; i < mushrooms.Length; i++)
        {
            Transform mushroom = mushrooms[i].transform;
            if (mushroom.parent != null)
                continue;

            float sqr = (mushroom.position - transform.position).sqrMagnitude;
            if (sqr < closestSqr)
            {
                closestSqr = sqr;
                closest = mushroom;
            }
        }

        return closest;
    }

    bool IsPlayerNear()
    {
        if (player == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
                return false;

            player = playerObject.transform;
        }

        return (transform.position - player.position).sqrMagnitude <= dangerRange * dangerRange;
    }

    void FleeFromPlayer()
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer > 0f && !HasArrived())
            return;

        repathTimer = repathInterval;

        if (TryGetFleeDestination(out Vector3 destination))
            agent.SetDestination(destination);
    }

    bool TryGetFleeDestination(out Vector3 result)
    {
        result = transform.position;

        Vector3 away = transform.position - player.position;
        away.y = 0f;
        if (away.sqrMagnitude < 0.01f)
            away = transform.forward;
        away.Normalize();

        float bestScore = float.NegativeInfinity;
        bool found = false;

        for (int i = 0; i < fleeAngles.Length; i++)
        {
            Vector3 direction = Quaternion.Euler(0f, fleeAngles[i], 0f) * away;
            Vector3 candidate = transform.position + direction * fleeDistance;

            // 탐색 반경을 좁게 잡아야 벽 너머 후보가 제 발밑으로 스냅되지 않고 걸러진다
            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, agent.areaMask))
                continue;

            if (!agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete)
                continue;

            float fromPlayer = Vector3.Distance(hit.position, player.position);
            float progress = Vector3.Distance(hit.position, transform.position);
            float score = fromPlayer + progress * 0.5f;

            if (score > bestScore)
            {
                bestScore = score;
                result = hit.position;
                found = true;
            }
        }

        return found;
    }

    void Wander()
    {
        if (!HasArrived())
            return;

        wanderTimer -= Time.deltaTime;
        if (wanderTimer > 0f)
            return;

        wanderTimer = Random.Range(wanderWaitMin, wanderWaitMax);
        agent.SetDestination(RandomPointAround(transform.position, wanderRadius));
    }

    Vector3 RandomPointAround(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(offset.x, 0f, offset.y);

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, agent.areaMask))
                continue;

            if (!agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete)
                continue;

            return hit.position;
        }

        return center;
    }

    bool HasArrived()
    {
        if (agent.pathPending)
            return false;

        if (!agent.hasPath)
            return true;

        return agent.remainingDistance <= agent.stoppingDistance + 0.5f;
    }
}
