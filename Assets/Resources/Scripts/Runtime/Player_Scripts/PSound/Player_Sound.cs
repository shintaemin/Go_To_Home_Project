using UnityEngine;

#region 플레이어 사운드
/*
 ▶ 할일
  - 플레이어의 사운드 범위를 관리할 스크립트
  - 미리 사운드의 범위를 지정해두고 사운드 재생하도록
  - 지정된 범위값을 외부에서 확인 할 수 있도록 하고 적도 이 값에따라 감지하는 로직을 구현할 예정
*/
#endregion


public class Player_Sound : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private AudioSource _audio;
    [SerializeField] private ClipList _clipList;
    [SerializeField] private Player_FootSound _footSoundCS;

    [Header("사운드 범위 값")]
    [SerializeField] private float _currentRange;
    [SerializeField] private float _crouchRage = 2f; // 웅크리기 사운드 범위
    [SerializeField] private float _walkRange = 5f; // 걷기 사운드 범위
    [SerializeField] private float _runRange = 10f; // 달리기 사운드 범위
    [SerializeField] private float _attackRange = 15f; // 공격 사운드 범위
    [SerializeField] private float _soundPlayCool = 0.3f;
    #endregion

    #region 내부 변수
    private float _lastSoundPlayTime;
    #endregion

    private void Awake()
    {
        if (_footSoundCS == null)
        {
            _footSoundCS = FindFirstObjectByType<Player_FootSound>();
        }
        GUtill.TryGetCS(this, ref _audio);
        _clipList.InitClipList();
    }

    private void Start()
    {
        _audio.spatialBlend = 1f; // 3D 오디오 설정
        _audio.playOnAwake = false; // 시작 사운드 재생 X
    }

    #region 외부 호출 함수
    public float GetPlayerSoundRange => _currentRange; // 외부에서 사운드 범위를 확인하기 위함

    /// <summary> 사운드 범위 지정 함수 </summary>
    /// <param name="state"> 플레이어의 움직임 상태에따른 사운드 범위지정</param>
    public void SetSoundDistatce(EMovementState state)
    {
        float range = -1f;
        switch(state)
        {
            case EMovementState.Idle:   range =     0f;         break;
            case EMovementState.Crouch: range = _crouchRage;    break;
            case EMovementState.Walk:   range = _walkRange;     break;
            case EMovementState.Run:    range = _runRange;      break;
            case EMovementState.Attack: range = _attackRange;   break;
        }

        if (range != -1f)
        {
            _currentRange = range;
        }
        else { GUtill.Log($"[{this.name}] : 상태 지정 실패 오디오범위 : {range}", EDebugType.Warn); }
    }

    public void OnMoveSoundEffectPlay()
    {
        if (Time.time - _lastSoundPlayTime < _soundPlayCool) { return; }
        _lastSoundPlayTime = Time.time;

        if (SoundEffect_PoolManager.Instance != null)
        {
            SoundEffect_PoolManager.Instance.SpawnEffect(transform.position, _currentRange);
        }

        _footSoundCS?.PlayFootStep(_audio, _clipList);
    }
    
    public void OnAttackEffectPlay()
    {
        if (Time.time - _lastSoundPlayTime < _soundPlayCool) { return; }
        _lastSoundPlayTime = Time.time;

        if (SoundEffect_PoolManager.Instance != null)
        {
            SoundEffect_PoolManager.Instance.SpawnEffect(transform.position, _currentRange);
        }
        /*
         if (SoundManager.Instance != null)
        {
            // 여기서 공격 사운드 재생
            ClipData clip = _clipList.GetClipData(EClipPlayType.Attack);

            if (clip != null) { SoundManager.Instance.SfxPlay(_audio, clip); }
        }
         */
    }

    // 외부 사용을 위해 (추후 SoundManager 작업시 필요할 수 있어 미리 작업)
    public AudioSource GetAudio => _audio; 

    public void AudioPlay()
    {
        _audio.Play();
    }
    #endregion
}
