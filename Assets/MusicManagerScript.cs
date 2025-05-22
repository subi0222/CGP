using UnityEngine;
using UnityEngine.Serialization;

public class MusicManagerScript : MonoBehaviour
{
    public static MusicManagerScript Instance;

    private AudioSource audioSource;
    public AudioClip PatrolBGM;
    public AudioClip DetectedBGM;
    public AudioClip RestrainedBGM;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        // 싱글톤 처리 (선택)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip) return; // 같은 음악이면 무시
        audioSource.clip = clip;
        audioSource.Play();
    }

    // 상황별 함수 예시
    public void PlayPatrolBGM() => PlayBGM(PatrolBGM);
    public void PlayDetectedBGM() => PlayBGM(DetectedBGM);
    public void PlayRestrainedBGM() => PlayBGM(RestrainedBGM);
}
