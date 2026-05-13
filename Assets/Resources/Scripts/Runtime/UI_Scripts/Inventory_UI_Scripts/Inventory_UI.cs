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
    [SerializeField] private bool _isActive = false;    // 테스트 및 정보 매칭 확인용

    [Header("생성 시킬 슬롯 프리펩")]
    [SerializeField] private GameObject _slotPrefab;

    [Header("인벤토리 슬롯을 생성시킬 위치")]
    [SerializeField] private Transform _slotGrid;
    #endregion

    #region 내부 변수
    private Player_InventoryAnim _invenAnim;
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

            _slotList[i].Data = slotList[i];
        }
    }

    public void SlotUpdate(int index, SlotData slot)
    {
        _slotList[index].Data = slot;
    }
    #endregion
}
