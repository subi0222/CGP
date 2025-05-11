using UnityEngine;
using System.Collections.Generic;

public class NPCDoctorBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    static GameObject PlayerGameObject;
    public static List<NPCDoctorBehavior> AllDoctors = new List<NPCDoctorBehavior>();

    public static Transform GetPlayerTransform()
    {
        if (PlayerGameObject == null)
        {
            PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        }
        return PlayerGameObject.transform;
    }

    public static GameObject GetPlayer()
    {
        if (PlayerGameObject == null)
        {
            PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        }
        return PlayerGameObject;
    }

    // 플레이어 탐지 거리
    public float DetectionDistance = 5f;

    // 부른 NPC의 소리 감지 거리
    public float HearingDistance = 10f;

    // 플레이어 탐지 각도
    public float DetectionAngle = 30f;

    // NPC가 잡을 수 있는 거리
    public float GrabDistance = 1.2f;

    // NPC의 탐색 포인트
    public List<Transform> PatrolPoints = new List<Transform>();

    // NPC를를 부는 소리
    public AudioClip WistleSoundClip;

    // NPC가 갖고 있는 플래시 라이트트
    public GameObject FlashLightPrefab;

    // NPC가 갖고 있는 플래시 라이트트
    private GameObject FlashLightInstance;

    // NPC가 갖고 있는 플래시 라이트를 붙일 소켓
    public Transform FlashLightAttachSocketTransform;

    // NPC의 애니메이터
    private Animator animator;

    // NPC의 상태를 관리하는 인터페이스
    private NPCIDoctorState currentState;

    private GameObject NPCDoctor;

    // *추가: NPC의 머리입니다.
    public GameObject Head;

    // *추가: NPC의 머리 좌표를 반환합니다.
    public Vector3 GetHeadPosition()
    {
        return Head.transform.position;
    }

    public GameObject GetNPCDoctor()
    {
        return NPCDoctor;
    }

    public void PlayWhistleSound()
    {
        if (currentState is NPCDoctorStateFollowingPlayer followingState)
        {
            followingState.HandleWhistleSound(); // 상태 객체에 연결된 로직 위임
        }
    }

    public void HandleGrabPlayer()
    {
        if (currentState is NPCDoctorStateGrabPlayer grabState)
        {
            grabState.HandleGrabPlayer(); // 상태 객체에 연결된 로직 위임
        }
    }

    //Get 메소드

    public NPCIDoctorState GetCurrentState()
    {
        return currentState;
    }

    public Transform GetPatrolPoint(int index)
    {
        if (index >= 0 && index < PatrolPoints.Count)
        {
            return PatrolPoints[index];
        }
        return null;
    }

    public Transform GetRandomPatrolPoint()
    {
        return PatrolPoints[Random.Range(0, PatrolPoints.Count)];
    }

    public Transform GetNextPatrolPoint(int index)
    {
        return PatrolPoints[(index + 1) % PatrolPoints.Count];
    }

    public int GetRandomPatriolPointIndex()
    {
        return Random.Range(0, PatrolPoints.Count);
    }

    public int GetNextPointIndex(int index)
    {
        return (index + 1) % PatrolPoints.Count;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    // 게임 첫 시작시 호출되는 함수
    public void Start()
    {
        animator = GetComponentInChildren<Animator>();
        FlashLightInstance = Instantiate(FlashLightPrefab, FlashLightAttachSocketTransform.position, FlashLightAttachSocketTransform.rotation, FlashLightAttachSocketTransform);
        currentState = new NPCDoctorStatePatrol(); // 초기 상태 설정
        currentState.stateInit(this); // 상태 초기화
        PatrolPoints.Clear(); // PatrolPoints 리스트 초기화
        GameObject[] allPatrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        foreach (GameObject point in allPatrolPoints)
        {
            PatrolPoints.Add(point.transform); // PatrolPoints 리스트에 모든 PatrolPoint 추가
        }
    }

    // NPC 상태를 매 프레임마다 업데이트
    public void Update()
    {
        if (currentState != null)
        {
            Debug.Log(currentState.ToString()); // 현재 상태 출력
            NPCIDoctorState newState = currentState.canChangeState(); // 상태 변경 가능 여부 확인
            if (newState != null && newState != currentState)
            {
                currentState = newState; // 상태 변경
                currentState.stateInit(this); // 새로운 상태 초기화
                Debug.Log("State changed to: " + currentState.ToString()); // 상태 변경 로그
            }
            else
            {
                currentState.stateContinue(); // 현재 상태 계속 진행
            }
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform point in PatrolPoints)
        {
            Gizmos.DrawSphere(point.position, 0.5f);
        }
    }

    // 스폰될 때 리스트에 추가
    void Awake()
    {
        AllDoctors.Add(this);
    }

    // 파괴될 때 리스트에서 제거
    private void OnDestroy()
    {
        AllDoctors.Remove(this);
    }
}