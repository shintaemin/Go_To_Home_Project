using UnityEngine;

#region 플레이어 애니메이션
/*
 ▶ 할일
  - 플레이어 애니메이션을 지정하는 스크립트
*/
#endregion

public class Player_Anim : MonoBehaviour
{
	#region 인스펙터
	[Header("애니메이터")]
	[SerializeField] private Animator _anim;
	[SerializeField] private float _speed;

    [Header("파라미터")]
	[SerializeField] private string _speedParam = "fSpeed";
	[SerializeField] private string _attackParam = "tAttack";

	[Header("옵션")]
	[SerializeField] private float _paramUpdatespeed = 1.0f;
	#endregion

	#region 내부 변수
	private int _hashSpeed;
	private int _hashAttack;
	private float _targerSpeed;
    #endregion

    private void Awake()
    {
		GUtill.TryGetCS(this, ref _anim);

        _hashSpeed = Animator.StringToHash(_speedParam);
        _hashAttack = Animator.StringToHash(_attackParam);
    }

    private void Update()
    {
        if (Mathf.Approximately(_speed, _targerSpeed)) { return; }

        _speed = Mathf.Lerp(_speed, _targerSpeed, _paramUpdatespeed * Time.deltaTime);

        _anim.SetFloat(_hashSpeed, _speed);
    }

    #region 외부 호출 함수
    public void MoveAnimUpdate(EMovementState state)
	{
		switch(state)
		{
			case EMovementState.Idle: _targerSpeed = 0.00f;	break;
            case EMovementState.Walk: _targerSpeed = 0.66f; break;
			case EMovementState.Crouch: _targerSpeed = 0.33f; break;
			case EMovementState.Run: _targerSpeed = 1.00f; break;
		}
    }

	public void SetTreggerAnim(EMovementState state)
	{
		switch (state)
		{
			case EMovementState.Attack: _anim.SetTrigger(_hashAttack); break;
				// 상호 작용 사망 등을 여기서 처리
        }
		
	}
	#endregion
}
