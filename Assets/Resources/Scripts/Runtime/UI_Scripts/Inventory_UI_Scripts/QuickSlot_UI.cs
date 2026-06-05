using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#region

#endregion


public class QuickSlot_UI : MonoBehaviour, IDropHandler
{
    #region 인스펙터
    [SerializeField] private SlotData _slotData;

    [SerializeField] private Image _image;
    [SerializeField] private GameObject _textRoot; 
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private string _countStr;
    #endregion

    #region 내부변수
    private Sprite _nullIcon;
    private Color _nullColor;
    private UI_SlotMove_Manager _uiSlotManager;
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
        _nullColor = _image.color;
        ClearSlot();
    }
    private void Start()
    {
        if (UI_SlotMove_Manager.Instance != null)
        {
            _uiSlotManager = UI_SlotMove_Manager.Instance;
        }
    }
    private void ClearSlot()
    {
        _slotData = null;
        _image.sprite = _nullIcon;
        _image.color = _nullColor;
        _countText.text = string.Empty;
        _textRoot.SetActive(false);
    }

    #region 외부 호출 함수
    public void OnDrop(PointerEventData eventData)
    {
        if (_uiSlotManager == null) { return; }

        // 이동중인 데이터 와 UI 를 확인
        SlotData dropData = _uiSlotManager.GetDragData;
        Slot_UI slotUI = _uiSlotManager.GetDragUI;

        if (dropData == null || slotUI == null || dropData.GetItem == null) { return; }
        if (dropData.GetItem.IsEquipable || dropData.GetItem is SoundItemSO)
        {
            _slotData = dropData;
            ItemDataSO item = dropData.GetItem;
            int index = dropData.Index;
            int count = dropData.Count;
            int dur = dropData.Dur;
            float cool = dropData.GetCoolEndTime;
            _slotData.SetItem(item, index, count, dur, cool);
            UpdateSlot();
            _uiSlotManager.DataMoveEnd();
        }
    }
    public void UpdateSlot()
    {
        if (_slotData == null || _slotData.GetItem == null || _slotData.Count <= 0)
        {
            ClearSlot();
            return;
        }

        ItemDataSO data = _slotData.GetItem;
        int count = _slotData.Count;
        _textRoot.SetActive(data.IsStackable);
        int max = data.MaxStack;
        Sprite icon = data.Icon;
        Color color = Color.white;
        color.a = 1;

        _image.sprite = icon;
        _image.color = color;
        _countStr = $"{count} / {max}";
        _countText.text = _countStr;
    }
    public SlotData GetSlotData() => _slotData;
    #endregion
}
