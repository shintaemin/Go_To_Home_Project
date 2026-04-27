using UnityEngine;

#region 플레이어 공격
/*
 ▶ 할일
  - 플레이어 공격 처리
  - 플레이어 입력 매니저의 Attack 이벤트를 구독하고 발생시 코루틴 시작
  - 공격시작시 애니메이션 재생 하고 입력대기 지정된 시간 안에 입력이있다면 콤보공격 수행
  - 애니메이션에 int 와 tregger 를 지정하여 실행
*/
#endregion


public class Player_Attack : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Player_Anim _anim;
    [SerializeField] private int _combo = 1;
    [SerializeField] private bool _comboInput = false;
    #endregion

    #region 내부 변수
    private PlayerInputManager _im;
    #endregion

    private void Awake()
    {
        if (_anim == null)
        {
            if (!TryGetComponent<Player_Anim>(out _anim))
            {
                Debug.LogWarning($"[Player_Attack] : 플레이어 애니메이션 스크립트 캐싱 실패");
                return;
            }
        }
    }

    private void OnEnable()
    {
        if (PlayerInputManager.Instance != null)
        {
            _im.OnAttack += TryAttack;
        }
    }

    private void TryAttack()
    {

    }
}
