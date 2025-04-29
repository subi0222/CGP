using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Animator 관련 변수들입니다.
    private static readonly int Xspeed = Animator.StringToHash("Xspeed");
    private static readonly int Yspeed = Animator.StringToHash("Yspeed");
    private float _sv = 0.0f;
    private const float St = 0.1f;
    private Vector3 _v = Vector3.zero;

    public float moveSpeed;
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float mouseSpeed = 20f;
    public Rigidbody rb;
    public Animator anim;
    
    private Vector3 _movement;
    private Vector3 _rotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        moveSpeed = Mathf.SmoothDamp(moveSpeed, Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed, ref _sv, St);

        _movement.x = Input.GetAxis("Horizontal") * moveSpeed;
        _movement.z = Input.GetAxis("Vertical") * moveSpeed;
        
        _rotation.y = Input.GetAxis("Mouse X") * mouseSpeed;
    }

    private void FixedUpdate()
    {
        SetAnim();
        _movement = rb.transform.TransformDirection(_movement);
        rb.MovePosition(rb.position + _movement * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(_rotation));
    }

    // Animator에서 사용되는 Xspeed, Yspeed 파라미터를 업데이트합니다.
    // 애니메이션 연결을 부드럽게 하기 위해서 SmoothDamp로 완충합니다.
    private void SetAnim()
    {
        var getParam = new Vector3(anim.GetFloat(Xspeed), 0, anim.GetFloat(Yspeed));
        var dampedParam = Vector3.SmoothDamp(getParam, _movement, ref _v, St);
        anim.SetFloat(Xspeed, dampedParam.x);
        anim.SetFloat(Yspeed, dampedParam.z);
    }
}
