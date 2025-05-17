using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorMovement : MonoBehaviour
{

    private Animator animator;
    private NavMeshAgent agent;

    // 플레이어에게 보일경우 멈추는 코드 추가
    private bool isSeenByPlayer = false; // 현재 보여지고 있나
    private Vector3 savedDestination;    // Nav Mesh 목적지 저장
    public bool isShy = false;           // 플레이어가 처다볼때 멈추면 shy 
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
        }
        
        // * 수정: Idle 때의 Action Type가 Humanoid가 아니어서 발생했던 문제로 보입니다. 
        // 그래서 의사 정지 해결을 위한 코드는 일단 주석처리 해놨습니다.
        /*
            // 의사 정지시 이상한 포즈 해결 위한 코드 추가
            animator.speed = 1f;
            // 여기까지 추가함
            Vector3 AgentVelocity = agent.velocity;
        }
        else // 정지 상태
        {
            // 의사 정지시 이상한 포즈 해결 위한 코드 변경
            if (!animator.GetBool("GrabPlayer")) // 그랩상태가 아니면 애니메이션 정지
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                animator.speed = 0f;
            }
            else // 그랩상태면 애니메이션 계속
            {
                animator.speed = 1f;
            }
            // 여기까지 변경함
        }
        */
        
        // // 플레이어에게 보일경우 멈추는 코드 추가
        // if (isShy)
        // {
        //     if (isSeenByPlayer)
        //     {
        //         // 플레이어에게 보여지고 있으면 움직이지 않음
        //         agent.isStopped = true;
        //         savedDestination = agent.destination;
        //         Debug.Log("doctor is being Seen by Player");
        //         return;
        //     }
        //
        //     if (!isSeenByPlayer)
        //     {
        //         // 플레이어에게 보여지고 있지않으면 마지막 목적지로 이동함
        //         agent.isStopped = false;
        //         agent.SetDestination(savedDestination);
        //         Debug.Log("doctor is not being Seen by Player");
        //         return;
        //     }
        // }
        // // 여기까지 추가함
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
        isSeenByPlayer = true;
    }

    public void OnNotSpottedByPlayer()
    {
        isSeenByPlayer = false;
    }
    // 여기까지 추가함
}
