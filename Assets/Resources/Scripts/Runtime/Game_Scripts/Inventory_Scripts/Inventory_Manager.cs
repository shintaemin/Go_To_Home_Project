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

    public void DecreseCount(int count)
    {
        if (count > _count)
        {
            count = _count;
        }

        _count -= count;
        if (_count < 0) { _count = 0; }
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
    public bool AddItem(ItemDataSO item) // 아이템 추가 ItemDataSO 를 통한 추가
    {
        bool isStack = item.IsStackable;

        if (isStack && _items.Count != 0) // 스택 유무와 아이템 리스트 요소 확인
        {   
            // 같은 item 슬롯 찾기
            for (int i = 0; i < _items.Count; i++)
            {   // 슬롯이 없거나 아이템 이 다르거나 최대스택갯수보다 크거나 같으면 다음 반복
                if (_items[i] == null) { continue; }
                if (_items[i].GetItem != item) { continue; }
                if (_items[i].GetCount >= item.MaxStack) { continue; }

                int rest = _items[i].AddCount(); // 아이템을 추가하고 남는값을 반환
                if (rest != 0) { continue; }    // 남은값이 있다면 continue;
                return true;                    // 정상적으로 스택되면 함수 종료
            }
        }

        int id = ProvidedID(); // 들어갈수 있는 공간 체크
        if (id == -1) { return false; }

        SlotData slot = new SlotData(); // 새로운 slot 생성
        slot.InitItem(item, id); // 아이템 설정

        // id 와 리스트의 갯수가 같으면 추가
        if (id == _items.Count) { _items.Add(slot); }
        else { _items[id] = slot; } // 그외 슬롯 할당

        return true;
    }

    public bool AddItem(SlotData slot) // 아이템 추가 SlotData 를 통한 추가
    {
        ItemDataSO item = slot.GetItem; // 슬롯의 아이템
        int amount = slot.GetCount;     // 추가할 슬롯의 아이템 갯수
        bool isStack = item.IsStackable; // 아이템 스택유무
        
        if (isStack)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == null) { continue; }
                if (_items[i].GetItem != item) { continue; }
                
                amount = _items[i].AddCount(amount); // 아이템 갯수 추가후 남는 값 반환

                if (amount == 0) { return true; } // 남은 갯수가 0 이면 함수종료
            }
        }

        int id = ProvidedID(); // 빈 인덱스 찾기
        if (id == -1) { return false; }

        SlotData newSlot = new SlotData(); // 추가할 슬롯 생성
        newSlot.InitItem(item, id, amount); // 데이터 할당

        if (id == _items.Count) { _items.Add(newSlot); }
        else { _items[id] = newSlot; }

        return true;
    }

    public SlotData GetSlotData(int id) // 아이디를 통해 슬롯데이터 획득
    {
        if (id < 0 || id >= _items.Count) { return null; }

        return _items[id];
    }

    public SlotData MoveSlot(int id, int count = 1) // 아이디와 갯수로 아이템 이동
    {
        SlotData slot = GetSlotData(id); // id 로 슬롯 받아오기

        if (slot == null) { return null; }

        float duration = slot.GetDur;   // 해당 아이템의 내구도 가져오기
        SlotData outSlot = new SlotData(); // 반환시킬 슬롯 생성

        outSlot.InitItem(slot.GetItem, -1, count); // 기존 슬롯 데이터 복사
        outSlot.SetDur(duration); // 기존 내구도 할당

        slot.DecreseCount(count); // 기존 슬롯의 아이템 갯수 감소

        if (slot.GetCount <= 0) // 기존 슬롯 이 0보다 작거나 같다면
        {
            RemoveSlot(id); // 슬롯 삭제
        }

        return outSlot; // 반환
    }

    public void RemoveSlot(int id) // 슬롯 삭제
    {
        if (_items[id] == null) { return; }

        _items[id] = null;
    }

    public void ResetInventory() // 인벤토리 초기화
    {
        if (_items.Count == 0) { return; }

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            _items[i] = null;
        }
    }
    #endregion
}
