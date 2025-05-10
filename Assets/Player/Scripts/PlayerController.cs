using UnityEngine;

//이동 및 중앙 관리 스크립트입니다.
public class PlayerController : MonoBehaviour
{
    // 플레이어 시야 처리관련 새로 추가한 변수들 입니다.
    public float viewRange = 15f;
    public float viewAngle = 60f;
    public LayerMask doctorMask; // ray가 의사한테만 맞는 레이어를 지정

    private float _sv = 0.0f;
    private const float St = 0.1f;

    public float moveSpeed;
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float mouseSpeed = 20f;
    public Rigidbody rb;
    public PlayerInteraction playerInteraction;
    public PlayerAnimation playerAnimation;
    public PlayerCameraBehaviourScript playerCamera;
    
    private Vector3 _movement;
    private Vector3 _rotation;
    
    private void Update()
    {
        
        // 새로 추가한 시야 처리용 함수입니다.
        CheckDoctorsInView();

        if (playerInteraction.isAttacked()) return;
        
        moveSpeed = Mathf.SmoothDamp(moveSpeed, Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed, ref _sv, St);

        _movement.x = Input.GetAxis("Horizontal") * moveSpeed;
        _movement.z = Input.GetAxis("Vertical") * moveSpeed;
        
        //_rotation.y = Input.GetAxis("Mouse X") * mouseSpeed;
    }
    
    private void FixedUpdate()
    {
        if (playerInteraction.isAttacked()) return;
        
        playerAnimation.Moving(_movement);
        _movement = rb.transform.TransformDirection(_movement);
        rb.MovePosition(rb.position + _movement * Time.fixedDeltaTime);
        //rb.MoveRotation(rb.rotation * Quaternion.Euler(_rotation));
    }
    
    public bool IsDoctorInTrigger(Collider collider)
    {
        // name != "npc" 를 tag != "Doctor"로 변경
        if (collider.gameObject.tag != "Doctor") return false;
        var dist = Vector2.Distance(this.transform.position, collider.gameObject.transform.position);
        var npc = collider.gameObject.GetComponent<NPCDoctorBehavior>();
        Debug.Log("Caught - Distance: " + dist);
        return dist <= npc.GrabDistance;
    }

    public void SetGrabbed(GameObject obj)
    {
        var doctor = obj.gameObject.GetComponent<NPCDoctorBehavior>();
        playerCamera.setDoctorPos(doctor.GetHeadPosition());
        playerInteraction.IdleToAttacked();
    }
    
    // 시야에 의사가 들어왔는지 처리하는 함수입니다.
    void CheckDoctorsInView()
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
}
