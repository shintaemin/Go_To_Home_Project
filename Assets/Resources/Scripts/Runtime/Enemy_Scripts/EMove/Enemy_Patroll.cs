using UnityEngine;
using UnityEngine.AI;

#region

#endregion


public class Enemy_Patroll : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private float _patrollRadius = 10.0f; // 순찰 반경

    [SerializeField] private float _patrollInterval = 10.0f;
    #endregion

    #region 내부 변수
    private float _nextPatrollTime;
    private bool _isPatrollActive;
    private Vector3 _patrollPos;
    #endregion

    private Vector3 GetRandomPatrollPos()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _patrollRadius;
        randomDirection += transform.position;

        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _patrollRadius, walkableMask))
        {
            return hit.position;
        }

        return transform.position;
    }
    #region 외부 호출 함수
    public void PatrollMoveActive(bool active)
    {
        if (_isPatrollActive == active) { return; }

        _isPatrollActive = active;
        if (_isPatrollActive)
        {
            _nextPatrollTime = Time.time;
        }
    }
    public void PatrollUpdate()
    {
        if (!_isPatrollActive) { return; }
        if (Time.time <= _nextPatrollTime) { return; }

        _patrollPos = GetRandomPatrollPos();
        _nextPatrollTime = Time.time + _patrollInterval;
    }
    public void PatrollWaitTimeUpdate()
    {
        _nextPatrollTime = Time.time + _patrollInterval;
    }
    public Vector3 PatrollPos()
    {
        return _patrollPos;
    }
    #endregion
}
