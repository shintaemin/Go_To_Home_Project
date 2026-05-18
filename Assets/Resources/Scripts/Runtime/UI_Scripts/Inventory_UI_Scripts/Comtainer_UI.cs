using System.Collections.Generic;
using UnityEngine;

#region 컨테이너 UI
/*
 ▶ 할일
  - 게임시작시 UI에 미리 SlotPrefab 을 셋팅 해두기
  - 외부에서 Root On / Off + 보여줄 아이템 갯수 까지 지정
  - 외부에서 SlotUI 에 아이템, 카운트 , dur 을 지정 하도록
*/
#endregion


public class Comtainer_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private List<Slot_UI> _slotUIList;
    [SerializeField] private bool _isActive;

    [Header("옵션")]
    [SerializeField] private GameObject _slotUIRoot;
    [SerializeField] private Transform _slotGrid;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private int _slotMax = 5;
    #endregion

    #region 내부 변수
    private Container_Anim _containerAnim;
    #endregion

    private void Awake()
    {
        if (_slotUIRoot == null)
        {
            _slotUIRoot = transform.GetChild(0).gameObject;
        }

        if (_slotGrid == null)
        {
            _slotGrid = _slotUIRoot?.transform.GetChild(1).transform;
        }

        if (_slotPrefab == null)
        {
            _slotPrefab = Resources.Load<GameObject>("Prefabs/Inventory_Prefabs/Slot_Obj");
        }

        InitSlotUI();
    }

    private void InitSlotUI()
    {
        Active(true);

        int length = _slotMax;
        for (int i = 0; i < length; i++)
        {
            GameObject slotObj = Instantiate(_slotPrefab);
            if (slotObj == null) { continue; }

            Slot_UI slotUI = null;
            Transform root = _slotGrid;
            slotObj.transform.SetParent(root);
            slotObj.transform.localScale = Vector3.one;

            GUtill.TryGetCS(slotObj, ref slotUI);
            if (slotUI != null)
            {
                SlotData slotData = new SlotData();
                slotUI.Data = slotData;
                slotUI.Index = i;
                slotUI.PathType = ESlotPathType.Container;
                _slotUIList.Add(slotUI);
            }
        }

        Active(false);
    }

    #region 외부 호출 함수
    public bool IsActive => _isActive;

    public void Active (bool active)
    {
        if (_containerAnim == null && UI_SlotMove_Manager.Instance != null)
        {
            Interact_Container conta = UI_SlotMove_Manager.Instance.GetContainer;
            GameObject go = conta != null ? conta.gameObject : null;
            if (go != null)
            {
                GUtill.TryGetCS(go, ref _containerAnim);
            }
        }

        _isActive = active;
        _slotUIRoot.SetActive(_isActive);
        if (_containerAnim != null)
        {
            _containerAnim.TreggerToggle(_isActive);
            if (!_isActive) { _containerAnim = null; }
        }
    }

    public void SetSlotUI(int index, SlotData slot)
    {
        if (_slotUIList[index] == null) 
        {
            GUtill.Log($"[{this.name}] : 현재 슬롯 비어있음");
            return; 
        }

        _slotUIList[index].Data = slot;
    }

    public void AllUpdata(List<SlotData> slotList)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            if (i < slotList.Count)
            {
                _slotUIList[i].gameObject.SetActive(true);
                _slotUIList[i].Data = slotList[i];
            }
            else
            {
                _slotUIList[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion
}
