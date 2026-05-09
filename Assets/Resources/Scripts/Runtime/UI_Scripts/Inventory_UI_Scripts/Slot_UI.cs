using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot_UI : MonoBehaviour
{

	#region 인스펙터
	[SerializeField] private SlotData _data;
	[SerializeField] private int _index;

	[Header("표시할 데이터")]
	[SerializeField] private Image _image;
	[SerializeField] private TextMeshProUGUI _countText;
	#endregion

	#region 내부 변수
	private Inventory_Manager _invenManager;
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

    #region 이벤트 구독
    private void SubInventoryManager()
	{
		if (Inventory_Manager.Instance != null)
		{
            _invenManager = Inventory_Manager.Instance;

            _invenManager.OnSlotChanged += SlotUpdata;
        }
	}

    private void OnDestroy()
    {
		if (_invenManager != null)
		{
            _invenManager.OnSlotChanged -= SlotUpdata;
		}
    }

    private void SlotUpdata(int id)
	{
		if (_index != id) { return; }
		if (_invenManager == null) { return; }

		if (_data == null || _data != _invenManager.GetSlotData(_index))
		{
            _data = _invenManager.GetSlotData(_index);
        }

        Sprite icon = null;

		if (_data == null)
		{
			GUtill.Log("너 들어오냐?");
			return;
		}

        if (_data.GetItem != null)
		{
            icon = _data.GetItem.Icon;
            int max = _data.GetItem.MaxStack; // 최대 갯수 체크
            int remain = _data.GetCount;

            UpdateUI(icon, $"{remain} / {max}");
        }
    }
	#endregion

	private void UpdateUI(Sprite icon, string str)
    {
        if (_image == null || _countText == null) { return; }
		if (icon == null) { return; }
		_image.sprite = icon;
        _countText.text = str;
    }
	
	#region 외부 호출 함수
	public void InitData(SlotData data)
    {
        _data = data;
		_index = data.GetId;
        SubInventoryManager();
        SlotUpdata(_index);
    }	
	#endregion
}
