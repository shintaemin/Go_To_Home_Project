using System.Collections;
using UnityEngine;

#region 플레이어 콘트롤러
/*
 ▶ 할일
  - 입력이 있을때 플레이어 상태를 정의하고 각 스크립트에 명령
*/
#endregion
public enum EMovementState
{
    None,
    Idle,
    Crouch,
    Walk,
    Run,
    Attack,
    Interact,
}

// 상태에 따른 입력 제어를 위한 열거형
public enum EControllMode
{
    None,
    Playing,
    UIOpen,
    Attack,
    AllLock,
}

public class Player_Controller : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private EMovementState _state;

    [SerializeField] private Player_State _stateCS;
    [SerializeField] private Player_Move _moveCS;
    [SerializeField] private Player_Anim _animCS;
    [SerializeField] private Player_Stemina _steminaCS;
    [SerializeField] private Player_Attack _attackCS;
    [SerializeField] private Player_Sound _soundCS;
    [SerializeField] private Player_LoockMousePointer _rotateCS;
    #endregion

    #region 내부 변수
    private PlayerInputManager _im;
    #endregion


    #region 프로퍼티
    public EMovementState MovementState
    {
        get { return _state; }
        set { SetMovementState(value); }
    }

    private bool CanMove { get; set; } // 상태에 따른 움직임 제어를 위한 프로퍼티
    private bool CanRotate { get; set; } // 상태에 따른 회전 제어를 위한 프로퍼티
    private bool CanAttack { get; set; }
    #endregion
    
    #region 공격 이벤트
    private void OnEnable()
    {
        if (PlayerInputManager.Instance != null)
        {
            _im = PlayerInputManager.Instance;
            _im.OnAttack += AttackInput;
        }
        else
        {
            StartCoroutine(CoWaitInputManager()); // 인풋매니저를 못찾을 경우를 대비한 안전장치
        }

        MovementState = EMovementState.Idle;
    }
    #region 인풋 매니저 생성 대기 코루틴
    private IEnumerator CoWaitInputManager()
    {
        while (true)
        {
            if (PlayerInputManager.Instance != null)
            {
                _im = PlayerInputManager.Instance;
                _im.OnAttack += AttackInput;
                yield break;
            }
            yield return null;
        }
    }
    #endregion
    private void OnDisable()
    {
        if (_im != null && PlayerInputManager.Instance != null)
        {
            _im.OnAttack -= AttackInput;
        }
    }

    private void AttackInput()
    {
        if (!CanAttack) { return; }
        if (_attackCS == null)
        {
            Debug.LogWarning($"[{this.name}] : 공격 스크립트 없음");
            return;
        }

        _attackCS.TryAttack();
    }
    #endregion

    private void Update()
    {
        MoveUpdate(); // 이동 업데이트
        RotateUpdate(); // 회전 업데이트
        _steminaCS.SetState(_state);    // 스테미너 업데이트
        _soundCS.SetSoundDistatce(_state); // 사운드 범위 업데이트
    }

    private void MoveUpdate()
    {
        if (!CanMove || _im == null) { return; }

        Vector2 move = _im.GetMoveInput;
        bool run = _im.GetRunInput;
        bool crouch = _im.GetCrouchInput;

        _moveCS.UpdateMove(move, run, crouch); // 이동 명령
        _animCS.MoveAnimUpdate(_state); // 이동 애니메이션 업데이트
    }

    private void SetMovementState(EMovementState state)
    {
        _state = state;
        ControllSwitch();
    }

    private void RotateUpdate()
    {
        if (!CanRotate || _im == null) { return; }

        _rotateCS.SetTarget(_im.GetMousePos);
    }

    // 상태가 바뀌면 실행될 입력제어 스위치
    private void ControllSwitch() 
    {
        EControllMode mode = EControllMode.Playing;
        switch (_state)
        {
            case EMovementState.Attack: mode = EControllMode.Attack; break;
            case EMovementState.Idle:case EMovementState.Walk:
            case EMovementState.Run: case EMovementState.Crouch:
                mode = EControllMode.Playing; break;
        }

        SetControllState(mode);
    }

    #region 외부 호출 함수
    /// <summary> 입력 상태 설정 상태에 따른 입력 제어 를 위함 </summary>
    /// <param name="state"> 각 상태를 넣기 </param>
    public void SetControllState(EControllMode state)
    {
        switch (state)
        {
            case EControllMode.Playing:
                CanMove = true; CanRotate = true; CanAttack = true;
                break;
            case EControllMode.UIOpen:
                CanMove = false; CanRotate = false; CanAttack = false;
                break;
            case EControllMode.Attack:
                CanMove = false; CanRotate = true; CanAttack = true;
                break;
            case EControllMode.AllLock:
                CanMove = false; CanRotate = false; CanAttack = false;
                break;
        }
    }
    #endregion
}
