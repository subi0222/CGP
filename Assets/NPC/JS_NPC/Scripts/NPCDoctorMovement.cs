using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorMovement : MonoBehaviour
{

    private Animator animator;
    private NavMeshAgent agent;

    // 플레이어에게 보일경우 멈추는 코드 추가
    private bool hasBeenSpotted = false; // 한 번이라도 플레이어에게 보였나
    private bool isSeenByPlayer = false; // 현재 보여지고 있나
    private Vector3 savedDestination;    // Nav Mesh 목적지 저장
    // 여기까지 추가함

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = agent.desiredVelocity;

        if (velocity.sqrMagnitude > 0.01f) // 움직이고 있는 경우
        {
            Vector3 localDir = transform.InverseTransformDirection(velocity.normalized);
            animator.SetFloat("X", localDir.x, 0.1f, Time.deltaTime);
            animator.SetFloat("Y", localDir.z, 0.1f, Time.deltaTime);
            Vector3 AgentVelocity = agent.velocity;
        }
        else // 정지 상태
        {
            animator.SetFloat("X", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Y", 0f, 0.1f, Time.deltaTime);
        }

        // 플레이어에게 보일경우 멈추는 코드 추가
        if (!hasBeenSpotted)
        {
            // 플레이어에게 보여진적 없으면 움직이지 않음
            agent.isStopped = true;
            savedDestination = agent.destination;
            Debug.Log("doctor has not Been Spotted");
            return;
        }

        if (isSeenByPlayer)
        {
            // 플레이어에게 보여지고 있으면 움직이지 않음
            agent.isStopped = true;
            savedDestination = agent.destination;
            Debug.Log("doctor is being Seen by Player");
            return;
        }

        if (!isSeenByPlayer)
        {
            // 플레이어에게 보여지고 있지않으면 마지막 목적지로 이동함
            agent.isStopped = false;
            agent.SetDestination(savedDestination);
            Debug.Log("doctor is not being Seen by Player");
            return;
        }
        // 여기까지 추가함
    }

    void LateUpdate()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            transform.position = agent.nextPosition;
        }
    }

    // 플레이어에게 보일경우 멈추는 코드 추가
    public void OnSpottedByPlayer()
    {
        if (!hasBeenSpotted)
        {
            hasBeenSpotted = true;
            Debug.Log("doctor has Been Spotted");
        }
        isSeenByPlayer = true;
    }

    public void OnNotSpottedByPlayer()
    {
        isSeenByPlayer = false;
    }
    // 여기까지 추가함
}
