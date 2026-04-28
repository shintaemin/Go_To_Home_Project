using UnityEngine;

#region 플레이어 상태
/*
 ▶ 할일
  - 플레이어의 상태를 지정받고 관리할 스크립트
  - 인풋 매니저에 현재 플레이어 상태에따른 지정까지 작업
*/
#endregion

public enum EPlayerState
{
    None,
    Idle,
    Attack,
    Interact,
    Healing,
    Dead,
}

public enum EMovementState
{
    None,
    Idle,
    Crouch,
    Walk,
    Run,
    Attack,
}

public class Player_State : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private EPlayerState _currentState;

    [Header("옵션")]
    [SerializeField] private EPlayerState _startState = EPlayerState.Idle; // 시작 상태 옵션
    [SerializeField] private bool _log = false; // 로그 확인용
    #endregion

    #region 내부변수
    private PlayerInputManager _im;
    private Player_Sound _sound;
    #endregion

    private void Awake()
    {
        if (_sound == null)
        {
            if (!TryGetComponent<Player_Sound>(out _sound))
            {
                Debug.LogWarning($"[{this.name}] : 플레이어 사운드 캐싱 Fail");
            }
        }
    }

    private void Start()
    {
        if (_im == null && PlayerInputManager.Instance != null)
        {
            _im = PlayerInputManager.Instance;
            if (_log) { Debug.Log($"[{this.name}] : 입력 매니저 캐싱 Success"); }
        }
        else
        {
            Debug.LogWarning($"[{this.name}] : 입력 매니저 캐싱 Fail");
        }

        _currentState = _startState;
    }

    // 상태가 변경되면 InputManager 에 전달할 스크립트
    private void InputStateUpdate(EPlayerState state)
    {
        if (_im == null && PlayerInputManager.Instance != null)
        {
            _im = PlayerInputManager.Instance;
            if (_im == null)
            {
                Debug.LogWarning($"[{this.name}] : 인풋 매니저가 씬에 없음");
                return;
            }
        }

        PlayerInputManager.EInputState input = PlayerInputManager.EInputState.Playing;

        switch(state)
        {
            case EPlayerState.Idle: input = PlayerInputManager.EInputState.Playing;
                break;
            case EPlayerState.Attack: input = PlayerInputManager.EInputState.Attack;
                break;
            case EPlayerState.Interact: input = PlayerInputManager.EInputState.AllLock;
                break;
            case EPlayerState.Dead: input = PlayerInputManager.EInputState.AllLock;
                break;
            case EPlayerState.Healing: input = PlayerInputManager.EInputState.AllLock;
                break;
        }

        _im.SetInputState(input);
    }

    #region 외부 호출 함수
    public void SetState(EPlayerState state, bool inputUpdate = true)
    {
        if (state == _currentState) { return; }

        EPlayerState lastState = _currentState;
        _currentState = state;

        if (_currentState == EPlayerState.Attack)
        {
            SetMovementState(EMovementState.Attack);
        }
        else if (lastState == EPlayerState.Attack)
        {
            SetMovementState(EMovementState.Idle);
        }

        if (_log) { Debug.Log($"[{this.name}] : 현재 상태 : {_currentState}"); }

        if (inputUpdate)
        {
            if (_log) { Debug.Log($"[{this.name}] : 인풋 상태 업데이트 완료"); }
            InputStateUpdate(_currentState);
        }
    }
    
    public void SetMovementState(EMovementState state)
    {
        if (_currentState == EPlayerState.Attack && state != EMovementState.Attack)
        {
            return;
        }

        if (_sound != null)
        {
            _sound.SetSoundDistatce(state);
        }
    }
    #endregion
}
