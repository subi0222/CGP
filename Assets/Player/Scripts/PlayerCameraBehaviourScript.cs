using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PlayerCameraBehaviourScript : MonoBehaviour
{
    public float mouseSpeed = 20f;
    public float maxDegree = 275f;
    public float minDegree = 50f;
    
    private Vector3 _rotation;
    private void Update()
    {
        _rotation.x = Input.GetAxis("Mouse Y") * -mouseSpeed;
    }

    private void FixedUpdate()
    {
        if (!CheckAngleValidity()) return;
        
        this.transform.Rotate(_rotation.x, 0, 0);
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
