using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Item_Interact : MonoBehaviour, IInteract
{
    #region ÀÎ½ºÆåÅÍ
    [SerializeField] private ItemDataSO _item;
    [SerializeField] private SlotData _slot;
    [SerializeField] private string _viewText = "[F] È¹ µæ";
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

    #region ¿ÜºÎ È£Ãâ ÇÔ¼ö
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

    public string ViewText()
    {
        return _viewText;
    }
    #endregion
}
