using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    [Header("옵션")]
    [SerializeField] private bool _canAddAttack = false;
    [SerializeField] private float _inputRockTime = 0.3f; // 시작되고 일정시간은 입력 감지 X
    [SerializeField] private float _inputWaitOverab = 0.3f; // 애니메이션 종료후 입력 감지 시간
    #endregion

    #region 내부 변수
    private Dictionary<float,WaitForSeconds> _waitDic = new Dictionary<float,WaitForSeconds>();
    private Coroutine _attackCo;
    private Coroutine _imWaitCo;
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

        _waitDic.Clear();
    }

    private void OnEnable()
    {
        if (PlayerInputManager.Instance != null)
        {
            _im.OnAttack += TryAttack;
        }
        else
        {
            _imWaitCo = StartCoroutine(CoWaitInputManager());
        }
    }
    
    private IEnumerator CoWaitInputManager()
    {
        while (true)
        {
            if (PlayerInputManager.Instance != null)
            {
                _im = PlayerInputManager.Instance;
                _im.OnAttack += TryAttack;

                Debug.Log($"[{this.name}] : 인풋매니저 캐싱 완료");
                yield break;
            }
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (PlayerInputManager.Instance != null)
        {
            PlayerInputManager.Instance.OnAttack -= TryAttack;
        }

        _attackCo = null;
    }

    private void TryAttack()
    {
        if (_canAddAttack)
        {
            AddCombo();
        }
        if (_attackCo != null)
        {
            return;
        }

        _attackCo = StartCoroutine(CoAttack());
    }

    private void AddCombo()
    {
        _combo++;
        _combo = Mathf.Min(_combo, 2);
    }

    private WaitForSeconds SetWaitTime(float t)
    {
        if (!_waitDic.ContainsKey(t))
        {
            _waitDic[t] = new WaitForSeconds(t);
        }

        return _waitDic[t];
    }

    private IEnumerator CoAttack()
    {
        if (_anim == null) { yield break; }

        if (_im != null)
        {
            _im.SetInputState(PlayerInputManager.EInputState.Attack);
        }

        _anim.SetComboInteger(_combo);
        _anim.SetTreggerAttack();

        yield return SetWaitTime(_inputRockTime);

        float firstTime = _anim.GetAnimLength + _inputWaitOverab - _inputRockTime;
        _canAddAttack = true;

        yield return SetWaitTime(firstTime);

        _canAddAttack = false;

        if (_combo < 2)
        {
            _im.SetInputState(PlayerInputManager.EInputState.Playing);
            _combo = 1;
            _anim.SetComboInteger(0);
            StopCoroutine(_attackCo);
            _attackCo = null;
            yield break;
        }

        _anim.SetComboInteger(_combo);

        yield return null;

        float secondTime = _anim.GetAnimLength;

        yield return SetWaitTime(secondTime);

        _im.SetInputState(PlayerInputManager.EInputState.Playing);
        _combo = 1;
        _anim.SetComboInteger(0);
        StopCoroutine(_attackCo);
        _attackCo = null;
    }
}
