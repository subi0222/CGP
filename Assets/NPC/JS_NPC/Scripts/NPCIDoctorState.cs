using UnityEngine;

public interface NPCIDoctorState
{
    void stateInit(NPCDoctorBehavior npc);

    void stateContinue();

    NPCIDoctorState canChangeState();
}
