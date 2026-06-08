using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Tutorial_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _tutorialRoot;
    #endregion

    #region

    #endregion
    private void Awake()
    {
        if (_tutorialRoot == null)
        {
            _tutorialRoot = transform.GetChild(0).gameObject;
        }
    }

    #region 외부 호출 함수
    public void ButtonEvent_TutorialUIActive()
    {
        bool remain = _tutorialRoot.activeSelf;
        _tutorialRoot.SetActive(!remain);
    }
    public void ButtonEvent_TutorialUIClose()
    {
        if (!_tutorialRoot.activeSelf) { return; }

        _tutorialRoot.SetActive(false);
    }
    #endregion
}
