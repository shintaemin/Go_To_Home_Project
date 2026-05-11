using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region 슬롯 UI
/*
 ▶ 할일 
  - 슬롯 데이터를 UI에 보여줄 스크립트
*/
#endregion

public class Slot_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private int _index;

	[Header("표시할 데이터")]
	[SerializeField] private Image _image;
	[SerializeField] private TextMeshProUGUI _countText;
    #endregion

    #region 내부 변수
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
            _countText.gameObject.SetActive(false);
            return;
        }

        int max = data.MaxStack;
        Sprite icon = data.Icon;

        _image.sprite = icon;
        _countText.text = $"{count} / {max}";
    }

    public int Index
    { 
        get { return _index; } 
        set { _index = value; }
    }
    #endregion
}
