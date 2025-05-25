using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEditor;

public class ExitTile : MonoBehaviour
{
    [Header("Fade (Sprite)")]
    public SpriteRenderer fadeSprite;
    public float fadeDuration = 1.5f;

    [Header("Optional")]
    public string nextSceneName;       

    private PlayerController _playerCtrl;
    private CinemachineCamera _fpCam;
    private CinemachineCamera _tpCam;
    private bool _playerInside, _exiting;
    private int _origFpPrio, _origTpPrio;

    private void Awake()
    {
        var cam = Camera.main;
        var fadeObj = cam.transform.Find("FadeSprite");
        if (fadeObj != null)
            fadeSprite = fadeObj.GetComponent<SpriteRenderer>();
        else
            Debug.LogError("FadeSprite 오브젝트를 Main Camera 자식에서 찾을 수 없습니다.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;

        // 플레이어·카메라 참조
        _playerCtrl = other.GetComponent<PlayerController>();
        var cams = other.GetComponentsInChildren<CinemachineCamera>(true);
        foreach (var cam in cams)
        {
            if (cam.name.Contains("Firstperson")) _fpCam = cam;
            else if (cam.name.Contains("Thirdperson")) _tpCam = cam;
        }
        if (_fpCam != null) _origFpPrio = _fpCam.Priority;
        if (_tpCam != null) _origTpPrio = _tpCam.Priority;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
       
    }

    private void Update()
    {
        if (!_playerInside || _exiting) return;
        if (Keyboard.current.eKey.wasPressedThisFrame)
            StartCoroutine(HandleExitSequence());
    }

    private IEnumerator HandleExitSequence()
    {
        _exiting = true;
        

        // 1) 플레이어 입력 잠금
        if (_playerCtrl != null)
        {
            _playerCtrl.Cleared();
            _playerCtrl.enabled = false;
            //var rb = _playerCtrl.GetComponent<Rigidbody>();
            //if (rb) rb.AddForce(Vector3.zero, ForceMode.VelocityChange);
        }

        // 2) 카메라 전환
        if (_fpCam != null && _tpCam != null)
            _tpCam.Priority = _fpCam.Priority + 10;

        // 3) 페이드아웃
        float t = 0f;
        // 초기 알파 0
        var col = fadeSprite.color;
        col.a = 0f;
        fadeSprite.color = col;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            col.a = Mathf.Clamp01(t / fadeDuration);
            fadeSprite.color = col;
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        // 5) 게임 종료
#if UNITY_EDITOR
        // 에디터에서는 Play 모드를 중지
        EditorApplication.isPlaying = false;
#else
        // 실제 빌드에서는 애플리케이션 종료
        Application.Quit();
#endif

    }

    private void OnDisable()
    {
        // 초기 상태 복구
        if (fadeSprite)
        {
            var c = fadeSprite.color;
            c.a = 0f;
            fadeSprite.color = c;
        }
        if (_fpCam != null) _fpCam.Priority = _origFpPrio;
        if (_tpCam != null) _tpCam.Priority = _origTpPrio;
    }
    
    public bool GetPlayerInside()
    {
        return _playerInside;
    }
}
