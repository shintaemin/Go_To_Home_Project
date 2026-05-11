using System.Collections;
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
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private int _slotMax = 5;
    #endregion

    #region

    #endregion

    private void Awake()
    {
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
            Transform root = _slotUIRoot.transform;
            slotObj.transform.SetParent(root);
            slotObj.transform.localScale = Vector3.one;

            GUtill.TryGetCS(slotObj, ref slotUI);
            if (slotUI != null)
            {
                SlotData slotData = new SlotData();
                slotUI.UpdataSlotUI(slotData);
                slotUI.Index = i;
                _slotUIList.Add(slotUI);
            }
        }

        Active(false);
    }

    #region 외부 호출 함수
    public void Active (bool active)
    {
        _isActive = active;
        _slotUIRoot.SetActive(_isActive);
    }

    public void SetSlotUI(int index, SlotData slot)
    {
        if (_slotUIList[index] == null) { return; }

        _slotUIList[index].UpdataSlotUI(slot);
    }

    public void AllUpdata(List<SlotData> slotList)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            if (i < slotList.Count)
            {
                _slotUIList[i].gameObject.SetActive(true);
                _slotUIList[i].UpdataSlotUI(slotList[i]);
            }
            else
            {
                _slotUIList[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion
}
