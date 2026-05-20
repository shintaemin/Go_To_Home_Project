using UnityEngine;

#region 자동문 스크립트
/*
 ▶ 할일
  - 충돌시 Open 애니메이션 재생 / 충돌종료시 Close 애니메이션 재생 
*/
#endregion


public class AutoDoor_Script : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Animator _anim;
    [SerializeField] private string _colTag = "Player";
    [SerializeField] private string _openParam = "tOpening";
    [SerializeField] private string _closeParam = "tClosing";
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_colTag)) { return; }

        _anim.SetTrigger(_openHash);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(_colTag)) { return; }

        _anim.SetTrigger(_closeHash);
    }
}
