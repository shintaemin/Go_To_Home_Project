using System;
using UnityEngine;

#region 플레이어 체력
/*
 ▶ 할일
  - 플레이어 체력을 증감 및 이벤트 발행 관리
*/
#endregion

public class Player_Health : MonoBehaviour
{
    #region 내부 변수
    private Player_DataSO _data;
    #endregion

    #region 이벤트
    public event Action OnHit;
    public event Action OnDead;
    #endregion

    private void Start()
    {
        if (_data == null)
        {
            if (Player_DataManager.Instance != null)
            {
                _data = Player_DataManager.Instance.GetDataSO;
            }
        }
    }

    private void Update()
    {
        // 테스트용
        if(Input.GetKeyDown(KeyCode.P))
        {
            AddHP(_data.GetMaxHP);
        }
    }

    #region 외부 호출 함수
    public void TakeDamage(int damage)
    {
        if (_data == null)
        {
            if (Player_DataManager.Instance != null)
            {
                _data = Player_DataManager.Instance.GetDataSO;
            }
        }
        if (_data.HP == 0) { return; }

        int currentHP = _data.HP;
        currentHP = Mathf.Max(currentHP - damage, 0);
        _data.HP = currentHP;

        GUtill.Log($"[{this.name}] : 체력 감소 현재 : [{_data.HP}]");
        if (currentHP <= 0)
        {
            OnDead?.Invoke();
        }
        else
        {
            OnHit?.Invoke();
        }
    }
    public void AddHP(int hp)
    {
        if (_data == null)
        {
            if (Player_DataManager.Instance != null)
            {
                _data = Player_DataManager.Instance.GetDataSO;
            }
        }
        if (_data.HP == _data.GetMaxHP) { return; }

        int currentHP = _data.HP;
        currentHP = Mathf.Min(currentHP + hp, _data.GetMaxHP);
        _data.HP = currentHP;

        GUtill.Log($"[{this.name}] : 체력 증가 현재 : [{_data.HP}]");
        // 체력회복 특정 이벤트 전달이 필요하면 여기서 이벤트 발생 가능
    }
    #endregion
}
