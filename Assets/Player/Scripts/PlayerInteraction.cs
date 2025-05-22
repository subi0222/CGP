using UnityEngine;

// 의사 NPC와의 상호작용 관련 스크립트입니다.
public class PlayerInteraction : MonoBehaviour
{
    public UIManager uiManager;
    public PlayerAnimation playerAnimation;

    // 상호작용 게이지의 최대 양
    private const float MaxQte = 10f;
    // 상호작용 게이지의 현재 양
    private float _qte = 5f;
    // Attacked 상태 탈출 직후 보장되는 시간
    private float _safeTimer = 3f;
    // Difficulty보다 높아지면 게이지를 1씩 깎습니다.
    private float _attackTimer = 0f;
    // _attackTimer가 이를 초과하면 게이지를 1씩 깎습니다.
    public float difficulty = 1f;
    // 평상시, 공격 받았을 때를 나누기 위한 State입니다.
    private enum State
    {
        Idle,
        Attacked,
    }
    // 기본 State 설정
    private State _state = State.Idle;

    private void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
    
    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                _safeTimer += Time.deltaTime;
                break;
            case State.Attacked:
            {
                if (_qte > MaxQte) AttackedtoIdle();
                if (Input.GetKeyDown(KeyCode.Space)) SetQte(_qte + 1f);
                _attackTimer += Time.deltaTime;
                if (_attackTimer >= (1 / difficulty))
                {
                    SetQte(_qte - 1f);
                    _attackTimer = 0f;
                }
                if (_qte <= 0) KillPlayer();
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
    
    public float GetQte()
    {
        return _qte;
    }
}
