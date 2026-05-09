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
    [SerializeField] private ItemDataSO _item;
    [SerializeField] private int _slotId;
    [SerializeField] private float _curentDur;
    [SerializeField] private int _count;

    #region 외부 호출 함수
    public ItemDataSO GetItem => _item;
    public int GetId => _slotId;
    public float GetDur => _curentDur;
    public int GetCount => _count;

    public int AddCount(int value = 1)
    {
        if (_item == null) { return -1; }
        if (!_item.IsStackable) { return -1; }

        int remain = _count;
        int amount = value;
        int result = remain + amount;
        int max = _item.MaxStack;
        int rest = 0;

        if (result > max)
        {
            rest = result - max;
            _count = max;
        }
        else
        {
            rest = 0;
            _count = result;
        }

        return rest;
    }

    public void DecreseCount(int count)
    {
        if (count > _count)
        {
            count = _count;
        }

        _count -= count;
        if (_count < 0) { _count = 0; }
    }

    public void InitItem(ItemDataSO item, int id, int count = 1)
    {
        if (item is WeaponDataSO weapon)
        {
            _curentDur = weapon.MaxDur;
        }

        _count = count;
        _item = item;
        _slotId = id;
    }

    public void SetDur(float value)
    {
        _curentDur = Mathf.Clamp(value, 0, 100);
    }

    public void DecreaseDur(float value)
    {
        float current = _curentDur - value;
        _curentDur = Mathf.Max(0, current);
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
