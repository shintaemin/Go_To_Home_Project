using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

public class Enemy_Controller : MonoBehaviour, ISoundListener
{
    #region 인스펙터
    [SerializeField] private EEnemyMoveState _moveState;

    [Header("옵션")]
    [SerializeField] private float _patrollInterval = 10.0f;
    #endregion

    #region 내부변수
    private Enemy_Patroll _patrollCS;
    private Enemy_Agent _agentCS;
    private Enemy_Anim _animCS;
    private float _nextPatrollTime;
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
    }

    private void Start()
    {
        _nextPatrollTime = Time.time + _patrollInterval;
    }

    private void Update()
    {
        if (_agentCS.TargetArrival()) // 목적지에 도착했다면
        {
            EnemyMoveState = EEnemyMoveState.Patroll;
            _animCS.SetSpeedParam(EEnemyMoveAnim.Idle);
        }

        PatrollMove();
    }

    private void PatrollMove()
    {
        if (_patrollCS == null) { return; }
        if (_animCS == null) { return; }
        if (_agentCS == null) { return; }

        if (Time.time >= _nextPatrollTime)
        {
            Vector3 targetPos = _patrollCS.GetRandomPatrollPos();

            AgentMoveUpdeate(targetPos , EEnemyMoveAnim.Walk); // Agent 에 타겟 위치 전달
            _nextPatrollTime = Time.time + _patrollInterval; // 다음 patroll 이동시간 지정
        }
    }

    private void TrackingMove(Vector3 pos)
    {
        EnemyMoveState = EEnemyMoveState.Tracking;

        AgentMoveUpdeate(pos, EEnemyMoveAnim.Fast);
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
    }
    public void OnSoundListen(Vector3 soundPos)
    {
        TrackingMove(soundPos);
    }
    #endregion
}
