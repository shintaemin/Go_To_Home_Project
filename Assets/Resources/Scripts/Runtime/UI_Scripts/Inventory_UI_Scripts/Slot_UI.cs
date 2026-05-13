using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#region 슬롯 UI
/*
 ▶ 할일 
  - 슬롯 데이터를 UI에 보여줄 스크립트
  - 이동시킬 슬롯을 지정하고 현재 슬롯에 Drop 수행시 매니저를 통해 분기
*/
#endregion

public enum ESlotPathType
{
    None,
    Inventory,
    Container,
    Field
}

public class Slot_UI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    #region 인스펙터
    [SerializeField] private SlotData _slotData;
    [SerializeField] private ESlotPathType _pathType;
    [SerializeField] private int _index;

	[Header("표시할 데이터")]
	[SerializeField] private Image _image;
    [SerializeField] private GameObject _textRoot;
	[SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private string _countStr;
    #endregion

    #region 내부 변수
    private Sprite _nullIcon;
    #endregion
    private void Awake()
    {
		if (_image == null)
		{
			GUtill.Log($"[{this.name}] : 이미지 컴포넌트 없음", EDebugType.Error);
		}

		if (_countText == null)
        {
            GUtill.Log($"[{this.name}] : 갯수 TMP Pro 없음", EDebugType.Error);
        }

        _nullIcon = _image.sprite;
    }

    private void UpdateSlot(SlotData slotData)
    {
        _slotData = slotData;

        ItemDataSO data = _slotData.GetItem;
        int count = _slotData.Count;

        if (data == null)
        {
            _image.sprite = _nullIcon;
            _textRoot.SetActive(false);
            return;
        }

        _textRoot.SetActive(true);
        int max = data.MaxStack;
        Sprite icon = data.Icon;

        _image.sprite = icon;
        _countStr = $"{count} / {max}";
        _countText.text = _countStr;
    }

    #region 외부 호출 함수
    #region 클릭, 드래그, 드랍 인터페이스
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (UI_SlotMove_Manager.Instance == null) { return; }

        UI_SlotMove_Manager.Instance.Begin(this);
    }

    // 드래그중 
    public void OnDrag(PointerEventData eventData)
    {
        if (UI_SlotMove_Manager.Instance == null) { return; }

        UI_SlotMove_Manager.Instance.Drag();
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (UI_SlotMove_Manager.Instance == null) { return; }

        UI_SlotMove_Manager.Instance.DragEnd();
    }

    // 현재 슬롯에 드랍
    public void OnDrop(PointerEventData eventData)
    {
        if (UI_SlotMove_Manager.Instance == null) { return; }

        // 이동중인 데이터 와 UI 를 확인
        SlotData dropData = UI_SlotMove_Manager.Instance.GetDragData;
        Slot_UI slotUI = UI_SlotMove_Manager.Instance.GetDragUI;

        GUtill.Log($"[{this.name}] : 드롭 성공 {PathType} 로 이동", EDebugType.Warn);
        // Drop 한 슬롯의 type 을 확인
        switch (PathType)
        {
            case ESlotPathType.Inventory: // 인벤토리에 아이템 추가
                Inventory_Manager.Instance.AddItem(dropData, Index); break;
            case ESlotPathType.Container: // 컨테이너에 아이템 추가
                UI_SlotMove_Manager.Instance.GetContainer.AddItem(dropData, Index); break;
        }

        GUtill.Log($"[{this.name}] : 이동 성공 {slotUI.PathType} 의 데이터 삭제", EDebugType.Warn);
        // 이동 UI 의 type 을 확인
        switch (slotUI.PathType)
        {
            case ESlotPathType.Inventory: // 인벤토리아이템 삭제
                Inventory_Manager.Instance.RemoveSlotData(dropData); break;
            case ESlotPathType.Container: // 컨테이너 아이템 삭제
                UI_SlotMove_Manager.Instance.GetContainer.RemoveItem(dropData); break;
        }
        UI_SlotMove_Manager.Instance.DataMoveEnd();
        GUtill.Log($"[{this.name}] : 슬롯데이터 이동완료 {slotUI.PathType} -> {PathType}", EDebugType.Warn);
    }
    #endregion  -> 드래그 드랍 End
    #region 프로퍼티
    public SlotData Data // 슬롯데이터
    {
        get { return _slotData; }
        set { UpdateSlot(value); }
    }
    public int Index // 인덱스 (id 매칭)
    {
        get { return _index; }
        set { _index = value; }
    }
    public ESlotPathType PathType // 현재 창고 (인벤토리, 루팅상자, 필드)
    {
        get { return _pathType; }
        set { _pathType = value; }
    }
    public Sprite SlotIcon // 아이콘 이미지
    {
        get { return _image.sprite; }
        set { _image.sprite = value; }
    }
    public string CountText // 갯수 텍스트
    {
        get { return _countStr; }
        set { _countStr = value; }
    }
    #endregion -> 프로퍼티 End
    #endregion -> 외부호출 함수 End 
}
