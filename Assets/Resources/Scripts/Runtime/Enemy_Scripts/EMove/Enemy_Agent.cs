using System;
using UnityEngine;
using UnityEngine.AI;

#region 적 이동
/*
 ▶ 할일
  - 적을 이동시킬떄 외부에서 목표위치만 검사해서 지정할 수 있도록 작업
*/
#endregion


public class Enemy_Agent : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private NavMeshAgent _agent;
    #endregion

    #region 내부변수
    public bool _isMove;
    #endregion

    #region 이벤트
    public event Action OnTargetPosArrival;
    #endregion

    private void Awake()
    {
        if (_agent == null)
        {
            GUtill.TryGetCS(this, ref _agent);
        }
    }

    private void Update()
    {
        if (!_isMove) { return; }
        if (_agent == null) { return; }
        if (_agent.pathPending) { return; }

        if (_agent.hasPath && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            TargetArrival();
            return;
        }
    }
    private void TargetArrival()
    {
        StopMove();

        OnTargetPosArrival?.Invoke();
    }
    #region 외부 호출 함수
    public void SetTargetPos(Vector3 pos)
    {
        if (_agent == null) { return; }

        _isMove = true;
        _agent.SetDestination(pos);
    }
    public void StopMove()
    {
        if (_agent == null) { return; }

        _agent.velocity = Vector3.zero;
        _agent.ResetPath();
        _isMove = false;
    }
   
    public void SetSpeed(float speed)
    {
        if (_agent == null) { return; }

        _agent.speed = speed;
    }
    public float GetSpeed()
    {
        return _agent != null ? _agent.velocity.magnitude : 0f;
    }
    #endregion
}
