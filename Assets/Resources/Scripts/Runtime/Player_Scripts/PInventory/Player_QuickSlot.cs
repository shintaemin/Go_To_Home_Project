using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Player_QuickSlot : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private QuickSlot_UI _weaponSlot;
    [SerializeField] private QuickSlot_UI _throwingSlot;
    #endregion

    #region 내부변수
    private Player_ItemEquip _equipCS;
    #endregion

    private void Awake()
    {
        if (_equipCS == null)
        {
            GUtill.TryGetCS(this, ref _equipCS);
        }
    }

    #region 외부 호출 함수
    public void WeaponQuickSlotEquip()
    {
        if (_equipCS == null || _weaponSlot == null) { return; }

        if (_weaponSlot.GetSlotData() == null) { return; }

        _equipCS.SetBackUpItem(_weaponSlot.GetSlotData());
        _equipCS.ButtonEvent_EquipItem();
    }
    public void ThrowingQuickSlotEquip()
    {
        if (_equipCS == null || _throwingSlot == null) { return; }

        if (_throwingSlot.GetSlotData() == null) { return; }

        _equipCS.SetBackUpItem(_throwingSlot.GetSlotData());
        _equipCS.ButtonEvent_EquipItem();
    }

    #endregion
}
