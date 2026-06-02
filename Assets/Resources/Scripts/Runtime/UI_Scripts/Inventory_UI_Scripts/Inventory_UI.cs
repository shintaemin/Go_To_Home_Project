using System.Collections.Generic;
using UnityEngine;

#region 인벤토리 UI
/*
 ▶ 할일
  - 인벤토리 UI On / Off
  - 자식 오브젝트에 들어갈 Slot 정보들을 불어오도록 해야하므로 이곳에서 함수 호출을 예정
*/
#endregion

public class Inventory_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private List<Slot_UI> _slotList;
    [SerializeField] private GameObject _inventoryRoot;
    [SerializeField] private GameObject _currentSlot_Root;
    [SerializeField] private bool _isActive = false;    // 테스트 및 정보 매칭 확인용

    [Header("생성 시킬 슬롯 프리펩")]
    [SerializeField] private GameObject _slotPrefab;

    [Header("인벤토리 슬롯을 생성시킬 위치")]
    [SerializeField] private Transform _slotGrid;
    #endregion

    #region 내부 변수
    private Player_InventoryAnim _invenAnim;
    private CurrentSlot_UI _currentSlotUICS;
    private Player_ItemEquip _equipCS;
    #endregion

    private void Awake()
    {
        if (_inventoryRoot == null)
        {
            _inventoryRoot = transform.GetChild(0).gameObject;
        }

        if (_slotPrefab == null)
        {
            _slotPrefab = Resources.Load<GameObject>("Prefabs/Inventory_Prefabs/Slot_Obj");
        }

        if (_slotGrid == null)
        {
            _slotGrid = _inventoryRoot?.transform.GetChild(1).transform;
        }

        if (_invenAnim == null)
        {
            _invenAnim = FindFirstObjectByType<Player_InventoryAnim>();
        }

        if (_currentSlot_Root != null)
        {
            GUtill.TryGetCS(_currentSlot_Root, ref _currentSlotUICS);
        }
        if (_equipCS == null)
        {
            _equipCS = FindFirstObjectByType<Player_ItemEquip>();
        }
    }

    #region 외부 호출 함수
    public void InitSlotUI(List<SlotData> slots)
    {
        Active(true);

        int length = slots.Count;
        for (int i = 0; i < length; i++)
        {
            Slot_UI slotui = null;

            GameObject obj = Instantiate(_slotPrefab);
            obj.transform.SetParent(_slotGrid);
            obj.transform.localScale = Vector3.one;

            GUtill.TryGetCS(obj, ref slotui);

            if (slotui != null)
            {
                slotui.Data = slots[i];
                slotui.Index = i;
                slotui.PathType = ESlotPathType.Inventory;
            }
            _slotList.Add(slotui);
        }

        Active(false);
    }

    public void Active(bool active)
    {
        _isActive = active;
        _inventoryRoot.SetActive(_isActive);

        if (_currentSlot_Root != null && _currentSlot_Root.activeSelf) { _currentSlot_Root.SetActive(false); }
    }

    public void OnClickCloseButton()
    {
        if (_invenAnim != null)
        {
            _invenAnim.TryInventoryOpen();
        }

        Active(false);
    }

    public void InventoryAllUpdate(List<SlotData> slotList)
    {
        for (int i = 0; i < _slotList.Count; i++)
        {
            if (_slotList[i] == null) { continue; }

            SlotUpdate(i, slotList[i]);
        }
    }

    public void SlotUpdate(int index, SlotData slot)
    {
        _slotList[index].Data = slot;
        CurrentSlotUIUpdate(slot);
    }

    public void CurrentSlotUIUpdate(SlotData slot)
    {
        if (_currentSlotUICS == null || slot.GetItem == null || _equipCS == null) { return; }

        if (!_currentSlot_Root.activeSelf)
        {
            _currentSlot_Root.SetActive(true);
        }

        ItemDataSO item = slot.GetItem;
        Sprite icon = item != null ? item.Icon : null;
        string name = item != null ? item.Name : string.Empty;
        string info = item != null ? item.Info : string.Empty;
        bool equip = item.IsEquipable;
        if (slot.Count <= 0) 
        { 
            icon = null; 
        }

        _currentSlotUICS.SetButton(equip);
        _currentSlotUICS.SetIcon(icon);
        _currentSlotUICS.SetName(name);
        _currentSlotUICS.SetInfo(info);
        _equipCS.SetBackUpItem(slot);
    }
    #endregion
}
