using UnityEngine;

#region 아이템 데이터 SO
/*
 ▶ 할일
  - 아이템의 공통 데이터를 관리
*/
#endregion

public enum EItemType
{
    None = 0,
    Weapon,
    Available,
}

public class ItemDataSO : ScriptableObject
{
    [Header("시트에서 불러올 데이터")]
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private EItemType _itemType;
    [SerializeField] private bool _isInteractable;
    [SerializeField] private bool _isStackable;
    [SerializeField] private int _maxStackCount;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _info;

    #region 외부 호출 함수
    public void SetUp(int id, string name, EItemType type, int isInteract, int isStackable, int maxStackCount, Sprite icon, string info)
    {
        _id = id;
        _name = name;
        _itemType = type;
        _isInteractable = isInteract == 1;
        _isStackable = isStackable == 1;
        _maxStackCount = _isStackable ? maxStackCount : 1;
        _icon = icon;
        _info = info;
    }
    #endregion

    #region 외부 호출 함수
    public int ID => _id;
    public string Name => _name;
    public EItemType Type => _itemType;
    public bool IsInteractable => _isInteractable;
    public bool IsStackable => _isStackable;
    public int MaxStack => _maxStackCount;
    public Sprite Icon => _icon;
    public string Info => _info;
    #endregion
}
