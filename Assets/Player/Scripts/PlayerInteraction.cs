using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public UIManagerScript uiManager;
    public PlayerAnimation playerAnimation;

    private const float MaxQte = 10f;
    private float _qte = 5f;
    
    private enum State
    {
        Idle,
        Attacked,
    }

    private State _state = State.Idle;

    private void Update()
    {
        if (_state == State.Attacked)
        {

            if (_qte > MaxQte)
            {
                AttackedtoIdle();
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetQte(_qte + 1f);
            }
        }
    }

    private void SetQte(float value)
    {
        _qte = value;
        uiManager.SetQteValue(value / MaxQte);
    }
    
    public void IdleToAttacked()
    {
        _state = State.Attacked;
        SetQte(5f);
        uiManager.SetQteSlider(true);
        playerAnimation.Attacked(true);
    }

    private void AttackedtoIdle()
    {
        _state = State.Idle;
        uiManager.SetQteSlider(false);
        playerAnimation.Attacked(true);
    }

    public bool isAttacked()
    {
        return _state == State.Attacked;
    }
}
