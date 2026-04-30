using System;
using UnityEngine;

#region 플레이어 스테미너
/*
 ▶ 할일
  - 스테미너 감소 상태를 관리
  - 느리게 걷기, 달리기, 공격 등을 하면 스테미너가 감소하도록
  - 느리게 걷기 나 달리기는 유지되는 상태이기때문에 일정 시간을 채웠을경우 스테미너를 감소
  - 공격은 일정 수치 감소
*/
#endregion


public class Player_Stemina : MonoBehaviour
{
    public enum ESteminaState
    {
        None = 0,
        Idle,
        Decrese,
        Attack,
    }

    #region 인스펙터
    [Header("상태")]
    [SerializeField] private ESteminaState _state;
    [SerializeField] private float _decreaseStateTime;
    [SerializeField] private float _idleStateTime;

    [Header("옵션")]
    [SerializeField] private float _decreaseCool = 0.3f;
    [SerializeField] private float _addCool = 30.0f;
    [SerializeField] private bool _log = false;
    #endregion

    #region 내부 변수
    private Player_DataSO _data;
    #endregion

    private void Awake()
    {
        if (_data == null)
        {
            if (Player_DataManager.Instance != null)
            {
                _data = Player_DataManager.Instance.GetDataSO;
            }
        }
    }

    private void Update() // 추후 중복 제거 예정
    {
        if (_state == ESteminaState.Idle)
        {
            _idleStateTime += Time.deltaTime;

            if (_idleStateTime >= _addCool)
            {
                AddStemina(_data.GetSteminaAddAmount);
                _idleStateTime = 0;
            }
        }

        if (_state == ESteminaState.Decrese)
        {
            _decreaseStateTime += Time.deltaTime;
            if (_decreaseStateTime >= _decreaseCool)
            {
                DecreaseStemina(_data.GetSteminaDecreaceCost);
                _decreaseStateTime = 0;
            }
        }
    }

    private ESteminaState TrangitionState(EMovementState moveState)
    {
        ESteminaState state = ESteminaState.Idle;

        switch (moveState)
        {
            case EMovementState.Idle:
            case EMovementState.Walk:
                state = ESteminaState.Idle;
                break;

            case EMovementState.Crouch:
            case EMovementState.Run:
                state = ESteminaState.Decrese;
                break;

            case EMovementState.Attack:
                state = ESteminaState.Attack;
                break;
        }

        return state;
    }

    #region 외부 호출 함수
    public void AddStemina(int value)
    {
        if (_data == null && Player_DataManager.Instance != null)
        {
            _data = Player_DataManager.Instance.GetDataSO;

            if (_data == null)
            {
                Debug.LogError($"[{this.name}] : 데이터 매니저 없음");
                return;
            }
        }

        int stemina = _data.Stemina;
        stemina += value;
        stemina = Mathf.Clamp(stemina, 0, 100);

        _data.Stemina = stemina;
        if (_log) { Debug.Log($"[{this.name}] : 현재 증가 현재 : {stemina}"); }
    }

    public void DecreaseStemina(int value)
    {
        if (_data == null && Player_DataManager.Instance != null)
        {
            _data = Player_DataManager.Instance.GetDataSO;

            if (_data == null)
            {
                Debug.LogError($"[{this.name}] : 데이터 매니저 없음");
                return;
            }
        }

        int stemina = _data.Stemina;
        stemina -= value;
        stemina = Mathf.Clamp(stemina, 0, 100);

        _data.Stemina = stemina;
        if (_log) { Debug.Log($"[{this.name}] : 현재 감소 현재 : {stemina}"); } 
    }

    public void SetState(EMovementState moveState)
    {
        ESteminaState state = TrangitionState(moveState);

        if (_state == state) { return; }

        _state = state;
        
        if (_state == ESteminaState.Attack)
        {
            DecreaseStemina(_data.GetSteminaAttackCost);
        }

        if (_log) { Debug.Log($"[{this.name}] : 상태 변경 = {_state}"); }
    }
    #endregion
}
