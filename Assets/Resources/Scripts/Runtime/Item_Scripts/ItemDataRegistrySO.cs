using System.Collections.Generic;
using UnityEngine;

#region 아이템 데이터 창고
/*
 ▶ 할일
  - 아이템 데이터 SO 들을 저장할 RegistrySO
*/
#endregion

[CreateAssetMenu(menuName = "RegistrySO/ItemDatas", fileName = "ItemDataRegistrySO")]
public class ItemDataRegistrySO : ScriptableObject
{
    #region 인스펙터
    [SerializeField] private List<ItemDataSO> _itemList = new List<ItemDataSO>();
    [SerializeField] private string _path;
    #endregion

    #region 내부변수
    private Dictionary<int , ItemDataSO> _itemsDic = new Dictionary<int , ItemDataSO>();
    #endregion

    private void InitDataList()
    {
        _itemList.Clear();
        _itemsDic.Clear();

        ItemDataSO[] items = Resources.LoadAll<ItemDataSO>(_path);

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

    public void Init()
    {
        InitDataList();
    }
    #endregion
}
