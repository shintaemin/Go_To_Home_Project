using System;
using UnityEngine;
using UnityEngine.InputSystem;

#region 플레이어 입력 매니저
/*
 ▶ 할일
  - 모든 입력을 관리할 1개의 매니저
  - 뉴인풋 시스템을 생성하고 생성된 액션의 입력을 구독하고 외부에서 사용하는 방식

   ※ 현재구조로 생각한 이유 ※
    각 스크립트에서 입력을 구독하고 사용하는 방식보다 유지보수가 쉬워진다.
    특정 상태에따른 제어가 가능해지고 추가된 상태만 EInputState 에 등록하거나 프로퍼티를 생성하여 사용하면된다.

    01. 객체지향 - 입력매니저는 입력을 검사만 하고 사용 하는곳에서는 호출하여 사용만하면됨
    02. 유지보수 - 검사를 한곳에서 하기때문에 특정 상태에따른 제어를 하거나 입력 오류 발생시 매니저 스크립트 하나만 확인하면됨
    03. 확장성 - 특정상태에선 입력을 끊거나 추가하는 게 매우 간편해지며 SetInputState 함수를 통해 입력제어를 하는것이 가능해짐
*/
#endregion


public class PlayerInputManager : MonoBehaviour
{
    // 셋팅시 타입을 넣어 반복을 최소화하기 위함
    public enum EInputType
    {
        None = 0,
        All,
        Run,
        Crouch,
    }

    public static PlayerInputManager Instance { get; private set; }

    #region 인스펙터
    [SerializeField] private bool _isRunToggle = true; // 달리기 토글
    [SerializeField] private bool _isCrouchToggle = true; // 웅크리기 토글
    #endregion

    #region 내부변수
    private PlayerInputActions _actions; // 생성한 인풋액션 에셋 (뉴인풋 시스템)
    #endregion

    #region 프로퍼티
    public bool RunInput { get; private set; } // 달리기 입력 프로퍼티
    public bool CrouchInput { get; private set; } // 웅크리기 입력 프로퍼티
    #endregion

    #region 이벤트
    public event Action OnAttack;   // 공격 이벤트
    public event Action OnInventory; // 인벤토리 이벤트
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        _actions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _actions.Enable(); // 뉴인풋 활성화

        InputSettingUpdate(EInputType.All); // 키 셋팅 업데이트
    }

    private void OnDisable()
    {
        _actions.Disable(); // 비활성화
        AllDiscription();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // 플레이어 셋팅이 변경되면 호출될 함수
    private void InputSettingUpdate()
    {
        // 토글 bool 값 전달 및 구독 진행
        RunToggleSetting(_isRunToggle); // 달리기 구독
        CrouchToggleSetting(_isCrouchToggle); // 웅크리기 구독
        _actions.Player.Attack.performed += OnAttackInput; // 공격 구독
        _actions.Player.Inventory.performed += OnInventoryInput; // 인벤토리 구독
    }

    private void AllDiscription()
    {
        _actions.Player.Crouch.performed -= OnCrouchToggle;
        _actions.Player.Crouch.started -= OnCrouchStarted;
        _actions.Player.Crouch.canceled -= OnCrouchCanceled;

        _actions.Player.Run.performed -= OnRunToggle;
        _actions.Player.Run.started -= OnRunStarted;
        _actions.Player.Run.canceled -= OnRunCanceled;

        _actions.Player.Attack.performed -= OnAttackInput;
        _actions.Player.Inventory.performed -= OnInventoryInput;
    }

    // 플레이어 셋팅이 변경되면 호출될 함수 (오버로딩)
    private void InputSettingUpdate(EInputType type)
    {
        switch(type)
        {
            case EInputType.All:    InputSettingUpdate();                   break;
            case EInputType.Run:    RunToggleSetting(_isRunToggle);         break;
            case EInputType.Crouch: CrouchToggleSetting(_isCrouchToggle);   break;
        }
    }

    // 달리기 토글 구독 설정
    private void RunToggleSetting(bool enabled)
    {
        _actions.Player.Run.performed -= OnRunToggle;
        _actions.Player.Run.started -= OnRunStarted;
        _actions.Player.Run.canceled -= OnRunCanceled;
        if (enabled)
        {
            _actions.Player.Run.performed += OnRunToggle;
        }
        else
        {
            _actions.Player.Run.started += OnRunStarted;
            _actions.Player.Run.canceled += OnRunCanceled;
        }
    }

    // 웅크리기 토글 구독 설정
    private void CrouchToggleSetting(bool enabled)
    {
        _actions.Player.Crouch.performed -= OnCrouchToggle;
        _actions.Player.Crouch.started -= OnCrouchStarted;
        _actions.Player.Crouch.canceled -= OnCrouchCanceled;

        if (enabled)
        {
            _actions.Player.Crouch.performed += OnCrouchToggle;
        }
        else
        {
            _actions.Player.Crouch.started += OnCrouchStarted;
            _actions.Player.Crouch.canceled += OnCrouchCanceled;
        }
    }

    /// <summary> 이벤트를 받기 위한 토글 상태따른 입력 설정 반전 </summary>
    /// <param name="ctx"> 이벤트의 값을 전달받는다 </param>
    #region 달리기 이벤트
    private void OnRunToggle(InputAction.CallbackContext ctx) => RunInput = !RunInput;
    private void OnRunStarted(InputAction.CallbackContext ctx) => RunInput = true;
    private void OnRunCanceled(InputAction.CallbackContext ctx) => RunInput = false;
    #endregion
    #region 웅크리기 이벤트
    private void OnCrouchToggle(InputAction.CallbackContext ctx) => CrouchInput = !CrouchInput;
    private void OnCrouchStarted(InputAction.CallbackContext ctx) => CrouchInput = true;
    private void OnCrouchCanceled(InputAction.CallbackContext ctx) => CrouchInput = false;
    #endregion
    #region 공격 이벤트
    private void OnAttackInput(InputAction.CallbackContext ctx) => OnAttack?.Invoke();
    #endregion
    #region 인벤토리 이벤트
    private void OnInventoryInput(InputAction.CallbackContext ctx) => OnInventory?.Invoke();
    #endregion

    #region 외부 호출 함수
    public Vector2 GetMoveInput => _actions.Player.Move.ReadValue<Vector2>();
    public Vector2 GetMousePos => _actions.Player.MousePosition.ReadValue<Vector2>();
    public bool GetRunInput => RunInput;
    public bool GetCrouchInput => CrouchInput;

    /// <summary> 이후 옵션 매니저 생성하여 게임옵션 설정때에 사용할 함수 미리 사용 </summary>
    /// <param name="toggle"> 토글 사용 유무 On / Off </param>
    public void SetRunToggle (bool toggle)
    {
        _isRunToggle = toggle;
        InputSettingUpdate(EInputType.Run);
    }
    public void SetCrouchToggle(bool toggle)
    {
        _isCrouchToggle = toggle;
        InputSettingUpdate(EInputType.Crouch);
    }
    #endregion
}
