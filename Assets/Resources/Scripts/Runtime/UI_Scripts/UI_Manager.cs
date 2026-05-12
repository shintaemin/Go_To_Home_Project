using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    #region 인스펙터
    [SerializeField] private Inventory_UI _invenUI;
    [SerializeField] private Comtainer_UI _containerUI;
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
        DontDestroyOnLoad(this.gameObject);

        if (_invenUI == null)
        {
            _invenUI = FindFirstObjectByType<Inventory_UI>();
        }
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

    public void InventoryActive(bool active)
    {
        if (_invenUI == null) { return; }

        _invenUI.Active(active);
    }

    public void ContainerActive(bool active)
    {
        if (_invenUI == null || _containerUI == null) { return; }

        _containerUI.Active(active);
    }

    public bool ContainerIsActive => _containerUI.IsActive;
    #endregion

}
