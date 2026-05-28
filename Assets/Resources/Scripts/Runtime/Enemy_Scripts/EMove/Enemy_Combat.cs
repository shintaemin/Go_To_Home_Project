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

    #endregion

    #region 이벤트
    public event Action OnAttack;
    #endregion

    #region 외부 호출 함수
    public void CombatActive()
    {

    }
    public void CombatUpdate()
    {

    }
    #endregion

}
