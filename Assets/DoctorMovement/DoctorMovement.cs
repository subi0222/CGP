using UnityEngine;
using UnityEngine.AI;

public class DoctorMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public float viewRange = 20f;
    public float viewAngle = 90f;
    public LayerMask playerMask;

    private bool hasBeenSpotted = false; // 한 번이라도 플레이어에게 보였나
    private bool isSeenByPlayer = false; // 현재 보여지고 있나
    private Vector3 lastKnownPlayerPosition;

    // 다른 의사와 공유 정보
    public static Vector3 sharedPlayerPosition;
    public static bool playerDetectedByAnyDoctor = false;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (!hasBeenSpotted)
        {
            // 플레이어에게 보여진적 없으면 움직이지 않음
            agent.isStopped = true;
            return;
        }

        if (isSeenByPlayer)
        {
            // 플레이어에게 보여지고 있으면 움직이지 않음
            agent.isStopped = true;
            return;
        }

        // 플레이어가 시야안에 있을까 체크
        if (CanSeePlayer())
        {
            // 플레이어가 보인다면 추적 개시
            agent.isStopped = false;
            agent.SetDestination(player.position);

            lastKnownPlayerPosition = player.position;
            sharedPlayerPosition = player.position;
            playerDetectedByAnyDoctor = true;
        }
        else
        {
            // 플레이어가 보이지 않게 되면 마지막으로 보여진 장소에 이동
            agent.isStopped = false;
            agent.SetDestination(lastKnownPlayerPosition);
        }

        // 다른 의사가 플레이어를 발견하면 공유 정보를 사용
        if (!CanSeePlayer() && playerDetectedByAnyDoctor)
        {
            agent.isStopped = false;
            agent.SetDestination(sharedPlayerPosition);
        }
    }

    public void OnSpottedByPlayer()
    {
        if (!hasBeenSpotted)
        {
            hasBeenSpotted = true;
        }
        isSeenByPlayer = true;
    }

    public void OnNotSpottedByPlayer()
    {
        isSeenByPlayer = false;
    }

    private bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > viewRange)
            return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < viewAngle / 2f)
        {
            Ray ray = new Ray(transform.position + Vector3.up, dirToPlayer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, viewRange, playerMask))
            {
                if (hit.transform == player)
                {
                    return true;
                }
            }
        }
        return false;
    }
}