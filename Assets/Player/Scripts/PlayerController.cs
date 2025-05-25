using UnityEngine;
using UnityEngine.InputSystem;

// 이동 및 중앙 관리 스크립트입니다.
public class PlayerController : MonoBehaviour
{
    // 플레이어 시야 처리관련 새로 추가한 변수들 입니다.
    public float viewRange = 15f;
    public float viewAngle = 60f;
    public float sightRange = 65f;
    public LayerMask doctorMask; // ray가 의사한테만 맞는 레이어를 지정

    // Damp 관련 함수들에 쓰이는 값입니다.
    private float _sv = 0.0f;
    private const float St = 0.1f;

    // 각종 속도 관련 설정값입니다.
    public float moveSpeed;
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float mouseSpeed = 10f;
    
    // 내부 호출 매핑
    public Rigidbody rb;
    public PlayerInteraction playerInteraction;
    public PlayerAnimation playerAnimation;
    
    // 마우스 기반 시점 전환에 쓰이는 변수입니다.
    private float _rotx;
    private float _roty;
    // 각종 시점 전환에 필요한 플레이어의 눈입니다.
    public GameObject eye;
    // 공격당할 시 바라볼 의사 NPC입니다.
    private GameObject _doctor;
    // Unity의 입력 시스템의 설정을 기반으로 입력 정보를 전달합니다.
    private InputAction _moveAction;
    // 입력 정보를 담을 벡터입니다.
    private Vector3 _movement;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _moveAction = InputSystem.actions.FindAction("Move");
    }
    
    private void Update()
    {
        // 새로 추가한 시야 처리용 함수입니다.
        CheckDoctorsInView();

        if (playerInteraction.isAttacked())
        {
            LookAtDoctor();
            return;
        }
        
        moveSpeed = Mathf.SmoothDamp(moveSpeed, Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed, ref _sv, St);
        _movement = _moveAction.ReadValue<Vector3>() * moveSpeed;
        _rotx = Input.GetAxis("Mouse X") * mouseSpeed;
        _roty = Input.GetAxis("Mouse Y") * mouseSpeed;
    }
    
    private void FixedUpdate()
    {
        if (playerInteraction.isAttacked()) return;
        
        playerAnimation.Moving(_movement);
        _movement = rb.transform.TransformDirection(_movement);
        rb.MovePosition(rb.position + _movement * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, _rotx, 0));
        eye.transform.localRotation = Quaternion.Euler(SightProcess(), 0, 0);
    }
    
    public bool IsDoctorInTrigger(Collider collider)
    {
        // name != "npc" 를 tag != "Doctor"로 변경
        if (collider.gameObject.tag != "Doctor") return false;
        var dist = Vector3.Distance(this.transform.position, collider.gameObject.transform.position);
        var npc = collider.gameObject.GetComponent<NPCDoctorBehavior>();
        return dist <= npc.GrabDistance;
    }

    public void SetGrabbed(GameObject obj)
    {
        _doctor = obj;
        playerInteraction.IdleToAttacked();
    }
    
    public void Cleared()
    {
        playerAnimation.Cleared();
    }
    
    // 시야에 의사가 들어왔는지 처리하는 함수입니다.
    private void CheckDoctorsInView()
    {
        Collider[] doctorsInRange = Physics.OverlapSphere(transform.position, viewRange, doctorMask);
        foreach (Collider doctorCollider in doctorsInRange)
        {
            NPCDoctorMovement doctor = doctorCollider.GetComponent<NPCDoctorMovement>();
            if (doctor == null) continue;

            Vector3 dirToDoctor = (doctor.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToDoctor);

            if (angle < viewAngle / 2f)
            {
                // 레이캐스트로 시야가 차단되어 있지않나 체크
                Ray ray = new Ray(transform.position + Vector3.up, dirToDoctor);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, viewRange))
                {
                    if (hit.transform == doctorCollider.transform)
                    {
                        // 의사를 발견했다면 의사에게 「님 발견됨」 이라고 전달
                        doctor.OnSpottedByPlayer();
                        continue;
                    }
                }
            }

            // 의사가 보이지 않으면 의사에게 「님 않보임」 이라고 전달
            doctor.OnNotSpottedByPlayer();
        }
    }
    
    private void LookAtDoctor()
    {
        var angle = Quaternion.LookRotation(_doctor.transform.position - transform.position).normalized;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, angle.eulerAngles.y, 0), 1f);
        var eyeAngle = Quaternion.LookRotation(_doctor.GetComponent<NPCDoctorBehavior>().GetHeadPosition() - eye.transform.position).normalized;
        eye.transform.localRotation = Quaternion.Lerp(eye.transform.localRotation, Quaternion.Euler(eyeAngle.eulerAngles.x, 25, 0), 1f);
    }
    private float SightProcess()
    {
        var x = eye.transform.localRotation.eulerAngles.x - _roty;
        if (x < 180 && x > sightRange) x = sightRange;
        else if (x > 180 && x < 360 - sightRange) x = 360 - sightRange;
        return x;
    }
}
