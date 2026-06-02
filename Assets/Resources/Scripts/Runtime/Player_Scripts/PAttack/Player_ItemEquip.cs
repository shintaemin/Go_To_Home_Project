using System;
using UnityEngine;

#region 아이템 장착, 사용로직
/*
 ▶ 할일
  - 처음에 구조를 잡을떄 슬롯별로 꺼내쓰는 틀은 잡았지만 디테일하게 고유ID를 두지않아
    슬롯내부 값들을 비교하는 비효율적인 로직이 생겨버림 SlotData에 또 고유ID를 추가하면 변해야하는게많아
    비효율이더라도 사용시점엔 확인하고 감소하는 로직이 추가로 생길듯 함
  
  - 아이템 선택 : 인벤토리에서 선택된 아이템을 예비 아이템으로 등록
  - 아이템 장착 : 등록된 예비 아이템을 손에들개 생성 기존 아이템이있다면 해제
  - 아이템 사용 : 현재 들고있는 아이템과 슬롯내부 데이터를 확인하고 사용 및 감소 하도록 구현

 - 현재 있을 버그
  완벽하게 똑같은 아이템을 2개이상 들고있으면 참조되는 대상이 변경되지만
  같은 아이템이라면 동일한 수치가 감소되므로 유저입장에선 큰 체감을 하기는 힘듬
*/
#endregion


public class Player_ItemEquip : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _handTr; // 손 위치
    [SerializeField] private GameObject _currentItem;
    #endregion

    #region 내부변수
    private SlotData _backupItem;
    private SlotData _currentSlotItem;
    private int _currentSlotIndex;

    private ItemDataSO _equipItem;
    private float _equipItemDur;
    private int _equipItemCount;
    #endregion

    #region 이벤트
    public event Action<SlotData> OnItemEquip; 
    #endregion
    private void Start()
    {
        Player_Attack attack = null;
        Player_Throwing throwing = null;
        GUtill.TryGetCS(this, ref attack);
        GUtill.TryGetCS(this, ref throwing);
        if (attack != null && throwing != null)
        {
            attack.OnSuccessAttack += OnSuccessAttack;
            throwing.OnSuccessThrowing += OnSuccessThrowing;
        }
    }

    private void OnDisable()
    {
        _backupItem = null;
        Player_Attack attack = null;
        Player_Throwing throwing = null;
        GUtill.TryGetCS(this, ref attack);
        GUtill.TryGetCS(this, ref throwing);
        if (attack != null && throwing != null)
        {
            attack.OnSuccessAttack -= OnSuccessAttack;
            throwing.OnSuccessThrowing -= OnSuccessThrowing;
        }
    }

    private SlotData CheckSlotItem(ItemDataSO currentItem, float currentDur, int currentCount = 1)
    {
        int length = Inventory_Manager.Instance.GetInventoryCount();
        for (int i = 0; i < length; i++)
        {
            SlotData slot = Inventory_Manager.Instance.GetSlotData(i);
            if (slot == null) { continue; }

            ItemDataSO item = slot.GetItem;
            if (item == null) { continue; }

            float dur = slot.Dur;
            int count = slot.Count;
            if (currentItem == item && currentDur == dur && currentCount == count)
            {
                return slot;
            }
        }
        return null;
    }
    private void OnSuccessAttack()
    {
        if (_currentSlotItem == null || Inventory_Manager.Instance == null) { return; }

        SlotData currentSlot = Inventory_Manager.Instance.GetSlotData(_currentSlotIndex);
        ItemDataSO currentItem = _currentSlotItem.GetItem;
        float currentDur = _currentSlotItem.Dur;

        if (currentItem != _equipItem || currentDur != _equipItemDur)
        {
            currentSlot = CheckSlotItem(_equipItem, _equipItemDur);
        }

        _currentSlotItem = currentSlot;
        _currentSlotIndex = currentSlot.Index;
        _currentSlotItem.DecreaseDur();

        _equipItem = _currentSlotItem.GetItem;
        _equipItemDur = _currentSlotItem.Dur;

        if (_currentSlotItem.Dur <= 0)
        {
            ReleaseItem();
        }

        GUtill.Log($"[{this.name}] : 내구도 감소 완료 : [{_currentSlotItem.Dur}]");
    }

    private void OnSuccessThrowing()
    {
        if (_currentSlotItem == null || Inventory_Manager.Instance == null) { return; }

        SlotData currentSlot = Inventory_Manager.Instance.GetSlotData(_currentSlotIndex);
        ItemDataSO currentItem = _currentSlotItem.GetItem;
        int currentCount = _currentSlotItem.Count;

        if (currentItem != _equipItem || currentCount != _equipItemCount)
        {
            currentSlot = CheckSlotItem(_equipItem, _equipItemDur, _equipItemCount);
        }

        _currentSlotItem = currentSlot;
        _currentSlotIndex = currentSlot.Index;
        _currentSlotItem.DecreseCount(1);

        _equipItem = _currentSlotItem.GetItem;
        _equipItemCount = _currentSlotItem.Count;

        if (_currentSlotItem.Count <= 0)
        {
            _currentSlotItem.RemoveItemData();
            ReleaseItem();
        }
        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InventorySlotUpdate(_currentSlotItem.Index, _currentSlotItem);
        }

        GUtill.Log($"[{this.name}] : 갯수 감소 완료 : [{_currentSlotItem.Count}]");
    }

    #region 외부 호출 함수
    public void SetBackUpItem(SlotData item)
    {
        if (item == null) { return; }
        if (!item.GetItem.IsEquipable) { return; }

        _backupItem = item;
    }
    public void ButtonEvent_EquipItem()
    {
        if (_backupItem == null || _handTr == null) { return; }

        ItemDataSO item = _backupItem.GetItem;

        if (_currentItem != null)
        {
            ReleaseItem();
        }
        if (item != null)
        {
            _currentSlotItem = _backupItem;
            if (_currentSlotItem.Count <= 0 || (item is WeaponDataSO && _currentSlotItem.Dur <= 0)) { return; }

            GameObject go = Instantiate(item.Prefab);
            go.transform.SetParent(_handTr);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            _currentItem = go;
            _currentSlotIndex = _currentSlotItem.Index;
            if (item is WeaponDataSO weapon && Player_DataManager.Instance != null)
            {
                int attackSteminaCost = weapon.AttackSteminaCost;
                Player_DataSO data = Player_DataManager.Instance.GetDataSO;
                data.AttackSteminaCost = attackSteminaCost;
                data.AttackDamage = weapon.Damage;
            }
            
            _equipItem = item;
            _equipItemDur = _currentSlotItem.Dur;
            _equipItemCount = _currentSlotItem.Count;
            OnItemEquip?.Invoke(_currentSlotItem);
        }
    }
    public void ReleaseItem()
    {
        Destroy(_currentItem);
        _currentItem = null;
        _equipItem = null;
        _equipItemDur = 0;
        _equipItemCount = 0;
    }
    public bool IsAttackable()
    {
        return _currentItem != null && _equipItem is WeaponDataSO;
    }
    public bool IsTrowingable()
    {
        return _currentItem != null && _equipItem is SoundItemSO;
    }
    public SlotData GetEqupiSlot()
    {
        if (_currentSlotItem == null) { return null; }

        return _currentSlotItem;
    }
    #endregion
}
