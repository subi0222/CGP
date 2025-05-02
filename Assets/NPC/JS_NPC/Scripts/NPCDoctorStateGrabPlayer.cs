using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorStateGrabPlayer : NPCIDoctorState
{
    private NPCDoctorBehavior NPCDoctor;

    private NavMeshAgent agent;

    // NPCIDoctorState 인터페이스스 구현부
    public void stateInit(NPCDoctorBehavior npc)
    {
        NPCDoctor = npc;
        Animator animator = NPCDoctor.GetAnimator();
        agent = npc.GetComponent<NavMeshAgent>();
        if (animator != null)
        {
            animator.Play("GrabPlayer");
        }
    }

    public void HandleGrabPlayer()
    {
        // 플레이어를 잡는 로직을 여기에 추가.
        GameObject Player = NPCDoctorBehavior.GetPlayer();
        if (Player != null)
        {
        }
    }

    public void stateContinue()
    {
        // 플레이어가 벗어낫는지 확인해야 할 듯하다.
        agent.isStopped = true; // NPC가 플레이어를 잡고 있는 동안 이동을 멈춤
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
            agent.isStopped = false; // NPC가 플레이어를 잡고 있지 않으면 이동을 재개
            return new NPCDoctorStateFollowingPlayer(); // 플레이어를 추적하는 상태로 전환
        }
    }

    bool CanGrabPlayer()
    {
        Transform playerTransform = NPCDoctorBehavior.GetPlayerTransform();
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(NPCDoctor.transform.position, playerTransform.position);
            if (distance <= NPCDoctor.GrabDistance)
            {
                Debug.Log("Grab Player");
                return true;
            }
        }
        return false;
    }
}