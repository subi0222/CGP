using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorStatePatrol : NPCIDoctorState
{

    private NavMeshAgent agent;
    private NPCDoctorBehavior NPCDoctor;
    private int currentPatrolIndex = 0;
    private bool getHearWistle = false;
    Transform nowPatrolPoint;

    public void HearWhistleSound()
    {
        getHearWistle = true;
    }

    // NPCIDoctorState 인터페이스스 구현부
    public void stateInit(NPCDoctorBehavior npc)
    {
        NPCDoctor = npc;
        agent = npc.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;
        currentPatrolIndex = NPCDoctor.GetRandomPatriolPointIndex();
        nowPatrolPoint = NPCDoctor.GetPatrolPoint(currentPatrolIndex);
        if (nowPatrolPoint != null)
        {
            agent.SetDestination(nowPatrolPoint.position);
        }
        getHearWistle = false;
    }

    public void stateContinue()
    {
        if (agent.remainingDistance <= 2.5f)
        {
            currentPatrolIndex = NPCDoctor.GetNextPointIndex(currentPatrolIndex);
            nowPatrolPoint = NPCDoctor.GetNextPatrolPoint(currentPatrolIndex);
            if (nowPatrolPoint != null)
            {
                agent.SetDestination(nowPatrolPoint.position);
            }
        }
    }

    public NPCIDoctorState canChangeState()
    {
        if (CanDetectPlayer())
        {
            return new NPCDoctorStateFollowingPlayer();
        }
        if (getHearWistle)
        {
            return new NPCDoctorStateFollowingNPC();
        }
        return this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool CanDetectPlayer()
    {
        Transform playerTransform = NPCDoctorBehavior.GetPlayerTransform();
        if (playerTransform == null)
            return false;
        else
        {
            Vector3 directionToPlayer = playerTransform.position - NPCDoctor.transform.position;
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
}
