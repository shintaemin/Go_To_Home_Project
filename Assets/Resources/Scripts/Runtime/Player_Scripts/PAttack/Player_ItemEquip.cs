using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Player_ItemEquip : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _handTr; // 손 위치
    #endregion

    #region 내부변수
    private GameObject _backupItem;
    private GameObject _currentItem;
    #endregion

    #region 외부 호출 함수
    public void SetBackUpItem(int index)
    {

    }
    #endregion
}
