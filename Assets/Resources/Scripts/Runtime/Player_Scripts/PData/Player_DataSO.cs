using System;
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

    [SerializeField] private float _maxSpeed = 10;

    [Header("HP 옵션")]
    [SerializeField] private int _maxHP = 100;

    [Header("스테미너 옵션")]
    [SerializeField] private int _maxStemina = 100;
    [SerializeField] private int _steminaAddAmount = 5;
    [SerializeField] private int _steminaDecreaceCost = 1;
    [SerializeField] private int _steminaAttaceCost = 10;


    [SerializeField] private float _testStartSpeed = 5;
    [SerializeField] private int _testStartHP = 100;
    [SerializeField] private int _testStartStemina = 100;
	#endregion

	#region 이벤트
	public event Action<int> OnHPUpdate;
	public event Action<int> OnSteminaUpdate; 
	#endregion

    private void Awake()
    {
        InitData(_testStartHP, _testStartStemina, _testStartSpeed);
    }

    #region 외부 호출 함수
    public string GetName => _name;
	public float GetSpeed => _moveSpeed;

    public int GetSteminaAddAmount => _steminaAddAmount;
    public int GetSteminaDecreaceCost => _steminaDecreaceCost;
    public int GetSteminaAttackCost => _steminaAttaceCost;

    public void InitData(int hp, int stemina, float moveSpeed)
    {
        HP = Mathf.Clamp(hp, 0, _maxHP);
        Stemina = Mathf.Clamp(stemina, 0, _maxStemina);
        _moveSpeed = Mathf.Clamp(moveSpeed, 0, _maxSpeed);

    }
    #endregion

    #region 프로퍼티
    public int HP
    {
        get { return _hp; }
        set
        {
            _hp = value;
            OnHPUpdate?.Invoke(_hp);
        }
    }

    public int Stemina
    {
        get { return _stemina; }
        set
        {
            _stemina = value;
            OnSteminaUpdate?.Invoke(_stemina);
        }
    }
    #endregion
}
