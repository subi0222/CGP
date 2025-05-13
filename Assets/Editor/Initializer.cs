using Unity.Cinemachine;
using UnityEngine;
using UnityEditor;

// Scene에 Player를 배치 시 해야 할 기초적인 세팅을 도와주는 Editor입니다.
[CustomEditor(typeof(MapGenerator))]
public class Initializer : Editor
{
    // UI 표시를 담당합니다. 플레이어가 NPC와 상호작용 시 호출됩니다.
    public GameObject uiManager;
    // 플레이어의 시야를 담당할 CinemachineCamera가 동작하기 위해 필요합니다.
    private Camera _mainCamera;
    void OnEnable()
    {
        CheckCamera();
        CheckUIManager();
    }

    // 카메라가 없으면 만들고, 있으면 호출하여 CinemachineBrain 컴포넌트를 추가합니다.
    private void CheckCamera()
    {
        if (FindAnyObjectByType<Camera>() == null)
        {
            _mainCamera = new GameObject("Main Camera").AddComponent<Camera>();
            _mainCamera.tag = "MainCamera";
        }
        else
        {
            _mainCamera = GameObject.FindAnyObjectByType<Camera>();   
        }
    }

    // UIManager를 생성합니다.
    private void CheckUIManager()
    {
        if (_mainCamera.GetComponent<CinemachineBrain>() == null)
        {
            _mainCamera.gameObject.AddComponent<CinemachineBrain>();
        }

        if (PrefabUtility.FindAllInstancesOfPrefab(uiManager).Length == 0)
        {
            PrefabUtility.InstantiatePrefab(uiManager);
        }
    }
}
