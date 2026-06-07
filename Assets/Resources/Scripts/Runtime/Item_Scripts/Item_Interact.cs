using UnityEngine;

#region

#endregion


public class Item_Interact : MonoBehaviour, IInteract
{
    #region 인스펙터
    [SerializeField] private ItemDataSO _item;
    [SerializeField] private SlotData _slot;
    [SerializeField] private int _interactLayerNum = 30;
    #endregion

    private void Start()
    {
        if (ItemDataManager.Instance != null)
        {
            ItemDataSO item = ItemDataManager.Instance.GetItem(gameObject);
            if (item != null)
            {
                _item = item;
            }
        }
        if ((_slot == null ||_slot.GetItem == null) && _item != null)
        {
            SlotData slot = new SlotData();
            slot.SetItem(_item, 0, 1, -1, 0);
            SetSlotData(slot, true);
        }
    }

    #region 외부 호출 함수
    public void SetSlotData(SlotData slot, bool field = false)
    {
        _slot = new SlotData();
        ItemDataSO item = slot.GetItem;
        int dur = slot.Dur;
        int count = slot.Count;
        float cooldown = slot.GetCoolEndTime;

        _slot.SetItem(item, 0, count, dur, cooldown);
        if (field)
        {
            this.gameObject.layer = _interactLayerNum;
        }
        else
        {
            this.gameObject.layer = 0;
        }
    }
    public void Interact()
    {
        if (Inventory_Manager.Instance != null && _slot != null)
        {
            int index = Inventory_Manager.Instance.ProvidedID();
            if (index > -1)
            {
                if(Inventory_Manager.Instance.AddItem(_slot, index))
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public string NameText()
    {
        return _slot.GetItem.Name;
    }
    public string ViewText()
    {
        return "[F] : 줍 기";
    }
    #endregion
}
