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
	[SerializeField] private Player_Move _move;

    [Header("파라미터")]
	[SerializeField] private string _speedParam = "fSpeed";
	[SerializeField] private string _attackParam = "tAttack";
	#endregion

	#region 내부 변수
	private int _hashSpeed;
	private int _hashAttack;
    #endregion

    private void Awake()
    {
		if (_anim == null)
		{
			if (!TryGetComponent<Animator>(out _anim))
			{
                Debug.LogWarning($"[Player_Anim] : 애니메이터 캐싱 실패 <인스펙터 확인>");
                return;
            }
		}

        _hashSpeed = Animator.StringToHash(_speedParam);
        _hashAttack = Animator.StringToHash(_attackParam);
    }
    private void Start()
    {
		if (_move == null)
		{
			if (!TryGetComponent<Player_Move>(out _move))
			{
				Debug.LogWarning($"[Player_Anim] : 무브 컴포넌트 캐싱 실패 <인스펙터 확인>");
				return;
			}
		}
    }

    private void Update()
    {
		float speed = _move.GetSpeedParam;
        _anim.SetFloat(_hashSpeed, speed);
    }

	#region 외부 호출 함수
	public void SetTreggerAttack()
	{
		_anim.SetTrigger(_hashAttack);
	}
	#endregion
}
