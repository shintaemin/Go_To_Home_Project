using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    #region 인스펙터
    [SerializeField] private Inventory_UI _invenUI;
    [SerializeField] private Comtainer_UI _containerUI;
    [SerializeField] private Interact_UI _interactUI;
    [SerializeField] private ESC_UI _escUI;
    #endregion

    #region 내부 변수
    #endregion

    private void Awake()
    {
        if (Instance  != null && Instance != this)
        {
            Destroy(this.gameObject);
            enabled = false;
            return;
        }
        Instance = this;

        if (_invenUI == null) { _invenUI = GetComponentInChildren<Inventory_UI>(); }
        if (_containerUI == null) { _containerUI = GetComponentInChildren<Comtainer_UI>(); }
        if (_interactUI == null) { _interactUI = GetComponentInChildren<Interact_UI>(); }
        if (_escUI == null) { _escUI = GetComponentInChildren<ESC_UI>(); }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    #region 외부 호출 함수
    public void InitInventoryUI(List<SlotData> slots)
    {
        if (_invenUI == null) { return; }

        _invenUI.InitSlotUI(slots);
    }

    public void InventoryUpdate(List<SlotData> slotList)
    {
        if (_invenUI == null) { return; }

        _invenUI.InventoryAllUpdate(slotList);
    }

    public void InventorySlotUpdate(int index, SlotData slot)
    {
        if (_invenUI == null) { return; }

        _invenUI.SlotUpdate(index, slot);
    }

    public void InventoryActive(bool active)
    {
        if (_invenUI == null) { return; }

        _invenUI.Active(active);
    }

    public void CurrentSlotUIUpdate(SlotData slot)
    {
        if (_invenUI == null) { return; }

        _invenUI.CurrentSlotUIUpdate(slot);
    }

    public void ContainerActive(bool active)
    {
        if (_invenUI == null || _containerUI == null) { return; }

        _containerUI.Active(active);
    }

    public void OpenInteractUI(Transform tr, string name, string viewStr)
    {
        if (_interactUI == null) { return; }

        _interactUI.SetActiveInteractView(true, name, viewStr);
        _interactUI.SetTarget(tr);
    }
    public void CloseInteractUI()
    {
        if (_interactUI == null) { return; }

        _interactUI.SetTarget(null);
        _interactUI.SetActiveInteractView(false);
    }

    public void InteractBoolActive(bool active, string name, string viewStr)
    {
        if (_interactUI == null) { return; }

        _interactUI.SetActiveInteractView(active, name, viewStr);
    }

    public void EscInputActive(bool ending = false)
    {
        if (_invenUI == null || _escUI == null) { return; }
        CloseInteractUI();

        if (_invenUI.IsInvenActive)
        {
            _invenUI.OnClickCloseButton();
            return;
        }

        _escUI.ActiveESCUI(!_escUI.IsActiveESCUI, ending);
    }

    public bool ContainerIsActive => _containerUI.IsActive;
    #endregion

}
