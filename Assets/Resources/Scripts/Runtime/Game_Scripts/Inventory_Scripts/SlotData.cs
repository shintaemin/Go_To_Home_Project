using UnityEngine;

#region 슬롯 데이터
/*
 ▶ 할일
  - 인벤토리에 들어오는 아이템을 자체 id 와 무기의경우 내구도를 저장해두기위한 스크립트
*/
#endregion

[System.Serializable]
public class SlotData
{
    #region 인스펙터
    [SerializeField] private ItemDataSO _item;
    [SerializeField] private int _slotIndex;
    [SerializeField] private float _curentDur;
    [SerializeField] private int _count;
    #endregion

    #region 프로퍼티
    public int Index
    {
        get { return _slotIndex; }
        set { _slotIndex = value; }
    }

    public float Dur
    {
        get { return _curentDur; }
        set { _curentDur = Mathf.Clamp(value, 0, 100); }
    }

    public int Count
    {
        get { return _count; }
        set { _count = value; }
    }
    #endregion

    #region 외부 호출 함수
    public ItemDataSO GetItem => _item;

    public int AddCount(int value = 1)
    {
        if (_item == null) { return -1; }
        if (!_item.IsStackable) { return -1; }

        int remain = Count;
        int amount = value;
        int result = remain + amount;
        int max = _item.MaxStack;
        int rest = 0;

        if (result > max)
        {
            rest = result - max;
            Count = max;
        }
        else
        {
            rest = 0;
            Count = result;
        }

        return rest;
    }

    public void RemoveItemData()
    {
        _item = null;
        _count = 0;
        _curentDur = 0;
    }

    public void DecreseCount(int count)
    {
        if (count > Count)
        {
            count = Count;
        }

        Count -= count;
        if (Count < 0) { Count = 0; }
    }

    public void SetItem(ItemDataSO item, int index, int count = 0, float dur = -0.1f)
    {
        if (item is WeaponDataSO weapon)
        {
            _curentDur = dur == -0.1f ? weapon.MaxDur : dur;
        }

        _item = item;
        _count = count != 0  ? count : 0;
        _slotIndex = index;
    }

    public void DecreaseDur(float value)
    {
        if (value > 100)
        {
            value = 100;
        }

        float current = Dur - value;
        Dur = Mathf.Max(0, current);
    }

    public void Repair()
    {
        if (_item is WeaponDataSO weapon)
        {
            _curentDur = weapon.MaxDur;
        }
    }
    #endregion
}
