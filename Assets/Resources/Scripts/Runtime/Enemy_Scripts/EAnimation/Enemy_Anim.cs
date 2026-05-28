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

    private Enemy_Controller _controllerCS;
    #endregion

    private void Awake()
    {
        if (_anim == null)
        {
            GUtill.TryGetCS(this, ref _anim);
        }
        if (_controllerCS == null)
        {
            GUtill.TryGetCS(this, ref _controllerCS);
        }

        RuntimeAnimatorController[] controllers = Resources.LoadAll<RuntimeAnimatorController>(_controllersPath);

        if (controllers.Length != 0)
        {
            int random = Random.Range(0, controllers.Length);
            _anim.runtimeAnimatorController = controllers[random];

            InjectAnimationEvents();
        }


        _speedHash = Animator.StringToHash(_speedParam);
        _randomHash = Animator.StringToHash(_randomParam);
        _attackHash = Animator.StringToHash(_attackParam);
        _deathHash = Animator.StringToHash(_deathParam);
        _wakeHash = Animator.StringToHash(_wakeParam);
    }
    private void InjectAnimationEvents()
    {
        if (_anim == null || _anim.runtimeAnimatorController == null) return;

        // 현재 랜덤 주입된 컨트롤러 안의 모든 애니메이션 클립 배열을 가져옵니다.
        AnimationClip[] clips = _anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            // 중복 주입 방지를 위해 기존 이벤트 찌꺼기를 한 번 청소합니다.
            clip.events = null;

            if (clip.name.Contains("death"))
            {
                // 3. 사망 마감 이벤트 생성 (맨 마지막 프레임에 강제 위치 배치)
                AnimationEvent deathLastEvent = new AnimationEvent();
                deathLastEvent.time = clip.length; // 클립 총 길이(맨 끝)를 타임으로 지정
                deathLastEvent.functionName = nameof(AnimEvent_DeathLastFrame);
                clip.AddEvent(deathLastEvent);
            }
        }
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
    public void AnimEvent_DeathLastFrame()
    {
        if (_controllerCS != null)
        {
            _controllerCS.DeathLastFrame();
        }
        else
        {
            Destroy(transform.root.gameObject);
        }
    }
    #endregion
}
