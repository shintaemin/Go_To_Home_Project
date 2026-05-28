using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Enemy_Combat : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private float _attackCoolDown = 2.0f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    [SerializeField] private float _attackDistance = 2.5f;
    #endregion

    #region 내부 변수
    private float _nextAttackTime;
    private bool _isCombatActive;
    #endregion

    #region 이벤트
    public event Action OnTryAttack;
    #endregion

    #region 외부 호출 함수
    public void CombatActive(bool active)
    {
        _isCombatActive = active;
        if (_isCombatActive)
        {
            _nextAttackTime = Time.time;
        }
    }
    public void CombatUpdate(Transform target)
    {
        Vector3 targetPos = target.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion start = transform.rotation;
            Quaternion end = Quaternion.LookRotation(dir);
            float t = 1.0f - Mathf.Exp(-_rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(start, end, t);
        }

        if (!OutOfTarget(target) && Time.time >= _nextAttackTime)
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
    #endregion

}
