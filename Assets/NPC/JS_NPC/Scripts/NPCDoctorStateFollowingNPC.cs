using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorStateFollowingNPC : NPCIDoctorState
{
    private NavMeshAgent agent;
    private NPCDoctorBehavior TargetNPCDoctor;

    private NPCDoctorBehavior NPCDoctor;

    // NPCIDoctorState 인터페이스스 구현부
    public void stateInit(NPCDoctorBehavior npc)
    {
        NPCDoctor = npc;
        foreach (NPCDoctorBehavior Doctor in NPCDoctorBehavior.AllDoctors)
        {
            if (Doctor.GetCurrentState() is NPCDoctorStateFollowingPlayer or NPCDoctorStateGrabPlayer)
            {
                TargetNPCDoctor = Doctor;
                break;
            }
        }
        agent = npc.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        if (TargetNPCDoctor != null)
        {
            agent.SetDestination(TargetNPCDoctor.transform.position);
        }
    }

    public void stateContinue()
    {
        if (!agent.pathPending && agent.remainingDistance >= 0.1f)
        {
            agent.SetDestination(TargetNPCDoctor.transform.position);
        }
    }

    public NPCIDoctorState canChangeState()
    {
        if (CanDetectPlayer())
        {
            return new NPCDoctorStateFollowingPlayer();
        }
        if (CheckTargetNPCGetLost())
        {
            return new NPCDoctorStatePatrol();
        }
        return this;
    }

    private bool CanDetectPlayer()
    {
        Transform playerTransform = NPCDoctorBehavior.GetPlayerTransform();
        if (playerTransform == null)
            return false;
        else
        {
            Vector2 directionToPlayer = playerTransform.position - NPCDoctor.transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            // Check if the player is within detection distance and angle
            if (distanceToPlayer <= NPCDoctor.DetectionDistance)
            {
                float angleToPlayer = Vector2.Angle(NPCDoctor.transform.forward, directionToPlayer.normalized);
                if (angleToPlayer <= NPCDoctor.DetectionAngle / 2f)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private bool CheckTargetNPCGetLost()
    {
        if (TargetNPCDoctor != null)
        {
            if (TargetNPCDoctor.GetCurrentState() is NPCDoctorStateFollowingPlayer)
            {
                return false; // Target NPC is lost
            }
            else if (TargetNPCDoctor.GetCurrentState() is NPCDoctorStateGrabPlayer)
            {
                return false; // Target NPC is not lost
            }
        }
        return true;
    }
}
