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
    private Player_Controller _controllCS;
    #endregion

    private void Awake()
    {
        if (_controllCS == null)
        {
            _controllCS = FindFirstObjectByType<Player_Controller>();
        }
    }

    #region 외부 호출 함수
    public void TryAttack()
    {
        _anim.SetTreggerAttack();
        _controllCS.MovementState = EMovementState.Attack;

        if (_log)
        {
            Debug.Log($"[{this.name}] : 공격 시작!");
        }
    }

    /// <summary>
    /// 애니메이션 마지막 프레임에 호출할 함수
    /// </summary>
    public void EndAttack()
    {
        _controllCS.MovementState = EMovementState.Idle;

        if (_log)
        {
            Debug.Log($"[{this.name}] : 공격 종료");
        }
    }
    #endregion
}
