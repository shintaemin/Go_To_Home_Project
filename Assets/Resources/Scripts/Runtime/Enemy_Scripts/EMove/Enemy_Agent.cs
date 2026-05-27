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

    private void Awake()
    {
        if (_agent == null)
        {
            GUtill.TryGetCS(this, ref _agent);
        }
    }

    #region 외부 호출 함수
    public void SetTargetPos(Vector3 pos)
    {
        if (_agent == null) { return; }

        _agent.SetDestination(pos);
    }
    public void StopMove()
    {
        if (_agent == null) { return; }

        _agent.velocity = Vector3.zero;
        _agent.ResetPath();
    }
    public bool TargetArrival()
    {
        if (_agent == null) { return false; }
        if (_agent.pathPending) { return false; }

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
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
