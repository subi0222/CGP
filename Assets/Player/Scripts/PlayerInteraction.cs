using UnityEngine;

//의사 NPC와의 상호작용 관련 스크립트입니다.
public class PlayerInteraction : MonoBehaviour
{
    public UIManagerScript uiManager;
    public PlayerAnimation playerAnimation;

    public float difficulty = 1f;

    private const float MaxQte = 10f;
    private float _qte = 5f;
    private float _safeTimer = 3f;
    private float _attackTimer = 0f;
    
    private enum State
    {
        Idle,
        Attacked,
    }

    private State _state = State.Idle;

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                _safeTimer += Time.deltaTime;
                break;
            case State.Attacked:
            {
                if (_qte > MaxQte)
                {
                    AttackedtoIdle();
                }
            
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SetQte(_qte + 1f);
                }
                
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= difficulty)
                {
                    SetQte(_qte - 1f);
                    _attackTimer = 0f;
                }

                if (_qte <= 0)
                {
                    KillPlayer();
                }

                break;
            }
        }
    }

    private void KillPlayer()
    {
        uiManager.SetQteUI(false);
        uiManager.SetDeathUI(true);
    }

    private void SetQte(float value)
    {
        _qte = value;
        uiManager.SetQteValue(value / MaxQte);
        Debug.Log("qte: " + _qte);
    }
    
    public void IdleToAttacked()
    {
        if (_state == State.Attacked || _safeTimer < 3f) return;
        _state = State.Attacked;
        SetQte(5f);
        uiManager.SetQteUI(true);
        playerAnimation.Attacked(true);
    }

    private void AttackedtoIdle()
    {
        if (_state == State.Idle) return;
        _state = State.Idle;
        uiManager.SetQteUI(false);
        playerAnimation.Attacked(true);
        _safeTimer = 0f;
    }

    public bool isAttacked()
    {
        return _state == State.Attacked;
    }
}
