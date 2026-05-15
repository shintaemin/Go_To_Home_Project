using UnityEngine;

#region 사운드 매니저
/*
 ▶ 할일
  - BGM 은 게임이 시작될때 게임매니저가 사운드매니저의 타입을 지정하며 bgm 을 재생시킬 예정

  - 한프레임에 재생시킬 사운드에 갯수를 지정
  - 사운드재생을 제어할 스크립트
  - UI가 열려있거나 특정 상태에선 재생을 막을 수 있도록 작업
*/
#endregion

public enum ESountPlayType
{
    None = 0,
    Play,
    UI,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    #region 인스펙터
    [SerializeField] private AudioSource _bgmAudio;
    [SerializeField] private ESountPlayType _playType;

    [Header("옵션")]
    [SerializeField] private int _maxSfxPlayingCount = 5; // 동시재생에 최대치

    [Header("옵션")]
    [SerializeField] private float _sfxVolume = 0.3f;
    [SerializeField] private float _bgmVolume = 0.3f;
    #endregion
    #region 내부변수
    private int _sfxPlayingCount = 0; // 한프레임에 재생시킨 sfx 사운드 갯수
    #endregion
    #region 프로퍼티
    public ESountPlayType SoundPlayType { get; set; } // 사운드 재생 타입
    public float SfxVolume // 효과음 볼륨
    {
        get {  return _sfxVolume; }
        set { _sfxVolume = Mathf.Clamp01(value); }
    }
    public float BgmVolume // 배경음 볼륨
    {
        get { return _bgmVolume; }
        set { _bgmVolume = Mathf.Clamp01(value); }
    }
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            enabled = false;
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void LateUpdate()
    {
        _sfxPlayingCount = 0;
    }

    #region 외부 호출 함수
    public void BgmPlay()
    {

        _bgmAudio.Play();
    }

    public void SfxPlay(AudioSource source, ClipData data)
    {
        if (_sfxPlayingCount >= _maxSfxPlayingCount) { return; }
        if (SoundPlayType != ESountPlayType.Play) { return; }

        source.clip = data.GetClip;
        source.pitch = data.GetPitch;
        source.volume = SfxVolume;
        source.spatialBlend = data.GetSpatialBlend;
        source.Play();

        _sfxPlayingCount++;
    }

    public void UISfxPlay(AudioSource source, ClipData data)
    {
        source.clip = data.GetClip;
        source.pitch = data.GetPitch;
        source.volume = SfxVolume;
        source.spatialBlend = data.GetSpatialBlend;
        source.Play();
    }
    #endregion
}
