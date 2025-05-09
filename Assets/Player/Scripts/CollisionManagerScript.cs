using UnityEngine;

public class CollisionManagerScript : MonoBehaviour
{
    public PlayerInteraction playerInteraction;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Doctor"))
        {
            playerInteraction.IdleToAttacked();
        }
    }
}
