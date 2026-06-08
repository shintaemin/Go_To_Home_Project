using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Cutscene_Cart : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private CineMashine_System _mashin;
    #endregion

    private void Awake()
    {
        if (_mashin == null)
        {
            _mashin = FindFirstObjectByType<CineMashine_System>();
        }
    }

    #region 외부 호출 함수
    public void FrameEvent_SeyMainCam()
    {
        _mashin.CutSceneEnd();
    }
    #endregion
}
