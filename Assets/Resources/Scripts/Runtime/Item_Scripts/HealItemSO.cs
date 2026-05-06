using UnityEngine;


#region 회복 아이템 SO
/*
 ▶ 할일
  - 회복 아이템이 갖는 기본 정보
*/
#endregion

public enum EHealingType
{
    None = 0,
    HP,
    Stemina,
}

public class HealItemSO : AvailableDataSO
{
    [SerializeField] private EHealingType _healType;
    [SerializeField] private int _value;
    [SerializeField] private float _duration;
    [SerializeField] private float _cooldown;

    #region 파싱시 셋업 함수
    public void SetUp
        (
        int id, 
        string name, 
        EItemType itemType, 
        int isInteract, 
        int isStackable, 
        int maxStackCount, 
        Sprite icon, 
        string info, 
        EAvailableType availableType, 
        EHealingType healType, 
        int value, 
        float dur, 
        float cooldown,
        GameObject prefab
        )
    {
        base.SetUp(id, name, itemType, isInteract, isStackable, maxStackCount, icon, info, availableType, prefab);
        _healType = healType;
        _value = value;
        _duration = dur;
        _cooldown = cooldown;
    }
    #endregion
    #region 외부 호출 함수
    public EHealingType HealType => _healType;
    public int Value => _value;
    public float Duration => _duration;
    public float Cooldown => _cooldown;

    public override void Use(GameObject obj)
    {
        if (obj == null || Player_DataManager.Instance == null) { return; }

        Player_DataSO pData = Player_DataManager.Instance.GetDataSO;
        
        switch(_healType)
        {
            case EHealingType.HP: pData.HP += _value; break;
            case EHealingType.Stemina: pData.Stemina += _value; break;
        }
    }
	#endregion
}
