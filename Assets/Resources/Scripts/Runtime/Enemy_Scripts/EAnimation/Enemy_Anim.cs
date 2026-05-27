using UnityEngine;

#region 애니메이션
/*
 ▶ 할일
  - 외부에서 애니메이션 을 업데이트할 수 있도록 작업
*/
#endregion

public enum EEnemyAnimTrigger
{
    None = 0,
    Attack,
    Death,
    Wake,
}

public enum EEnemyMoveAnim
{
    None = 0,
    Idle,
    Walk,
    Fast,
}

public class Enemy_Anim : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Animator _anim;
    [SerializeField] private float _speed;
    [SerializeField] private float _targetSpeed;
    [SerializeField] private string _controllersPath = "Animator/Enemy_Controllers";

    [SerializeField] private string _speedParam = "fSpeed";
    [SerializeField] private string _randomParam = "iRandom";
    [SerializeField] private string _attackParam = "tAttack";
    [SerializeField] private string _deathParam = "tDeath";
    [SerializeField] private string _wakeParam = "tWake";

    [SerializeField] private float _updateSpeed = 5.0f;
    #endregion

    #region 내부변수
    private int _speedHash;
    private int _randomHash;
    private int _attackHash;
    private int _deathHash;
    private int _wakeHash;
    #endregion

    private void Awake()
    {
        if (_anim == null)
        {
            GUtill.TryGetCS(this, ref _anim);
        }

        RuntimeAnimatorController[] controllers = Resources.LoadAll<RuntimeAnimatorController>(_controllersPath);

        if (controllers.Length != 0)
        {
            int random = Random.Range(0, controllers.Length);
            _anim.runtimeAnimatorController = controllers[random];
        }


        _speedHash = Animator.StringToHash(_speedParam);
        _randomHash = Animator.StringToHash(_randomParam);
        _attackHash = Animator.StringToHash(_attackParam);
        _deathHash = Animator.StringToHash(_deathParam);
        _wakeHash = Animator.StringToHash(_wakeParam);
    }

    private void Update()
    {
        if (_anim == null) { return; }
        if (Mathf.Approximately(_speed, _targetSpeed)) { return; }

        float start = _speed;
        float end = _targetSpeed;
        float t = 1.0f - Mathf.Exp(-_updateSpeed * Time.deltaTime);
        float current = Mathf.Lerp(start, end, t);

        _speed = current;

        _anim.SetFloat(_speedHash, _speed);
    }

    #region 외부 호출 함수
    public void TriggerAnim(EEnemyAnimTrigger trigger)
    {
        int hash = 0;

        switch (trigger)
        {
            case EEnemyAnimTrigger.Attack: 
                hash = _attackHash;
                int random = Random.Range(0, 2);
                _anim.SetInteger(_randomHash, random);
                break;
            case EEnemyAnimTrigger.Death: hash = _deathHash; break;
            case EEnemyAnimTrigger.Wake: hash = _wakeHash; break;
        }

        if (hash != 0) { _anim.SetTrigger(hash); }
    }
    public void SetSpeedParam(EEnemyMoveAnim anim)
    {
        if (_anim == null) { return; }

        float speed = 0;

        switch(anim)
        {
            case EEnemyMoveAnim.Idle: speed = 0; break;
            case EEnemyMoveAnim.Walk: speed = 0.5f; break;
            case EEnemyMoveAnim.Fast: speed = 1f; break;
        }

        _targetSpeed = speed;
    }
    #endregion
}
