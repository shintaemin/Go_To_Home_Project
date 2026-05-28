using System;
using UnityEngine;

#region 적 체력
/*
 ▶ 할일
  - 내부적으로 체력을 들고있고 외부에서 TakeDamage 호출 체력이 0 보다적어지면 OnDead 이벤트 발행
*/
#endregion


public class Enemy_Health : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private int _startHP = 100;
    #endregion

    #region 내부 변수
    private int _currentHP;
    #endregion

    #region 프로퍼티
    public int HP 
    { 
        get { return _currentHP; }
        set { _currentHP = value; }
    }
    #endregion

    #region 이벤트
    public event Action OnDead;
    #endregion

    private void Start()
    {
        _currentHP = _startHP;
    }

    #region 외부 호출 함수
    public void TakeDamage(int damage)
    {
        HP = Mathf.Max(HP - damage, 0);

        if (HP <= 0)
        {
            OnDead?.Invoke();
        }
    }
    #endregion
}
