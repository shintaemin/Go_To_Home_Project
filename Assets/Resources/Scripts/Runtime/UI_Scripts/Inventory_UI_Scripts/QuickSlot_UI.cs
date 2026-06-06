using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region

#endregion

public enum EQuickSlotType
{
    None = 0,
    Weapon,
    Throwing,
}

public class QuickSlot_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private SlotData _slotData;

    [SerializeField] private Image _image;
    [SerializeField] private GameObject _textRoot; 
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Slider _durSlider;
    [SerializeField] private string _countStr;
    [SerializeField] private EQuickSlotType _quickSlotType;
    #endregion

    #region 내부변수
    private Sprite _nullIcon;
    private Color _nullColor;
    private Player_ItemEquip _equipCS;
    private int _trackedIndex = -1;
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
        if (_equipCS == null)
        {
            _equipCS = FindFirstObjectByType<Player_ItemEquip>();
        }
        _nullIcon = _image.sprite;
        _nullColor = _image.color;
        ClearSlot();
    }
    private void Start()
    {
        if (_equipCS != null)
        {
            _equipCS.OnItemEquip += SetQuickSlotEquipItem;
            _equipCS.OnSlotUpdate += UpdateSlot;
            _equipCS.OnReleaseItem += HandleItemReleased;
        }
    }
    private void OnDestroy()
    {
        if (_equipCS != null)
        {
            _equipCS.OnItemEquip -= SetQuickSlotEquipItem;
            _equipCS.OnSlotUpdate -= UpdateSlot;
            _equipCS.OnReleaseItem -= HandleItemReleased;
        }
    }
    private void ClearSlot()
    {
        _slotData = null;
        _trackedIndex = -1;
        _image.sprite = _nullIcon;
        _image.color = _nullColor;
        _countText.text = string.Empty;
        _textRoot.SetActive(false);
    }
    private void HandleItemReleased()
    {
        if (_trackedIndex != -1 && Inventory_Manager.Instance != null)
        {
            SlotData realInventorySlot = Inventory_Manager.Instance.GetSlotData(_trackedIndex);

            if (realInventorySlot == null || realInventorySlot.GetItem == null || realInventorySlot.Count <= 0 || (realInventorySlot.GetItem is WeaponDataSO && realInventorySlot.Dur <= 0))
            {
                ClearSlot();
            }
        }
    }

    #region 외부 호출 함수
    public void SetQuickSlotEquipItem(SlotData slot)
    {
        if (slot == null || slot.GetItem == null || slot.Count <= 0 || (slot.GetItem is WeaponDataSO && slot.Dur <= 0))
        {
            return;
        }
        bool isWeapon = slot.GetItem is WeaponDataSO && _quickSlotType == EQuickSlotType.Weapon;
        bool isThrowing = slot.GetItem is SoundItemSO && _quickSlotType == EQuickSlotType.Throwing;
        if (isWeapon || isThrowing)
        {
            _slotData = slot;
            _trackedIndex = slot.Index;
            UpdateSlot(slot);
        }
    }
    public void UpdateSlot(SlotData slot)
    {
        if (slot == null || slot.GetItem == null)
        {
            return;
        }

        bool isWeapon = slot.GetItem is WeaponDataSO;
        bool isThrowing = slot.GetItem is SoundItemSO;

        if (_quickSlotType == EQuickSlotType.Weapon && !isWeapon) { return; }
        if (_quickSlotType == EQuickSlotType.Throwing && !isThrowing) { return; }

        if (slot.Count <= 0 || (isWeapon && slot.Dur <= 0))
        {
            ClearSlot();
            return;
        }

        if (slot.Index != _trackedIndex)
        {
            return;
        }

        _slotData = slot;
        ItemDataSO data = _slotData.GetItem;

        _durSlider.gameObject.SetActive(isWeapon);

        if (data is WeaponDataSO weapon && _slotData.Dur > 0)
        {
            _durSlider.gameObject.SetActive(true);
            float maxDur = weapon.MaxDur;
            _durSlider.value = (float)_slotData.Dur / maxDur;
        }

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
