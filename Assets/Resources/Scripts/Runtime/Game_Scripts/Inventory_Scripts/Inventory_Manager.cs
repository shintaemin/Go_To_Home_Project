using System.Collections.Generic;
using UnityEngine;

#region 인벤토리 매니저
/*
  ▶ 할일 
   - 플레이어의 실질적인 인벤토리
   - 아이템 데이터의 이동 과 UI 에 표시하는 목적
   - 우선은 이동을 목표로만하고 이후에 Stack 유무 와 최대갯수 체크하고 등록하는 로직을 추가 예정
   - 감소 도 마찬가지
*/
#endregion

[System.Serializable]
public class SlotData
{
    #region 슬롯 데이터
    /*
     ▶ 할일
      - 인벤토리에 들어오는 아이템을 자체 id 와 무기의경우 내구도를 저장해두기위한 스크립트
    */
    #endregion
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

    public void InitItem(ItemDataSO item, int id,  int count = 1)
    {
        if (item is WeaponDataSO weapon)
        {
            _curentDur = weapon.MaxDur;
        }

        _count = count;
        _item = item;
        _slotId = id;
    }

    public void DecreaseDur(float value)
    {
        float current = _curentDur - value;
        _curentDur = Mathf.Max(0, current);
    }

    public void Repair()
    {
        if (_item is  WeaponDataSO weapon)
        {
            _curentDur = weapon.MaxDur;
        }
    }
    #endregion
}

public class Inventory_Manager : MonoBehaviour
{
    public static Inventory_Manager Instance { get; private set; }
    #region 인스펙터
    [SerializeField] private List<SlotData> _items;

    [Header("옵션")]
    [SerializeField] private int _maxStorege = 20;
    [SerializeField] private int _capacityOverlab = 5;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            enabled = false;
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        _capacityOverlab += _maxStorege;
        _items = new List<SlotData>(_capacityOverlab); // 리스트의 최대 범위를 미리 지정
    }

    private int ProvidedID()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] == null) { return i; }
        }

        if (_items.Count < _maxStorege) { return _items.Count; }

        GUtill.Log($"[{this.name}] : 인벤토리 가득참", EDebugType.Warn);
        return -1;
    }

    #region 외부 호출 함수
    public bool AddItem(ItemDataSO item)
    {
        bool isStack = item.IsStackable;

        if (isStack && _items.Count != 0)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == null) { continue; }
                if (_items[i].GetItem != item) { continue; }
                if (_items[i].GetCount >= item.MaxStack) { continue; }

                int rest = _items[i].AddCount();
                if (rest != 0) { continue; }
                return true;
            }
        }

        int id = ProvidedID();
        if (id == -1) { return false; }

        SlotData slot = new SlotData();
        slot.InitItem(item, id);

        if (id == _items.Count) { _items.Add(slot); }
        else { _items[id] = slot; }

        return true;
    }

    public bool AddItem(SlotData slot)
    {
        ItemDataSO item = slot.GetItem;
        int amount = slot.GetCount;
        int max = item.MaxStack;
        bool isStack = item.IsStackable;
        
        if (isStack)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == null) { continue; }
                if (_items[i].GetItem != item) { continue; }

                amount = _items[i].AddCount(amount);

                if (amount == 0) { return true; }
            }
        }

        int id = ProvidedID();
        if (id == -1) { return false; }

        SlotData newSlot = new SlotData();
        newSlot.InitItem(item, id, amount);

        if (id == _items.Count) { _items.Add(newSlot); }
        else { _items[id] = newSlot; }

        return true;
    }

    public SlotData GetSlotData(int id)
    {
        if (id < 0 || id >= _items.Count) { return null; }

        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (_items[id] == null) { return; }

        _items[id] = null;
    }

    public void ResetInventory()
    {
        if (_items.Count == 0) { return; }

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            _items[i] = null;
        }
    }
    #endregion
}
