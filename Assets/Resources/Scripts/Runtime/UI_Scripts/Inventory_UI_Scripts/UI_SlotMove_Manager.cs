using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region 슬롯 이동 매니저
/*
 ▶ 할일
  - 슬롯 데이터가 이동할 수 있도록 슬롯의 타입과 인덱스 Slot_UI 를 받아 지정
  - 외부에서 이동 데이터를 확인할 수 있도록 설계
*/
#endregion

public class UI_SlotMove_Manager : MonoBehaviour
{
    public static UI_SlotMove_Manager Instance { get; private set; }

    #region 인스펙터
    [Header("이동시킬 오브젝트")]
    [SerializeField] private GameObject _dragObj;
    [Header("이동시킬 슬롯 데이터")]
    [SerializeField] private SlotData _dragData;
    [Header("이동시킬 슬롯 UI")]
    [SerializeField] private Slot_UI _dragSlot;
    [Header("이동중 표시할 이미지")]
    [SerializeField] private Image _drageImage;
    #endregion

    #region 내부변수
    private Interact_Container _container; // 컨테이너 상호작용 수행시 지정받을 컨테이너
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
        _dragObj.SetActive(false);
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
        _dragObj.SetActive(false);
        // 레이 충돌 슬롯에 등록 로직 구상
    }
    
    // 드래그 시작
    public void Begin(Slot_UI slotUI)
    {
        _dragSlot = slotUI; // 이동시킬 슬롯UI 지정
        _dragData = _dragSlot.Data; // 이동시킬 데이터 지정
        _drageImage.sprite = _dragSlot.SlotIcon; // 이동시 보여줄 이미지 지정
        _dragObj.SetActive(true); // 이동오브젝트 켜기
    }

    // 드래그 중
    public void Drag()
    {
        _dragObj.transform.position = Input.mousePosition;
    }

    // 드래그 종료
    public void DragEnd()
    {
        _dragObj.SetActive(false);
    }

    // 이동데이터 초기화
    public void DataMoveEnd()
    {
        _dragData = null;
        _dragSlot = null;
    }

    // 컨테이너지정
    public void SetContainer(Interact_Container container)
    {
        _container = container;
    }

    public Interact_Container GetContainer => _container;
    public SlotData GetDragData => _dragData;
    public Slot_UI GetDragUI => _dragSlot;
    #endregion
}
