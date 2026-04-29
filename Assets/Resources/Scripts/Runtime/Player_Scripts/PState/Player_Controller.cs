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

    public bool CanMove { get; set; } // 상태에 따른 움직임 제어를 위한 프로퍼티
    public bool CanRotate { get; set; } // 상태에 따른 회전 제어를 위한 프로퍼티

    #endregion

    private void OnEnable()
    {
        if (PlayerInputManager.Instance != null)
        {
            _im = PlayerInputManager.Instance;
            _im.OnAttack += AttackInput;
        }
        else
        {
            StartCoroutine(CoWaitInputManager());
        }
    }
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

    private void OnDisable()
    {
        if (_im != null && PlayerInputManager.Instance != null)
        {
            _im.OnAttack -= AttackInput;
        }
    }

    private void Update()
    {
        ControllSwitch();

        MoveUpdate();
        RotateUpdate();
        AnimUpdate();
        SteminaUpdate();
        SoundUpdate();
    }

    private void MoveUpdate()
    {
        if (!CanMove || _im == null) { return; }
        if (_state == EMovementState.Attack) { return; }

        Vector2 move = _im.GetMoveInput;
        bool run = _im.GetRunInput;
        bool crouch = _im.GetCrouchInput;

        _moveCS.UpdateMove(move, run, crouch);
    }

    private void AnimUpdate()
    {
        _animCS.MoveAnimUpdate(_state);
    }

    private void SteminaUpdate()
    {
        _steminaCS.SetState(_state);
    }
    private void SoundUpdate()
    {
        _soundCS.SetSoundDistatce(_state);
    }

    private void SetMovementState(EMovementState state)
    {
        _state = state;
    }

    private void RotateUpdate()
    {
        if (!CanRotate || _im == null) { return; }

        _rotateCS.SetTarget(_im.GetMousePos);
    }

    private void AttackInput()
    {
        if (_attackCS == null)
        {
            Debug.LogWarning($"[{this.name}] : 공격 스크립트 없음");
            return;
        }

        _attackCS.TryAttack();
    }

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
                CanMove = true; CanRotate = true;
                break;
            case EControllMode.UIOpen:
                CanMove = false; CanRotate = false;
                break;
            case EControllMode.Attack:
                CanMove = false; CanRotate = true;
                break;
            case EControllMode.AllLock:
                CanMove = false; CanRotate = false;
                break;
        }
    }
    #endregion
}
