using UnityEngine;

#region 슬롯 이동 매니저
/*
 ▶ 할일
  - 슬롯 데이터가 이동할 수 있도록 슬롯의 타입과 인덱스 Slot_UI 를 받아
  - 타입에따라 분기 하여 인벤토리 -> 컨테이너 , 컨테이너 -> 인벤토리 로 아이템 이동을 수행
*/
#endregion


public class UI_SlotMove_Manager : MonoBehaviour
{
    public static UI_SlotMove_Manager Instance { get; private set; }

    #region 인스펙터
    [SerializeField] private Slot_UI _dragSlot;
    [SerializeField] private Slot_UI _dragIcon;
    #endregion

    #region 내부변수
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

    }

    private void Start()
    {
        _dragIcon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region 외부 호출 함수
    public void DropSlot()
    {
        _dragIcon.gameObject.SetActive(false);
        // 레이 충돌 슬롯에 등록 로직 구상
    }

    public void GetUpSlot(Slot_UI slotUI)
    {
        _dragSlot = slotUI;
        _dragIcon.SetSlotIcon(slotUI.GetSlotIcon);
        _dragIcon.SetText(slotUI.GetCountText);
        _dragIcon.gameObject.SetActive(true);
    }

    public void DragSlot()
    {
        _dragIcon.transform.position = Input.mousePosition;
    }

    public void DragEnd()
    {
        _dragIcon.gameObject.SetActive(false);
        _dragSlot = null;
    }
    #endregion
}
