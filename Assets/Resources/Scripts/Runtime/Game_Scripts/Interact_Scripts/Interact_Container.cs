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
    [SerializeField] private List<SlotData> _items;
    [SerializeField] private int _randomLength;

    [Header("옵션")]
    [SerializeField] private int _slotMin = 3;
    [SerializeField] private int _slotMax = 5;
    [SerializeField] private bool _isInit = false;
    #endregion

    #region 내부 변수
    private Comtainer_UI _containerUI;
    private bool _isOpen;
    #endregion

    private void Awake()
    {
        if (_containerUI == null)
        {
            _containerUI = FindFirstObjectByType<Comtainer_UI>();
        }
    }

    private void InitItems()
    {
        if (ItemDataManager.Instance == null) 
        {
            GUtill.Log($"[{this.name}] : 아이템 데이터 매니저 없음");
            return; 
        }

        _items.Capacity = _slotMax;

        int random = Random.Range(_slotMin, _slotMax);
        
        for (int i = 0; i < random; i++)
        {
            SlotData data = new SlotData();
            ItemDataSO item = ItemDataManager.Instance.GetRandomItem();
            int count = Random.Range(1, item.MaxStack);

            data.InitItem(item, i, count);
            _containerUI.SetSlotUI(i, data);
            _items.Add(data);
        }

        _isInit = true;
    }

    #region 외부 호출 함수
    public void Interact()
    {
        if (_isOpen)
        {
            _isOpen = false;
            _containerUI.Active(false);
            _containerUI.AllUpdata(_items);
            return;
        }

        // 여기서 UI매니저 통해서 컨테이너 UI 띄우기
        _containerUI.Active(true);
        _isOpen = true;

        if (!_isInit)
        {
            InitItems();
        }

        // 리스트에 정보를 컨테이너 UI 에 SetUI(icon, count, dur) 을 지정
        _containerUI.AllUpdata(_items);
    }
    #endregion
}
