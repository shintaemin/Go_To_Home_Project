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
    Throwing,
    Interact,
    Hit,
    Dead,
    End,
}

// 상태에 따른 입력 제어를 위한 열거형
public enum EControllMode
{
    None,
    Playing,
    Run,
    Inventory,
    Attack,
    Throwing,
    AllLock,
}

public class Player_Controller : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private EMovementState _state;
    [SerializeField] private EControllMode _controlMode;

    [SerializeField] private Player_Health _healthCS;
    [SerializeField] private Player_Move _moveCS;
    [SerializeField] private Player_Anim _animCS;
    [SerializeField] private Player_Sound _soundCS;
    [SerializeField] private Player_Stemina _steminaCS;
    [SerializeField] private Player_Attack _attackCS;
    [SerializeField] private Player_Throwing _throwingCS;
    [SerializeField] private Player_LoockMousePointer _rotateCS;
    [SerializeField] private Player_InteractFinder _finderCS;
    [SerializeField] private Player_Interact _interactCS;
    [SerializeField] private Player_ItemEquip _itemEquipCS;
    [SerializeField] private Player_QuickSlot _quickSlotCS;
    [SerializeField] private Player_HeadLight _headLightCS;
    [SerializeField] private Inventory_Manager _inventoryCS;
    #endregion

    #region 내부 변수
    private PlayerInputManager _im;
    private bool _waitAttack = false;
    #endregion

    #region 프로퍼티
    public EMovementState MovementState
    {
        get { return _state; }
        set { SetMovementState(value); }
    }

    private bool CanMove { get; set; } // 상태에 따른 움직임 제어를 위한 프로퍼티
    private bool CanRotate { get; set; } // 상태에 따른 회전 제어를 위한 프로퍼티
    private bool CanAttack { get; set; } // 상태에 따른 공격 제어를 위한 프로퍼티
    #endregion
    
    #region 이벤트 구독
     private void OnEnable()
    {
        if (PlayerInputManager.Instance != null)
        {
            SubscriptInputManager();
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
                SubscriptInputManager();
                yield break;
            }
            yield return null;
        }
    }
    private void SubscriptInputManager() // 구독
    {
        if (_im != null || PlayerInputManager.Instance == null) { return; }

        _im = PlayerInputManager.Instance;
        _im.OnAttack += AttackInput;
        _im.OnInventory += InventoryInput;
        _im.OnInteract += InteractInput;
        _im.OnWeaponEquip += WeaponEquip;
        _im.OnThrowingEquip += ThrowingEquip;
        _im.OnHeadLight += HeadLightInput;
        _im.OnEsc += EscInput;
    }
    #endregion
    private void OnDisable()
    {
        if (_im != null && PlayerInputManager.Instance != null)
        {
            _im.OnAttack -= AttackInput;
            _im.OnInventory -= InventoryInput;
            _im.OnInteract -= InteractInput;
            _im.OnWeaponEquip -= WeaponEquip;
            _im.OnThrowingEquip -= ThrowingEquip;
            _im.OnHeadLight -= HeadLightInput;
            _im.OnEsc -= EscInput;
        }
        if (_healthCS != null)
        {
            _healthCS.OnHit += OnHitHandle;
            _healthCS.OnDead += OnDeadHandle;
        }
    }

    private void WeaponEquip()
    {
        if (_quickSlotCS == null || _controlMode != EControllMode.Playing) { return; }

        _quickSlotCS.WeaponQuickSlotEquip();
    }
    private void ThrowingEquip()
    {
        if (_quickSlotCS == null || _controlMode != EControllMode.Playing) { return; }

        _quickSlotCS.ThrowingQuickSlotEquip();
    }

    private void HeadLightInput()
    {
        if (_headLightCS == null || _controlMode != EControllMode.Playing) { return; }

        _headLightCS.SetToogleHeadLight();
    }

    private void EscInput()
    {
        if (UI_Manager.Instance == null) { return; }

        UI_Manager.Instance.EscInputActive();
    }

    private void AttackInput()
    {
        if (!CanAttack) { return; }
        if (_itemEquipCS == null || _attackCS == null || _throwingCS == null) { return; }


        if (_waitAttack)
        {
            _throwingCS.OnTrowing();
        }
        else
        {
            if (!_itemEquipCS.IsAttackable()) { return; }
            _attackCS.TryAttack();
        }
    }

    private void InventoryInput()
    {
        if (MovementState is EMovementState.Attack or EMovementState.Dead) { return; }
        if (_inventoryCS == null)
        {
            if (Inventory_Manager.Instance == null)
            {
                GUtill.Log($"[{this.name}] : 인벤토리 매니저 없음", EDebugType.Warn);
                return;
            }

            _inventoryCS = Inventory_Manager.Instance;
        }

        _inventoryCS.TryInventoryOpen();
        GUtill.Log($"[{this.name}] : 인벤토리 열림");
    }

    private void InteractInput()
    {
        if (MovementState is EMovementState.Attack or EMovementState.Dead) { return; }
        if (_controlMode == EControllMode.Inventory) { return; }
        if (_interactCS == null)
        {
            GUtill.Log($"[{this.name}] : 상호작용 스크립트 없음", EDebugType.Warn);
            return;
        }

        GUtill.Log($"[{this.name}] : 상호작용 시작");
        _interactCS.TryInteract();
    }
    private void OnHitHandle()
    {
        MovementState = EMovementState.Hit;
        _animCS.SetTreggerAnim(MovementState);
    }
    private void OnDeadHandle()
    {
        CanMove = false;
        CanRotate = false;
        CanAttack = false;
        MovementState = EMovementState.Dead;
        _animCS.SetTreggerAnim(MovementState);
    }
    #endregion

    private void Awake()
    {
        if (_moveCS == null) { GUtill.TryGetCS(this, ref _moveCS); }
        if (_animCS == null) { GUtill.TryGetCS(this, ref _animCS); }
        if (_soundCS == null) { GUtill.TryGetCS(this, ref _soundCS); }
        if (_steminaCS == null) { GUtill.TryGetCS(this, ref _steminaCS); }
        if (_attackCS == null) { GUtill.TryGetCS(this, ref _attackCS); }
        if (_interactCS == null) { GUtill.TryGetCS(this, ref _interactCS); }
        if (_rotateCS == null) { GUtill.TryGetCS(this, ref _rotateCS); }
        if (_finderCS == null) { GUtill.TryGetCS(this, ref _finderCS); }
        if (_healthCS == null) { GUtill.TryGetCS(this, ref _healthCS); }
        if (_itemEquipCS == null) { GUtill.TryGetCS(this, ref _itemEquipCS); }
        if (_throwingCS == null) { GUtill.TryGetCS(this, ref _throwingCS); }
        if (_quickSlotCS == null) { GUtill.TryGetCS(this, ref _quickSlotCS); }
        if (_headLightCS == null) { GUtill.TryGetCS(this, ref _headLightCS); }
    }

    private void Start()
    {
        if (_healthCS != null)
        {
            _healthCS.OnHit += OnHitHandle;
            _healthCS.OnDead += OnDeadHandle;
        }
    }

    private void Update()
    {
        if (MovementState == EMovementState.Dead) { return; }
        if (_controlMode == EControllMode.AllLock) { return; }

        ThrowingUpdate();
        MoveUpdate(); // 이동 업데이트
        RotateUpdate(); // 회전 업데이트
        _steminaCS?.SetState(_state);    // 스테미너 업데이트
        _soundCS?.SetSoundDistatce(_state);
        _finderCS?.Find();
    }
    private void ThrowingUpdate()
    {
        if (_throwingCS == null || _im == null || _itemEquipCS == null || !_itemEquipCS.IsTrowingable() || _controlMode == EControllMode.Inventory)
        {
            if (_waitAttack) 
            { 
                _waitAttack = false;
                _throwingCS?.OffThrowing();
            }
            return; 
        }

        bool aiming = _im.GetThrowingInput;

        if (aiming)
        {
            _waitAttack = true;
            _throwingCS.TrowingPosUpdate(_im.GetMousePos);
        }
        else
        {
            _waitAttack = false;
            _throwingCS.OffThrowing();
        }
    }
    private void MoveUpdate()
    {
        if (MovementState == EMovementState.Dead) { return; }
        if (!CanMove || _im == null) { return; }

        Vector2 move = _im.GetMoveInput;
        bool run = _im.GetRunInput;
        bool crouch = !run ? _im.GetCrouchInput : false;

        _moveCS?.UpdateMove(move, run, crouch); // 이동 명령
        _animCS?.MoveAnimUpdate(_state); // 이동 애니메이션 업데이트
    }
    private void RotateUpdate()
    {
        if (MovementState == EMovementState.Dead) { return; }
        if (!CanRotate || _im == null) { return; }

        _rotateCS?.SetTarget(_im.GetMousePos);
    }
    private void SetMovementState(EMovementState state)
    {
        _state = state;
        ControllSwitch(_state);
    }

    // 상태가 바뀌면 실행될 입력제어 스위치
    private void ControllSwitch(EMovementState state) 
    {
        EControllMode mode = EControllMode.Playing;
        switch (state)
        {
            case EMovementState.Attack: 
                mode = EControllMode.Attack; break;
            case EMovementState.Run: 
                mode = EControllMode.Run; break;
            case EMovementState.Throwing: 
                mode = EControllMode.Throwing; break;
            case EMovementState.Dead: mode = EControllMode.AllLock; break;
            case EMovementState.End: 
                mode = EControllMode.AllLock; 
                if (_animCS != null)
                {
                    _animCS.MoveAnimUpdate(_state);
                }
                break;
            case EMovementState.Idle: case EMovementState.Crouch: case EMovementState.Walk: 
                mode = EControllMode.Playing; break;
        }

        SetControllState(mode);
    }

    #region 외부 호출 함수
    /// <summary> 입력 상태 설정 상태에 따른 입력 제어 를 위함 </summary>
    /// <param name="state"> 각 상태를 넣기 </param>
    public void SetControllState(EControllMode state)
    {
        _controlMode = state;
        switch (state)
        {
            case EControllMode.Playing:   CanMove = true; CanRotate = true; CanAttack = true;    break;

            case EControllMode.Run:       CanMove = true; CanRotate = false; CanAttack = true;   break;

            case EControllMode.Attack:    CanMove = false; CanRotate = true; CanAttack = true;   break;

            case EControllMode.Throwing:  CanMove = true; CanRotate = true; CanAttack = true; break;

            case EControllMode.Inventory: CanMove = false; CanRotate = false; CanAttack = false; break;

            case EControllMode.AllLock:   CanMove = false; CanRotate = false; CanAttack = false; break;
        }
    }
    #endregion
}
