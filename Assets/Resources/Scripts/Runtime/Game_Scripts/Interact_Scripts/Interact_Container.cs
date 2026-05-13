using System;
using System.Collections.Generic;
using UnityEngine;

#region 컨테이너
/*
 ▶ 할일
  - 컨테이너 상호작용시 아이템, 슬롯 생성
*/
#endregion


public class Interact_Container : MonoBehaviour, IInteract
{
    #region 인스펙터
    [SerializeField] private List<SlotData> _itemList;
    [SerializeField] private int _randomLength;

    [Header("옵션")]
    [SerializeField] private int _slotMin = 3;
    [SerializeField] private int _slotMax = 5;
    [SerializeField] private bool _isInit = false;
    #endregion

    #region 내부 변수
    private Comtainer_UI _containerUI;
    private Inventory_Manager _inventoryManager;
    private UI_SlotMove_Manager _slotMoveManager;
    #endregion

    private void Awake()
    {
        if (_containerUI == null)
        {
            _containerUI = FindFirstObjectByType<Comtainer_UI>();
        }
    }

    private void Start()
    {
        if (_inventoryManager == null && Inventory_Manager.Instance != null)
        {
            _inventoryManager = Inventory_Manager.Instance;
        }
    }
    private void InitItems()
    {
        if (ItemDataManager.Instance == null) 
        {
            GUtill.Log($"[{this.name}] : 아이템 데이터 매니저 없음");
            return; 
        }

        _itemList.Capacity = _slotMax;

        _randomLength = UnityEngine.Random.Range(_slotMin, _slotMax);
        
        for (int i = 0; i < _randomLength; i++)
        {
            SlotData data = new SlotData();
            ItemDataSO item = ItemDataManager.Instance.GetRandomItem();
            int count = UnityEngine.Random.Range(1, item.MaxStack);

            data.SetItem(item, i, count);
            _containerUI.SetSlotUI(i, data);
            _itemList.Add(data);
        }

        _isInit = true;
    }
    private int ProvidedID()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].GetItem == null) { return i; }
        }

        if (_itemList.Count < _randomLength) { return _itemList.Count; }

        GUtill.Log($"[{this.name}] : 인벤토리 가득참", EDebugType.Warn);
        return -1;
    }

    #region 외부 호출 함수
    public void Interact()
    {
        if (UI_Manager.Instance == null) { return; }
        if (UI_SlotMove_Manager.Instance == null) { return; }
        if (UI_Manager.Instance.ContainerIsActive) { return; }

        if (_inventoryManager == null) { _inventoryManager = Inventory_Manager.Instance; }
        if (_slotMoveManager == null) { _slotMoveManager = UI_SlotMove_Manager.Instance; }

        _inventoryManager.TryInventoryOpen();
        _slotMoveManager.SetContainer(this);
        // 여기서 UI매니저 통해서 컨테이너 UI 띄우기
        _containerUI.Active(true);

        if (!_isInit)
        {
            InitItems();
        }

        // 리스트에 정보를 컨테이너 UI 에 Update
        _containerUI.AllUpdata(_itemList);
    }

    public bool AddItem(SlotData slot, int index)
    {
        ItemDataSO item = slot.GetItem; // 슬롯의 아이템
        int amount = slot.Count;     // 추가할 슬롯의 아이템 갯수
        bool isStack = item.IsStackable; // 아이템 스택유무
        float dur = slot.Dur;

        if (isStack)
        {
            if (index >= 0 && index < _randomLength && _itemList[index].GetItem == item)
            {
                amount = _itemList[index].AddCount(amount);

                _containerUI.AllUpdata(_itemList);

                if (amount == 0) { return true; }
            }

            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i] == null) { continue; }
                if (_itemList[i].GetItem != item) { continue; }

                amount = _itemList[i].AddCount(amount); // 아이템 갯수 추가후 남는 값 반환

                _containerUI.AllUpdata(_itemList);

                if (amount == 0) { return true; }// 남은 갯수가 0 이면 함수종료
            }
        }

        if (_itemList[index].GetItem != null)
        {
            index = ProvidedID();

            if (index == -1) { return false; }
        }

        _itemList[index].SetItem(item, index, amount, dur); // 데이터 할당

        _containerUI.AllUpdata(_itemList);
        return true;
    }

    public void RemoveItem(SlotData slot)
    {
        int id = slot.Index;

        _itemList[id].RemoveItemData();

        _containerUI.AllUpdata(_itemList);
    }

    public int ProvidedIndex()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].GetItem != null) { continue; }

            return i;
        }

        if (_itemList.Count < _slotMax) { return _itemList.Count; }

        GUtill.Log($"[{this.name}] : 루팅 상자 가득참", EDebugType.Warn);
        return -1;
    }

    public List<SlotData> GetItemList => _itemList;
    #endregion
}
