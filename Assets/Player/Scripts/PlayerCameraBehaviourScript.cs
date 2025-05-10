using UnityEngine;

//카메라 관련 스크립트들입니다.
public class PlayerCameraBehaviourScript : MonoBehaviour
{
    public GameObject player;
    public Rigidbody rb;
    public PlayerInteraction playerInteraction;
    public float mouseSpeed = 20f;
    public float maxDegree = 275f;
    public float minDegree = 50f;
    
    private Vector3 _rotation;
    private Vector3 _doctorPos;
    
    private void Update()
    {
        if (playerInteraction.isAttacked())
        {
            LookAtDoctor();
            return;
        }
        _rotation.x = Input.GetAxis("Mouse Y") * -mouseSpeed;
        _rotation.y = Input.GetAxis("Mouse X") * mouseSpeed;
        this.transform.localRotation = Quaternion.Euler(this.transform.localRotation.eulerAngles.x, 0, 0);
    }

    private void FixedUpdate()
    {
        if (playerInteraction.isAttacked() || !CheckAngleValidity()) return;
        
        this.transform.Rotate(_rotation.x, 0, 0);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, _rotation.y, 0));
    }

    public void setDoctorPos(Vector3 pos)
    {
        _doctorPos = pos;
    }

    private void LookAtDoctor()
    {
        var angle = Quaternion.LookRotation(_doctorPos - transform.position).normalized;
        player.transform.rotation = Quaternion.Euler(0, angle.eulerAngles.y, 0);
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(0, angle.eulerAngles.y, 0), 1f);
        this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(angle.eulerAngles.x, 0, 0), 1f);
    }

    // 카메라 앵글을 y축으로 움직일 때에 제한을 두는 함수입니다.
    private bool CheckAngleValidity()
    {
        switch (this.transform.rotation.eulerAngles.x)
        {
            case > 270 when this.transform.rotation.eulerAngles.x <= maxDegree:
                this.transform.rotation = Quaternion.Euler(maxDegree, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
                return false;
            case > 270 when this.transform.rotation.eulerAngles.x + _rotation.x < maxDegree:
                return false;
            case < 90 when this.transform.rotation.eulerAngles.x >= minDegree:
                this.transform.rotation = Quaternion.Euler(minDegree, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
                return false;
            case < 90 when this.transform.rotation.eulerAngles.x + _rotation.x > minDegree:
                return false;
            default: return true;
        }
    }
}
