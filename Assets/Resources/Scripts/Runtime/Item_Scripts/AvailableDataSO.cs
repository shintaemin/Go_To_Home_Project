using UnityEngine;

#region 사용가능 아이템 데이터 SO
/*
 ▶ 할일
  - 사용 가능한 아이템의 기본 정의
*/
#endregion

public enum EAvailableType
{
    None,
    Healing,
}

public abstract class AvailableDataSO : ItemDataSO
{
    [SerializeField] private EAvailableType _availableType;

    #region 파싱 시 호출 함수
    public void SetUp(EAvailableType type) => _availableType = type;
    #endregion

    #region 외부 호출 함수
    public EAvailableType AvailableDataType => _availableType;
    public abstract void Use(GameObject obj);
    #endregion
}
