using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 플레이어 데이터
/*
 ▶ 할일
  - 플레이어의 모든 스탯 데이터를 들고있을 오브젝트
  - 스크립터블 오브젝트로 만드는걸 고려하기
*/
#endregion

[CreateAssetMenu(menuName = "DataSO/Player_Data", fileName = "Player_Data_SO")]
public class Player_Data : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private string _name = "Daddy";
	[SerializeField] private float _moveSpeed = 5.0f;

	#endregion

	#region 외부 호출 함수
	public float GetSpeed => _moveSpeed;
	#endregion
}
