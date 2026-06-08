using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class QuickSlot_UI_Root : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _quickSlotRoot;
    #endregion

    private void Awake()
    {
        if (_quickSlotRoot == null)
        {
            _quickSlotRoot = transform.GetChild(0).gameObject;
        }
        SetActiveQuickSlotRoot(false);
    }

    #region 외부 호출 함수
    public void SetActiveQuickSlotRoot(bool active)
    {
        if (_quickSlotRoot == null) { return; }

        _quickSlotRoot.SetActive(active);
    }
    #endregion
}

