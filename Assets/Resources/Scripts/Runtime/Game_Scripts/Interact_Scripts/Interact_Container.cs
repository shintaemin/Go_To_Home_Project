using System.Collections.Generic;
using Unity.VisualScripting;
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

        _randomLength = Random.Range(_slotMin, _slotMax);
        
        for (int i = 0; i < _randomLength; i++)
        {
            SlotData data = new SlotData();
            ItemDataSO item = ItemDataManager.Instance.GetRandomItem();
            int count = Random.Range(1, item.MaxStack);

            data.InitItem(item, i, count);
            _containerUI.SetSlotUI(i, data);
            _itemList.Add(data);
        }

        _isInit = true;
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

    public void AddItem(SlotData slot)
    {
        
    }

    public void RemoveItem(SlotData slot)
    {

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
