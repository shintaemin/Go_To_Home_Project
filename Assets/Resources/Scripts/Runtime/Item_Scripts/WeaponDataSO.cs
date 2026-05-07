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

    #region 파싱시 셋업 함수
    public void SetUp
        (
        int id,
        string name,
        EItemType itemType,
        int isInteract,
        int isStackable,
        int maxStackCount,
        Sprite icon,
        string info,
        EWeaponType weaponType,
        int damage,
        float speed,
        float dur,
        float cost,
        EAttackSoundType soundType,
        GameObject prefab
        )
    {
        base.SetUp(id, name, itemType, isInteract, isStackable, maxStackCount, icon, info, prefab);

        _weapontype = weaponType;
        _damage = damage;
        _attackSpeedRatio = speed;
        _maxDurability = dur;
        _attackCost = cost;
        _attackSoundType = soundType;
    }
    #endregion

    #region 외부 호출 함수
    public EWeaponType WeaponType => _weapontype;
    public int Damage => _damage;
    public float AttackSpeedRatio => _attackSpeedRatio;
    public float MaxDur => _maxDurability;
    public float AttackCost => _attackCost;
    public EAttackSoundType AttackSoundType => _attackSoundType;
    #endregion
}
