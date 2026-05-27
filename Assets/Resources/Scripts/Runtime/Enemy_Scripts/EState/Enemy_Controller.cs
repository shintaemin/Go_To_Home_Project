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
        if (EnemyMoveState == EEnemyMoveState.Patroll)
        {
            PatrollMove();
        }

    }

    private void PatrollMove()
    {
        if (_patrollCS == null) { return; }
        if (_animCS == null) { return; }
        if (_agentCS == null) { return; }

        Vector3 targetPos;

        if (Time.time >= _nextPatrollTime) 
        {
            targetPos = _patrollCS.GetRandomPatrollPos();

            _nextPatrollTime = Time.time + _patrollInterval;
        }

        
    }
    private void AgentMoveUpdeate(Vector3 pos)
    {
        _agentCS.SetTargetPos(pos);
        _animCS.SetSpeedParam(EEnemyMoveAnim.Walk);
    }

    #region 외부 호출 함수
    public void SetMoveState(EEnemyMoveState state)
    {
        EnemyMoveState = state;
    }
    public void OnSoundListen(Vector3 soundPos)
    {
        EnemyMoveState = EEnemyMoveState.Tracking;
        _agentCS.SetTargetPos(soundPos);
        _animCS.SetSpeedParam(EEnemyMoveAnim.Fast);
    }
    #endregion
}
