using UnityEngine;

#region 컨트롤러
/*
 ▶ 할일
  - 플레이어처럼 한곳에서 모아서 관리하기 위함
*/
#endregion

public enum EEnemyMoveState
{
    None = 0,
    Patroll,
    Tracking,
    Combat,
    Dead,
}

public class Enemy_Controller : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private EEnemyMoveState _moveState;
    #endregion

    #region 내부변수
    private Enemy_Patroll _patrollCS;
    private Enemy_Agent _agentCS;
    private Enemy_Anim _animCS;
    private Enemy_Tracking _trackingCS;
    private Enemy_Combat _combatCS;
    private Enemy_Health _healthCS;
    private Vector3 _lastPatrollPos;
    #endregion

    #region 프로퍼티
    public EEnemyMoveState EnemyMoveState
    {
        get { return _moveState; }
        private set { _moveState = value; }
    }
    #endregion

    private void Awake()
    {
        GUtill.TryGetCS(this, ref _patrollCS); GUtill.TryGetCS(this, ref _agentCS); 
        GUtill.TryGetCS(this, ref _animCS);    GUtill.TryGetCS(this, ref _trackingCS); 
        GUtill.TryGetCS(this, ref _combatCS);  GUtill.TryGetCS(this, ref _healthCS);
    }
    #region 각 이벤트 구독
    private void OnEnable()
    {
        SubscriptEvent();
    }
    private void OnDisable()
    {
        DiscriptEvent();
    }
    private void SubscriptEvent()
    {
        if (_trackingCS != null) { _trackingCS.OnSoundTracking += HandleOnSoundTracking; }
        if (_agentCS != null) { _agentCS.OnTargetPosArrival += HandleOnTargetPosArrival; }
        if (_combatCS != null) { _combatCS.OnTryAttack += HandleOnAttack; }
        if (_healthCS != null)
        {
            _healthCS.OnHit += HandleOnHit;
            _healthCS.OnDead += HandleOnDead;
        }
    }
    private void DiscriptEvent()
    {
        if (_trackingCS != null) { _trackingCS.OnSoundTracking -= HandleOnSoundTracking; }
        if (_agentCS != null) { _agentCS.OnTargetPosArrival -= HandleOnTargetPosArrival; }
        if (_combatCS != null) { _combatCS.OnTryAttack -= HandleOnAttack; }
        if (_healthCS != null)
        {
            _healthCS.OnHit -= HandleOnHit;
            _healthCS.OnDead -= HandleOnDead;
        }
    }
    private void HandleOnSoundTracking(Vector3 soundPos)
    {
        if (EnemyMoveState == EEnemyMoveState.Dead || EnemyMoveState == EEnemyMoveState.Combat) return;

        if (EnemyMoveState == EEnemyMoveState.Patroll)
        {
            SetMoveState(EEnemyMoveState.Tracking);
            _trackingCS.TrackingStart();
        }
    }
    private void HandleOnTargetPosArrival()
    {
        if (EnemyMoveState == EEnemyMoveState.Dead) return;

        switch (EnemyMoveState)
        {
            case EEnemyMoveState.Patroll:
                _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
                _patrollCS.PatrollWaitTimeUpdate();
                break;
            case EEnemyMoveState.Tracking:
                if (_trackingCS.HasLiveTarget())
                {
                    SetMoveState(EEnemyMoveState.Combat);
                    _combatCS.CombatActive(true, _trackingCS.GetTarget());
                }
                else
                {
                    _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
                    ReturnPatroll();
                }
                break;
        }
    }
    private void HandleOnAttack()
    {
        if (EnemyMoveState == EEnemyMoveState.Dead) return;

        _animCS.TriggerAnim(EEnemyAnimTrigger.Attack);
    }
    private void HandleOnHit()
    {
        if (EnemyMoveState == EEnemyMoveState.Dead) return;

        _agentCS.StopMove();
        _combatCS.CombatActive(false);
        _animCS.TriggerAnim(EEnemyAnimTrigger.Hit);
        SetMoveState(EEnemyMoveState.Tracking);
    }
    private void HandleOnDead()
    {
        if (_patrollCS == null || _trackingCS == null || _agentCS == null || _animCS == null || _combatCS == null) { return; }

        _agentCS.StopMove();
        _trackingCS.TargetClear();
        _combatCS.CombatActive(false);
        _patrollCS.PatrollMoveActive(false);
        SetMoveState(EEnemyMoveState.Dead);
        DiscriptEvent();

        _agentCS.enabled = false;
        _trackingCS.enabled = false;
        _combatCS.enabled = false;
        _patrollCS.enabled = false;
        _healthCS.enabled = false;
        _animCS.TriggerAnim(EEnemyAnimTrigger.Death);
    }
    #endregion
    private void Start()
    {
        ReturnPatroll();
    }
    private void Update()
    {
        if (_patrollCS == null || _trackingCS == null || _agentCS == null || _animCS == null || _combatCS == null) { return; }
        if (EnemyMoveState == EEnemyMoveState.Dead) { return; }

        switch(EnemyMoveState)
        {
            case EEnemyMoveState.Patroll: PatrollLoop(); break;
            case EEnemyMoveState.Tracking: TrackingLoop(); break;
            case EEnemyMoveState.Combat: CombatLoop(); break;
        }
    }
    private void PatrollLoop()
    {
        _trackingCS.UpdateTarget();
        if (_trackingCS.HasLiveTarget())
        {
            _patrollCS.PatrollMoveActive(false);
            SetMoveState(EEnemyMoveState.Tracking);
            return;
        }

        _patrollCS.PatrollUpdate();

        Vector3 patrollPos = _patrollCS.PatrollPos();
        if (_lastPatrollPos != patrollPos)
        {
            _lastPatrollPos = patrollPos;
            float speed = _animCS.CanFastMove() ? 5f : 3f;
            _agentCS.SetSpeed(speed);
            AgentMoveUpdeate(_lastPatrollPos, EEnemyMoveAnim.Walk);
        }
    }
    private void TrackingLoop()
    {
        if (_trackingCS.IsTrackingAnimPlaying()) 
        {
            _agentCS.StopMove();
            _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
            return; 
        }

        if (!_trackingCS.IsTargetTracking())
        {
            ReturnPatroll();
            return;
        }

        _trackingCS.UpdateTarget();

        Vector3 targetPos = _trackingCS.GetTargetPos();
        if (targetPos != Vector3.zero)
        {
            Vector3 currentDest = _agentCS.GetComponent<UnityEngine.AI.NavMeshAgent>().destination;
            if (Vector3.Distance(currentDest, targetPos) > 0.1f)
            {
                float speed = _animCS.CanFastMove() ? 7f : 5f;
                _agentCS.SetSpeed(speed);
                AgentMoveUpdeate(targetPos, EEnemyMoveAnim.Fast);
            }
        }
    }
    private void CombatLoop()
    {
        if (!_trackingCS.IsTargetTracking())
        {
            _combatCS.CombatActive(false);
            ReturnPatroll();
            return;
        }

        Transform target = _trackingCS.GetTarget();
        if (target == null)
        {
            _combatCS.CombatActive(false);
            ReturnPatroll();
            return;
        }
        if (_combatCS.OutOfTarget(target))
        {
            _combatCS.CombatActive(false, target);
            SetMoveState(EEnemyMoveState.Tracking);
            return;
        }

        _combatCS.CombatUpdate();
    }
    private void ReturnPatroll()
    {
        EnemyMoveState = EEnemyMoveState.Patroll;
        _trackingCS.TargetClear();
        _agentCS.StopMove();
        _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
        _patrollCS.PatrollMoveActive(true);
    }
    private void AgentMoveUpdeate(Vector3 pos, EEnemyMoveAnim anim)
    {
        _agentCS.SetTargetPos(pos);
        _animCS.SetSpeedParam(anim);
    }
    private void SetMoveState(EEnemyMoveState state)
    {
        EnemyMoveState = state;
        if (EnemyMoveState == EEnemyMoveState.Dead || EnemyMoveState == EEnemyMoveState.Combat)
        {
            _agentCS?.StopMove();
        }
    }
    #region 외부 호출 함수
    public void DeathLastFrame()
    {
        _animCS.enabled = false;
        Destroy(this.gameObject);
        this.enabled = false;
    }
    #endregion
}
