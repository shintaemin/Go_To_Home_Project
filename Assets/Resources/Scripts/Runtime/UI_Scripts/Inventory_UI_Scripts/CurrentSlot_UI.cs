using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentSlot_UI : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private Image _currentImage;
	[SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _infoText;
	#endregion

	#region 외부 호출 함수 
	public void SetIcon(Sprite icon)
	{
		if (_currentImage == null || icon == null) { return; }
		
		_currentImage.sprite = icon;
	}
	public void SetName(string name)
	{
		if (_nameText == null) { return; }
		
		_nameText.text = name;
	}
	public void SetInfo(string info)
	{
		if(_infoText == null) { return; }

		_infoText.text = info;
	}
	#endregion
}
