using Unity.VisualScripting;
using UnityEngine;

#region 무기 데이터 SO
/*
 ▶ 할일
  - 무기 아이템이 같는 기본 데이터
*/
#endregion

public enum EWeaponType
{
    None = 0,
    Blunt,
    Axe,
    Hand,
}

public enum EAttackSoundType
{
    None = 0,
    Heavy,
    Sharp,
    Blunt,
}

public class WeaponDataSO : ItemDataSO
{
    [Header("시트에서 가져올 데이터")]
    [SerializeField] private EWeaponType _weapontype;
    [SerializeField] private int _damage;
    [SerializeField] private float _attackSpeedRatio;
    [SerializeField] private float _maxDurability;
    [SerializeField] private float _attackCost;
    [SerializeField] private EAttackSoundType _attackSoundType;

    [Header("업데이트 될 데이터")]
    [SerializeField] private float _currentDurability = 0.0f;

    #region 외부 호출 함수
    public EWeaponType WeaponType => _weapontype;
    public int Damage => _damage;
    public float AttackSpeedRatio => _attackSpeedRatio;
    public float MaxDur => _maxDurability;
    public float CurrentDur => _currentDurability;
    public float AttackCost => _attackCost;
    public EAttackSoundType AttackSoundType => _attackSoundType;

    public void DecreaseDur(float value)
    {
        float current = _currentDurability - value;
        _currentDurability = Mathf.Max(0, current);
    }
    
    public void Repair()
    {
        _currentDurability = _maxDurability;
    }
    #endregion
}
