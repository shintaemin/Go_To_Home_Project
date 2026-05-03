using UnityEngine;

#region 플레이어 인벤토리
/*
 ▶ 할일
  - 아이템 리스트를 들고있을 스크립트
  - 가방 오브젝트의 부모오브젝트 지정 및 애니메이션을 호출
*/
#endregion


public class Player_Inventory : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _bag;
    [SerializeField] private GameObject _spawnPosObj;
    [SerializeField] private GameObject _leftHandBackPackPos;
    //[SerializeField] private List<Item_Data> _items = new List<Item_Data>();

    [Header("생성 옵션")]
    [SerializeField] private Vector3 _spawnBagPos = new Vector3(0, 0.1f, 1);
    #endregion

    #region 내부변수
    private bool _isOpening = false;
    private Player_BackPack _backPack;
    private Player_Controller _controller;
    #endregion

    private void Awake()
    {
        if (_bag != null)
        {
            GUtill.TryGetCS(_bag, ref _backPack);
        }
        GUtill.TryGetCS(this, ref _controller);
    }

    private void SpawnPosUpdate()
    {
        if (_spawnPosObj == null) { return; }

        Transform origin = transform;
        Vector3 offset = _spawnBagPos;

        Vector3 spawnPos = origin.position +
            (origin.right * offset.x) + (origin.up * offset.y) + (origin.forward * offset.z);

        _spawnPosObj.transform.position = spawnPos;
        _spawnPosObj.transform.LookAt(this.transform.position);
        _spawnPosObj.transform.rotation = Quaternion.Euler(-90, _spawnPosObj.transform.eulerAngles.y, 0);
        _spawnPosObj.transform.localScale = Vector3.one * 2f;
    }

    private void BackPackPosUpdate(Transform parent)
    {
        _bag.transform.SetParent(parent);
        _bag.transform.localPosition = Vector3.zero;
        _bag.transform.localRotation = Quaternion.identity;
    }

    private void Open()
    {
        _controller.SetControllState(EControllMode.Inventory);
        SpawnPosUpdate();
        BackPackPosUpdate(_spawnPosObj.transform);

        if (_backPack != null) { _backPack.SetTriggerBackPack(true); }
        _isOpening = true;
    }

    private void Close()
    {
        if (_backPack != null) { _backPack.SetTriggerBackPack(false); }
    }

    #region 외부 호출 함수
    public void TryInventoryOpen()
    {
        if (_isOpening)
        {
            Close();
            return;
        }

        Open();
    }

    public void MoveToHand()
    {
        BackPackPosUpdate(_leftHandBackPackPos.transform);
        _isOpening = false;
        _controller.SetControllState(EControllMode.Playing);
    }
    #endregion
}
