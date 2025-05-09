using UnityEngine;

//애니메이션 관련 스크립트입니다.
public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;
    private static readonly int Xspeed = Animator.StringToHash("Xspeed");
    private static readonly int Yspeed = Animator.StringToHash("Yspeed");
    private const float St = 0.1f;
    private Vector3 _v = Vector3.zero;
    
    public void Moving(Vector3 movement)
    {
        var getParam = new Vector3(anim.GetFloat(Xspeed), 0, anim.GetFloat(Yspeed));
        var dampedParam = Vector3.SmoothDamp(getParam, movement, ref _v, St);
        anim.SetFloat(Xspeed, dampedParam.x);
        anim.SetFloat(Yspeed, dampedParam.z);
    }

    public void Attacked(bool isAttacked)
    {
        anim.SetBool("Attacked", isAttacked);
    }
}
