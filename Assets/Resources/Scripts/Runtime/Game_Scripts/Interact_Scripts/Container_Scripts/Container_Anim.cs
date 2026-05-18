using UnityEngine;

#region 컨테이너 애니메이션
/*
 ▶ 할일
  - 상호작용시 애니메이션 트리거 호출하도록 작업
*/
#endregion


public class Container_Anim : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Animator _anim;
    [SerializeField] private string _openParam = "tOpening";
    [SerializeField] private string _closeParam = "tClosing";
    #endregion

    #region 내부 변수
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

    #region 외부 호출 함수
    public void TreggerToggle(bool toggle) // 인벤토리 OnOff 에서 캐싱해서 사용 예정
    {
        if (_anim == null) { return; }

        int hash = toggle ? _openHash : _closeHash;

        _anim.SetTrigger(hash);
    }
    #endregion
}
