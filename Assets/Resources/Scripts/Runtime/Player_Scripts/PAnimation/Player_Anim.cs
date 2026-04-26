using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

	[Header("파라미터")]
	[SerializeField] private string _speedParam = "fSpeed";
	#endregion

	#region 내부 변수
	private Player_MoveScripts _moveScript;
	private float _nextLogTime; // 테스팅 로그 출력 시간 지정
    #endregion

    private void Awake()
    {
		if (_moveScript == null)
		{
			if (!TryGetComponent<Player_MoveScripts>(out _moveScript))
			{
				Debug.LogWarning($"[Player_Anim] : 무브 컴포넌트 캐싱 실패 <인스펙터 확인>");
				return;
			}
		}
    }
}
