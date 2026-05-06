using System.Collections.Generic;
using UnityEngine;

#region 아이템 데이터 창고
/*
 ▶ 할일
  - 아이템 데이터 SO 들을 저장할 RegistrySO
  - 게임중에 변경되지않도록 외부에서 추가나 등록이 불가하게 private 로하고 사용만 가능하도록 작업하기
*/
#endregion

[CreateAssetMenu(menuName = "RegistrySO/ItemDatas", fileName = "ItemDataRegistrySO")]
public class ItemDataRegistrySO : ScriptableObject
{
    #region 인스펙터
    [SerializeField] private List<ItemDataSO> _itemList = new List<ItemDataSO>();
    [SerializeField] private string _path = "SO/ItemSO";
    #endregion

    #region 내부변수
    private Dictionary<int , ItemDataSO> _itemsDic = new Dictionary<int , ItemDataSO>();
    #endregion

    // 데이터를 List 와 Dictionary 에 등록할 함수
    private void InitDataList()
    {
        _itemList.Clear();  // 시작시 비워버리기
        _itemsDic.Clear();

        ItemDataSO[] items = Resources.LoadAll<ItemDataSO>(_path); // 지정된 위치에 모든 ItemDataSO 를 배열로 생성

        // 찾은 아이템 SO 들을 딕셔너리의 <키 : id , 밸류 , ItemDataSO> 로 등록
        foreach(ItemDataSO data in items)
        {
            if (data == null) { continue; }
            ItemDataSO item = data;
            int id = item.ID;

            if (_itemsDic.ContainsKey(id))
            {
                GUtill.Log($"[{this.name}] : 등록중 id 중복", EDebugType.Warn);
                continue;
            }

            AddItemData(item);
            _itemsDic.Add(id, item);
        }

        GUtill.Log($"[{this.name}] : 딕셔너리 초기화 완료", EDebugType.Warn);
    }

    // 리스트에 아이템을 추가할 함수
    private void AddItemData(ItemDataSO data)
    {
        if (_itemList.Contains(data))
        {
            GUtill.Log($"[{this.name}] : 이미 {data} 가 리스트에 담겨있음", EDebugType.Warn);
            return;
        }

        _itemList.Add(data);
    }

    #region 외부 호출 함수
    // 매니저를 통해 아이템을 꺼내쓸때 사용할 함수
    public ItemDataSO GetItemData(int id)
    {
        if (_itemsDic.Count == 0) 
        { 
            InitDataList();
            GUtill.Log($"[{this.name}] : 딕셔너리 가 비어있어 자동 업데이트 수행", EDebugType.Warn);
        }

        _itemsDic.TryGetValue(id, out ItemDataSO data);
        return data;
    }

    // 외부에서 이름으로도 찾을수 있도록 간단하게 리스트를 순회하는 로직
    public ItemDataSO GetItemData(string name)
    {
        ItemDataSO data = null;

        for (int i = _itemList.Count - 1; i >= 0; i--)
        {
            if (_itemList[i].Name == name)
            {
                data = _itemList[i];
                break;
            }
        }

        return data;
    }

    // 시작시 업데이트를 진행할 진입점
    public void Init()
    {
        InitDataList();
    }
    #endregion
}
