using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorStateGrabPlayer : NPCIDoctorState
{
    private NPCDoctorBehavior NPCDoctorBehavior;

    private NavMeshAgent agent;

    // NPCIDoctorState 인터페이스스 구현부
    public void stateInit(NPCDoctorBehavior npc)
    {
        NPCDoctorBehavior = npc;
        Animator animator = NPCDoctorBehavior.GetAnimator();
        agent = npc.GetComponent<NavMeshAgent>();
        agent.isStopped = true; // NPC가 플레이어를 잡고 있는 동안 이동을 멈춤
        agent.velocity = Vector3.zero; // 속도를 0으로 설정하여 이동을 멈춤
        agent.ResetPath(); // 경로를 초기화하여 NPC가 이동하지 않도록 함
        if (animator != null)
        {
            animator.SetBool("GrabPlayer", true); // 애니메이션 상태를 설정
        }
    }

    public void HandleGrabPlayer()
    {
        // 플레이어를 잡는 로직을 여기에 추가.
        GameObject Player = NPCDoctorBehavior.GetPlayer();
        if (Player != null)
        {
            PlayerController playerController = Player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetGrabbed();
            }
        }
    }

    public void stateContinue()
    {
        // 플레이어가 벗어낫는지 확인해야 할 듯하다.
        // NPC가 플레이어를 잡고 있는 동안 이동을 멈춤
        agent.isStopped = true;
        HandleGrabPlayer(); // 플레이어를 잡는 로직을 호출
        return;
    }

    public NPCIDoctorState canChangeState()
    {
        if (CanGrabPlayer())
        {
            return this; // 현재 상태를 유지
        }
        else
        {
            return new NPCDoctorStateFollowingPlayer(); // 플레이어를 추적하는 상태로 전환
        }
    }

    bool CanGrabPlayer()
    {
        return NPCDoctorBehavior.GetPlayer().GetComponent<PlayerController>().IsDoctorInTrigger(NPCDoctorBehavior.GetComponent<Collider>());
        // Transform playerTransform = NPCDoctorBehavior.GetPlayerTransform();
        // if (playerTransform != null)
        // {
        //     float distance = Vector2.Distance(NPCDoctor.transform.position, playerTransform.position);
        //     if (distance <= NPCDoctor.GrabDistance)
        //     {
        //         return true;
        //     }
        // }
        // return false;
    }
}