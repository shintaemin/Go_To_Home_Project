using UnityEngine;

#region 던지기 가능 아이템 SO
/*
 ▶ 할일
  - 던질 수 있는 아이템의 기본 데이터
*/
#endregion

public enum EThrowingSoundType
{
    None = 0,
    Brick,
    Glass,
}

public class SoundItemSO : AvailableDataSO
{
    #region 인스펙터
    [SerializeField] private EThrowingSoundType _trowingSoundType;
    [SerializeField] private bool _isThrowingable;
    [SerializeField] private float _soundRange;
    [SerializeField] private float _throwDistance;
    #endregion

    #region 파싱시 셋업 함수
    public void SetUp
        (
        int id,
        string name,
        EItemType itemType,
        int isInteract,
        int isStackable,
        int isEquipable,
        int maxStackCount,
        Sprite icon,
        string info,
        EAvailableType availableType,
        EThrowingSoundType trowingSoundType,
        int isThrowingable,
        float soundRange,
        float throwDistance,
        GameObject prefab
        )
    {
        base.SetUp(id, name, itemType, isInteract, isStackable, isEquipable, maxStackCount, icon, info, availableType, prefab);
        _trowingSoundType = trowingSoundType;
        _isThrowingable = isThrowingable == 1;
        _soundRange = soundRange;
        _throwDistance = throwDistance;
    }
    #endregion
    #region 외부 호출 함수
    public EThrowingSoundType ThrowingSoundType => _trowingSoundType;
    public float SoundRange => _soundRange;
    public bool IsThrowingable => _isThrowingable;
    public float ThrowDistance => _throwDistance;

    public override void Use(GameObject obj)
    {
        /*
         추후 필요시 구현 예정 현재 구조적으로 사용하지 않을것으로 추정
         만약 사용시 오브젝트 전달하여 해당위치에 생성하고 손에들게하는 로직과 연결 예정
         */
    }
    #endregion
}
