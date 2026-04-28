using System.Collections;
using UnityEngine;

#region 플레이어 공격
/*
 ▶ 할일
  - 플레이어 공격 처리
  - 인풋 공격 이벤트 구독하고 TryAttack 를 수행 _anim 에 Attack Tregger 활성화
  - EndAttack 를 만들어서 애니메이션 마지막 프레임에 호출되도록 작업

   ※ 현재구조로 선택한 이유 ※
    01. 코루틴을 활용한 시간기반 작업으로 구현했으나 타이밍이 오류나거나 트랜지션전이가 안되거나 애니메이션의 Speed 값이 변경되면 오류가생겼음
    02. 간단한 구조이지만 오류가 적고 변경에있어 작업이 유연해보임
*/
#endregion


public class Player_Attack : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Player_Anim _anim;

    [Header("옵션")]
    [SerializeField] private bool _log = true;
    #endregion

    #region 내부 변수
    private PlayerInputManager _im;
    private Player_State _pState;
    #endregion

    private void Awake()
    {
        if (_anim == null)
        {
            if (!TryGetComponent<Player_Anim>(out _anim))
            {
                Debug.LogWarning($"[{this.name}] : 플레이어 애니메이션 스크립트 캐싱 실패");
                return;
            }
        }
        if (_pState == null)
        {
            if (!TryGetComponent<Player_State>(out _pState))
            {
                Debug.LogWarning($"[{this.name}] : 플레이어 상태 스크립트 캐싱 실패");
                return;
            }
        }
    }

    private void OnEnable()
    {
        if (PlayerInputManager.Instance != null)
        {
            _im = PlayerInputManager.Instance;
            _im.OnAttack += TryAttack;
        }
        else
        {
            if (_log) { Debug.LogWarning($"[{this.name}] : 인풋매니저 생선 전 자동 구독 시작"); }
            StartCoroutine(CoWaitInputManager());
        }
    }

    // 인풋매니저 대기를 위한 코루틴 - 현재는 같은씬에서 생성되어 이 코루틴이 호출되지만 빌드시점에 씬이 나눠질거라서 큰 문제는없음 추후 삭제도 무방함
    private IEnumerator CoWaitInputManager()
    {
        while(true)
        {
            if (PlayerInputManager.Instance != null)
            {
                _im = PlayerInputManager.Instance;
                _im.OnAttack += TryAttack;

                if (_log) { Debug.Log($"[{this.name}] : 공격 이벤트 구독 완료"); }

                yield break;
            }

            yield return null;
        }
    }

    private void TryAttack()
    {
        _anim.SetTreggerAttack();
        _pState.SetState(EPlayerState.Attack);
    }

    #region 외부 호출 함수
    /// <summary>
    /// 애니메이션 마지막 프레임에 호출할 함수
    /// </summary>
    public void EndAttack()
    {
        _pState.SetState(EPlayerState.Idle);
    }
    #endregion
}
