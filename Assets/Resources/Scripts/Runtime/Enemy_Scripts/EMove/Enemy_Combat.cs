using System;
using UnityEngine;

#region

#endregion


public class Enemy_Combat : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private float _attackCoolDown = 2.0f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    [SerializeField] private float _attackDistance = 2.5f;
    [SerializeField] private int _attackDamage = 20;

    [SerializeField] private Transform _target;
    #endregion

    #region 내부 변수
    private float _nextAttackTime;
    private float _lastHitTime;
    private bool _isCombatActive;
    #endregion

    #region 이벤트
    public event Action OnTryAttack;
    #endregion

    #region 외부 호출 함수
    public void CombatActive(bool active, Transform target = null)
    {
        _isCombatActive = active;
        if (_isCombatActive)
        {
            _target = target;
            _nextAttackTime = Time.time;
        }
        else
        {
            _target = target;
        }
    }
    public void CombatUpdate()
    {
        if (_target == null) { return; }

        Vector3 targetPos = _target.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion start = transform.rotation;
            Quaternion end = Quaternion.LookRotation(dir);
            float t = 1.0f - Mathf.Exp(-_rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(start, end, t);
        }

        if (!OutOfTarget(_target) && Time.time >= _nextAttackTime)
        {
            OnTryAttack?.Invoke();
            
            _nextAttackTime = Time.time + _attackCoolDown;
        }
    }
    public bool OutOfTarget(Transform target)
    {
        float dis = Vector3.Distance(transform.position, target.position);
        if (dis >= _attackDistance)
        {
            return true;
        }
        return false;
    }
    public void AnimEvent_AttackHit()
    {
        if (_target == null) return;
        if (Time.time < _lastHitTime + 0.3f) { return; }

        if (!OutOfTarget(_target))
        {
            if (_target.TryGetComponent<Player_Health>(out Player_Health player))
            {
                _lastHitTime = Time.time;

                player.TakeDamage(_attackDamage);
            }
        }
    }
    #endregion

}
