using UnityEngine;

#region 플레이어 데이터
/*
 ▶ 할일
  - 플레이어의 모든 스탯 데이터를 들고있을 오브젝트
  - 스크립터블 오브젝트로 만드는걸 고려하기
*/
#endregion

[CreateAssetMenu(menuName = "DataSO/Player_Data_SO", fileName = "Player_Data_SO")]
public class Player_DataSO : ScriptableObject
{
	#region 인스펙터
	[SerializeField] private string _name = "Daddy";
	[SerializeField] private float _moveSpeed = 5.0f;
	[SerializeField] private int _hp = 100;
	[SerializeField] private int _stemina = 100;
	#endregion

	#region 외부 호출 함수
	public string GetName => _name;
	public float GetSpeed => _moveSpeed;

	public int HP
	{
		get { return _hp; }
		set { _hp = value; }
	}

	public int Stemina
	{
		get { return _stemina; }
		set { _stemina = value; }
	}
	#endregion
}
