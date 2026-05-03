using UnityEngine;

#region 플레이어 상태
/*
 ▶ 할일
  
*/
#endregion

public enum EPlayerState
{
    None,
    Idle,
    Attack,
    Interact,
    Healing,
    Dead,
}

public class Player_State : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private EPlayerState _currentState;

    [Header("옵션")]
    [SerializeField] private EPlayerState _startState = EPlayerState.Idle; // 시작 상태 옵션
    #endregion

    #region 내부변수
    #endregion

    private void Start()
    {
        _currentState = _startState;
    }

    #region 외부 호출 함수
    public void SetState(EPlayerState state, bool inputUpdate = true)
    {
        if (state == _currentState) { return; }

        EPlayerState lastState = _currentState;
        _currentState = state;

        GUtill.Log($"[{this.name}] : 현재 상태 : {_currentState}");
    }
    #endregion
}
