using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Player_HeadLight : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _lightObj;
    #endregion

    #region

    #endregion

    #region 외부 호출 함수
    public void SetToogleHeadLight()
    {
        if (_lightObj == null) { return; }

        bool active = !_lightObj.activeSelf;
        _lightObj.SetActive(active);
    }
    #endregion
}
