using Unity.Cinemachine;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class Initializer : Editor
{
    public GameObject uiManager;
    private Camera _mainCamera;
    void OnEnable()
    {
        CheckCamera();
        CheckUIManager();
    }

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
