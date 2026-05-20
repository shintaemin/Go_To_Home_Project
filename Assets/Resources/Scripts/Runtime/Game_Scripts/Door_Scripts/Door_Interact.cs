using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 문 상호작용
/*
 ▶ 할일
  - 상호작용시 문열기 / 닫기
*/
#endregion


public class Door_Interact : MonoBehaviour, IInteract
{
    #region 인스펙터
    [SerializeField] private Animator _anim;
    [SerializeField] private string _openParam = "tOpening";
    [SerializeField] private string _closeParam = "tClosing";
    [SerializeField] private bool _isOpen = false;

    [Header("옵션")]
    [SerializeField] private bool _startOpen = false;
    #endregion

    #region 내부변수
    private int _openHash;
    private int _closeHash;
    #endregion
    private void Awake()
    {
        if (_anim == null)
        {
            GUtill.TryGetCS(this, ref _anim);
        }
        _openHash = Animator.StringToHash(_openParam);
        _closeHash = Animator.StringToHash(_closeParam);

        if (_startOpen)
        {
            Opening();
        }
    }

    private void Opening()
    {
        if (_anim == null) { return; }

        _anim.SetTrigger(_openHash);
        // 사운드 클립 찾아서 여기서 DoorOpenSound 넣어주기
        _isOpen = true;
    }

    private void Closing()
    {
        if (_anim == null) { return; }

        _anim.SetTrigger(_closeHash);
        // 사운드 클립 찾아서 여기서 DoorCloseSound 넣어주기
        _isOpen = false;
    }

    #region 외부 호출 함수
    public void Interact()
    {
        if (!_isOpen)
        {
            Opening();
        }
        else
        {
            Closing();
        }
    }

    public string ViewText()
    {
        return _isOpen ? "닫기 [F]" : "열기 [F]";
    }
    #endregion
}
