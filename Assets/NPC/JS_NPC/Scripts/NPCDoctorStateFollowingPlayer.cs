using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCDoctorStateFollowingPlayer : NPCIDoctorState
{
    private NavMeshAgent agent;
    private NPCDoctorBehavior NPCDoctor;

    private AudioSource WhistleSound;

    // NPCIDoctorState 인터페이스스 구현부
    public void stateInit(NPCDoctorBehavior npc)
    {
        NPCDoctor = npc;
        // Initialize the state when it is first entered
        agent = NPCDoctor.GetComponent<NavMeshAgent>();
        WhistleSound = NPCDoctor.GetComponent<AudioSource>();
        agent.isStopped = false;
        agent.updatePosition = true; // Enable position updates
        HandleWhistleSound(); // Handle the whistle sound when entering this state
    }

    public void stateContinue()
    {
        // Continue the state logic, such as moving towards the player
        Transform playerTransform = NPCDoctorBehavior.GetPlayerTransform();
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    public NPCIDoctorState canChangeState()
    {
        if (CanGrabPlayer())
        {
            // If the player is within grab distance, return the grab state
            return new NPCDoctorStateGrabPlayer(); // Assuming you have a state for grabbing the player
        }

        else if (!CanDetectPlayer())
        {
            // If the player is no longer detected, return to patrol state
            return new NPCDoctorStatePatrol(); // Assuming you have a patrol state
        }

        return this;
    }


    public void HandleWhistleSound()
    {
        // Play the whistle sound when the state is initialized
        if (WhistleSound != null && NPCDoctor.WistleSoundClip != null)
        {
            WhistleSound.clip = NPCDoctor.WistleSoundClip;
            WhistleSound.Play();
            List<NPCDoctorBehavior> allDoctors = NPCDoctorBehavior.AllDoctors;
            foreach (NPCDoctorBehavior doctor in allDoctors)
            {
                if (doctor != NPCDoctor)
                {
                    if (doctor.GetCurrentState() is NPCDoctorStatePatrol)
                    {
                        double distance = Vector2.Distance(doctor.transform.position, NPCDoctor.transform.position);
                        if (distance <= NPCDoctor.HearingDistance)
                        {
                            NPCDoctorStatePatrol OtherDoctorState = doctor.GetCurrentState() as NPCDoctorStatePatrol;
                            OtherDoctorState.HearWhistleSound();
                        }
                    }
                }
            }
        }
    }

    private bool CanGrabPlayer()
    {
        Transform playerTransform = NPCDoctorBehavior.GetPlayerTransform();
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(NPCDoctor.transform.position, playerTransform.position);
            if (distanceToPlayer <= NPCDoctor.GrabDistance)
            {
                // Debug line to visualize the grab distance
                Debug.DrawLine(NPCDoctor.transform.position, playerTransform.position, Color.red, 1f);
                return true; // Player is within grab distance
            }
        }
        return false;
    }

    private bool CanDetectPlayer()
    {
        Transform playerTransform = NPCDoctorBehavior.GetPlayerTransform();
        if (playerTransform == null)
            return false;
        else
        {
            Vector3 directionToPlayer = playerTransform.position - NPCDoctor.transform.position;
            float angle = Vector2.Angle(NPCDoctor.transform.forward, directionToPlayer);
            return angle <= NPCDoctor.DetectionAngle / 2f && directionToPlayer.magnitude <= NPCDoctor.DetectionDistance;
        }
    }
}
