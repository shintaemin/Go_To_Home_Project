using System;
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

public class Inventory_Manager : MonoBehaviour
{
    public static Inventory_Manager Instance { get; private set; }

    #region 인스펙터
    [SerializeField] private List<SlotData> _itemList;
    [SerializeField] private Player_InventoryAnim _inventoryAnim;

    [Header("옵션")]
    [SerializeField] private int _maxStorege = 24;
    [SerializeField] private int _capacityOverlab = 1;
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

        if (_inventoryAnim == null)
        {
            _inventoryAnim = FindFirstObjectByType<Player_InventoryAnim>();
        }

        _capacityOverlab += _maxStorege;
        _itemList = new List<SlotData>(_capacityOverlab); // 리스트의 최대 범위를 미리 지정
        for (int i = 0; i < _maxStorege; i++)
        {
            _itemList.Add(new SlotData()); // 미리 리스트 채워듬
            _itemList[i].SetItem(null, i, 0);
        }
    }

    private void Start()
    {
        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InitInventoryUI(_itemList);
            GUtill.Log($"[{this.name}] : 인벤토리 초기업데이트 수행");
        }
    }

    // 아이템 리스트 안 빈공간을 찾아 리턴
    private int ProvidedID()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].GetItem == null) { return i; }
        }

        if (_itemList.Count < _maxStorege) { return _itemList.Count; }

        GUtill.Log($"[{this.name}] : 인벤토리 가득참", EDebugType.Warn);
        return -1;
    }

    private void RemoveSlot(int index) // 슬롯 삭제
    {
        if (_itemList[index].GetItem == null) { return; }

        _itemList[index].RemoveItemData();

        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InventorySlotUpdate(index, _itemList[index]);
        }
    }

    #region 외부 호출 함수
    public bool AddItem(SlotData slot, int index) // 아이템 추가 SlotData 를 통한 추가
    {
        ItemDataSO item = slot.GetItem;  // 슬롯의 아이템
        int amount = slot.Count;         // 추가할 슬롯의 아이템 갯수
        bool isStack = item.IsStackable; // 아이템 스택유무
        float dur = slot.Dur;

        if (isStack)
        {
            if (index >= 0 && index < _maxStorege && _itemList[index].GetItem == item)
            {
                amount = _itemList[index].AddCount(amount);

                if (UI_Manager.Instance != null)
                {
                    UI_Manager.Instance.InventorySlotUpdate(index, _itemList[index]);
                }

                if (amount == 0) { return true; }
            }

            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i] == null) { continue; }
                if (_itemList[i].GetItem != item) { continue; }
                
                amount = _itemList[i].AddCount(amount); // 아이템 갯수 추가후 남는 값 반환

                if (UI_Manager.Instance != null)
                {
                    UI_Manager.Instance.InventorySlotUpdate(i, _itemList[i]);
                }

                if (amount == 0) { return true; }// 남은 갯수가 0 이면 함수종료
            }
        }

        if (_itemList[index].GetItem != null) 
        {
            index = ProvidedID();

            if (index == -1) { return false; }
        }

        _itemList[index].SetItem(item, index, amount, dur); // 데이터 할당

        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InventorySlotUpdate(index, _itemList[index]);
        }
        return true;
    } // <- AddItem End

    public SlotData GetSlotData(int index) // 아이디를 통해 슬롯데이터 획득
    {
        if (index < 0 || index >= _itemList.Count) { return null; }

        return _itemList[index];
    }

    public void RemoveSlotData(SlotData data)
    {
        int id = data.Index;

        _itemList[id].RemoveItemData();

        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InventorySlotUpdate(id, _itemList[id]);
        }
    }

    public SlotData MoveSlot(int index, int count = 1) // 아이디와 갯수로 아이템 이동
    {
        SlotData slot = GetSlotData(index); // id 로 슬롯 받아오기

        if (slot == null || slot.GetItem == null) { return null; }

        float duration = slot.Dur;   // 해당 아이템의 내구도 가져오기
        SlotData outSlot = new SlotData(); // 반환시킬 슬롯 생성

        outSlot.SetItem(slot.GetItem, -1, count); // 기존 슬롯 데이터 복사
        outSlot.Dur = duration; // 기존 내구도 할당

        slot.DecreseCount(count); // 기존 슬롯의 아이템 갯수 감소

        if (slot.Count <= 0) // 기존 슬롯 이 0보다 작거나 같다면
        {
            RemoveSlot(index); // 슬롯 삭제
        }

        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InventorySlotUpdate(index, slot);
        }

        return outSlot; // 반환
    }

    public void ResetInventory() // 인벤토리 초기화
    {
        if (_itemList.Count == 0) { return; }

        for (int i = _itemList.Count - 1; i >= 0; i--)
        {
            _itemList[i].RemoveItemData();
        }
    }

    public void TryInventoryOpen()
    {
        if (_inventoryAnim == null) { return; }

        _inventoryAnim.TryInventoryOpen();
    }
    #endregion
}
