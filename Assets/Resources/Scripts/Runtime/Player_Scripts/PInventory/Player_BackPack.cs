using UnityEngine;

#region 가방
/*
 ▶ 할일
  - 가방 오브젝트의 애니메이션을 재생
  
    ※ 애니메이션 처리 이유 ※
     - 코루틴이나 Update 로 매프레임 혹은 일정 간격으로 검사하는 방식보다는 필요할떄 1회 호출되는 애니메이션 트리거 방식이 성능상 이득이라고 생각했음
*/
#endregion


public class Player_BackPack : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Animator _anim;

    [Header("파라미터 옵션")]
    [SerializeField] private string _openParam = "tOpening";
    [SerializeField] private string _closeParam = "tClosing";
    #endregion

    #region 내부 변수
    private int _openHash;
    private int _closeHash;
    private Player_Inventory _inventoryCS;
    #endregion

    private void Awake()
    {
        GUtill.TryGetCS(this, ref _anim);
        _inventoryCS = FindFirstObjectByType<Player_Inventory>();

        _openHash = Animator.StringToHash(_openParam);
        _closeHash = Animator.StringToHash(_closeParam);
    }

    #region 외부 호출 함수
    public void SetTriggerBackPack(bool open) // 외부에서 애니메이션 지정
    {
        int num = open ? _openHash : _closeHash;

        _anim.SetTrigger(num);
    }

    public void UIOpen()
    {
        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InventoryActive(true);
        }
    }

    public void UIClose()
    {
        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.InventoryActive(false);
        }
    }

    // 애니메이션이 끝날떄 키프레임 마지막에 실행될 함수
    public void ClosingEnd()
    {
        _inventoryCS.MoveToHand();
    }
    #endregion
}
