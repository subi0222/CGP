using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorMovement : MonoBehaviour
{

    private Animator animator;
    private NavMeshAgent agent;
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
    }

    void LateUpdate()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            transform.position = agent.nextPosition;
        }
    }
}
