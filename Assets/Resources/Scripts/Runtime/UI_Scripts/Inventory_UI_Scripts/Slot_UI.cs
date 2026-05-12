using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#region 슬롯 UI
/*
 ▶ 할일 
  - 슬롯 데이터를 UI에 보여줄 스크립트
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
    [SerializeField] private int _index;
    [SerializeField] private ESlotPathType _pathType;

	[Header("표시할 데이터")]
	[SerializeField] private Image _image;
    [SerializeField] private GameObject _textRoot;
	[SerializeField] private TextMeshProUGUI _countText;
    #endregion

    #region 내부 변수
    private string _countStr;
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

    }

    #region 외부 호출 함수
    public void UpdataSlotUI(SlotData slotData)
    {
        if (_image == null || _countText == null) { return; }

        SlotData slot = slotData;
        ItemDataSO data = slot.GetItem;
        int count = slot.GetCount;

        if (data == null)
        {
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

    #region 클릭, 드래그, 드랍 인터페이스
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (UI_SlotMove_Manager.Instance == null)
        {
            GUtill.Log($"[{this.name}] : 드래그 실패?", EDebugType.Warn);
            return; 
        }

        UI_SlotMove_Manager.Instance.GetUpSlot(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (UI_SlotMove_Manager.Instance == null) { return; }

        UI_SlotMove_Manager.Instance.DragSlot();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (UI_SlotMove_Manager.Instance == null) { return; }

        UI_SlotMove_Manager.Instance.DragEnd();
    }

    public void OnDrop(PointerEventData eventData)
    {

    }
    #endregion

    public int Index
    { 
        get { return _index; } 
        set { _index = value; }
    }

    public ESlotPathType PathType
    {
        get { return _pathType; }
        set { _pathType = value; }
    }

    public Sprite GetSlotIcon => _image.sprite;

    public string GetCountText => _countStr;
    public void SetSlotIcon(Sprite icon) => _image.sprite = icon;
    public void SetText(string text) => _countText.text = text;
    #endregion
}
