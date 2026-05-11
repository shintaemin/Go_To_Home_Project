using UnityEngine;

#region 아이템 데이터 매니저
/*
 ▶ 할일
  - 모든 아이템을 쉽게 접근하기 위한 매니저 스크립트
*/
#endregion


public class ItemDataManager : MonoBehaviour
{
    public static ItemDataManager Instance { get; private set; }
    #region 인스펙터
    [SerializeField] private ItemDataRegistrySO _registrySO;
    #endregion

    #region

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            enabled = false;
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        if (_registrySO == null)
        {
            _registrySO = Resources.Load<ItemDataRegistrySO>("SO/ItemSO/ItemDataRegistrySO");
        }

        _registrySO?.Init();
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region 외부 호출 함수
    public ItemDataSO GetItem(int id) // id 로 아이템을 꺼내오기
    {
        return _registrySO?.GetItemData(id);
    }
    public ItemDataSO GetItem(string name)
    {
        return _registrySO?.GetItemData(name);
    }

    public ItemDataSO GetRandomItem()
    {
        if (_registrySO == null)
        {
            GUtill.Log($"[{this.name}] : 레지스트리 비어있음");
            return null;
        }

        int length = _registrySO.GetLength;

        int random = Random.Range(0, length);

        return _registrySO.GetItemDataIndex(random);
    }
    #endregion
}