using System.Collections;
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
        GUtill.TryGetCS(this, ref _patrollCS);
        GUtill.TryGetCS(this, ref _agentCS);
        GUtill.TryGetCS(this, ref _animCS);
        GUtill.TryGetCS(this, ref _trackingCS);
    }
    private void OnEnable()
    {
        if (_trackingCS != null) { _trackingCS.OnSoundTracking += HandleOnSoundTracking; }
        if (_agentCS != null) { _agentCS.OnTargetPosArrival += HandleOnTargetPosArrival; }
    }
    private void OnDestroy()
    {
        if (_trackingCS != null) { _trackingCS.OnSoundTracking -= HandleOnSoundTracking; }
        if (_agentCS != null) { _agentCS.OnTargetPosArrival -= HandleOnTargetPosArrival; }
    }
    private void Start()
    {
        ReturnPatroll();
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
        switch(EnemyMoveState)
        {
            case EEnemyMoveState.Patroll:
                _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
                _patrollCS.PatrollWaitTimeUpdate(); 
                break;
            case EEnemyMoveState.Tracking:
                if (_trackingCS.HasLiveTarget())
                {
                    // Combat 작업 전 테스팅용 3줄
                    Debug.Log($"[{this.name}] : 전투 돌입!");
                    _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
                    ReturnPatroll();
                }
                else
                {
                    _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
                    ReturnPatroll();
                }
                break;
        }
    }
    private void Update()
    {
        switch(EnemyMoveState)
        {
            case EEnemyMoveState.Patroll: PatrollLoop(); break;
            case EEnemyMoveState.Tracking: TrackingLoop(); break;
            case EEnemyMoveState.Combat: CombatLoop(); break;
        }
    }
    private void PatrollLoop()
    {
        if (_patrollCS == null || _agentCS == null || _animCS == null) { return; }

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
            AgentMoveUpdeate(_lastPatrollPos, EEnemyMoveAnim.Walk);
        }
    }
    private void TrackingLoop()
    {
        if (_trackingCS == null || _agentCS == null || _animCS == null) { return; }

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
                AgentMoveUpdeate(targetPos, EEnemyMoveAnim.Fast);
            }
        }
    }
    private void CombatLoop()
    {

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

    #region 외부 호출 함수
    public void SetMoveState(EEnemyMoveState state)
    {
        EnemyMoveState = state;
        if (state == EEnemyMoveState.Dead || state == EEnemyMoveState.Combat)
        {
            _agentCS?.StopMove();
        }
    }
    #endregion
}
