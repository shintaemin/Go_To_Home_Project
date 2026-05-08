using UnityEngine;

#region 인벤토리 UI
/*
 ▶ 할일
  - 인벤토리 UI On / Off
  - 자식 오브젝트에 들어갈 Slot 정보들을 불어오도록 해야하므로 이곳에서 함수 호출을 예정
*/
#endregion


public class Inventory_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _inventoryRoot;
    [SerializeField] private bool _isActive = false;    // 테스트 및 정보 매칭 확인용

    #endregion

    private void Awake()
    {
        if (_inventoryRoot == null)
        {
            _inventoryRoot = transform.GetChild(0).gameObject;
        }
    }

    private void Start()
    {
        Active(_isActive);
    }

    #region 외부 호출 함수
    public void Active(bool active)
    {
        _isActive = active;
        _inventoryRoot.SetActive(_isActive);
    }

    public void OnClickCloseButton()
    {
        Active(false);
    }
    #endregion
}
