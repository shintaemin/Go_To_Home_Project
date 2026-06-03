using System;
using UnityEngine;

#region

#endregion


public class Player_Throwing : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _handPos;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _endPosPrefab;

    [Header("생성 / 캐싱 확인용")]
    #endregion

    #region 내부변수
    private Player_ItemEquip _equipCS;
    private SlotData _equipSlot;
    private Camera _cam;
    private GameObject _endPosObj;
    private bool _isTrowing = false;
    #endregion

    #region 외부 호출 함수
    public event Action OnSuccessThrowing;
    #endregion

    private void Awake()
    {
        if (_equipCS == null)
        {
            GUtill.TryGetCS(this, ref _equipCS);
        }
    }

    private void Start()
    {
        if (_equipCS != null)
        {
            _equipCS.OnItemEquip += OnEquipItem;
        }
    }

    private void OnDisable()
    {
        if (_equipCS != null)
        {
            _equipCS.OnItemEquip -= OnEquipItem;
        }
    }

    private void OnEquipItem(SlotData slot)
    {
        _equipSlot = slot;
    }

    private GameObject EndPosSpawn()
    {
        if (_endPosPrefab == null) { return null; }
        GameObject go = Instantiate(_endPosPrefab);
        return go;
    }

    private GameObject FindThrowingItem()
    {
        if (_equipSlot == null) { return null; }

        if (_equipSlot.Count <= 0) 
        { 
            if (Inventory_Manager.Instance != null)
            {
                Inventory_Manager.Instance.RemoveSlotData(_equipSlot);
            }
            _equipSlot = null;
            return null; 
        }

        GameObject go = Instantiate(_equipSlot.GetItem.Prefab);
        go.transform.SetParent(null, true);
        go.transform.position = _handPos.position;
        return go;
    }

    #region 외부 호출 함수
    public void TrowingPosUpdate(Vector3 mouseInput)
    {
        if (_cam == null) { _cam = Camera.main; }
        if (_equipSlot == null) { return; }

        Ray ray = _cam.ScreenPointToRay(mouseInput);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            if (_endPosObj == null)
            {
                _endPosObj = EndPosSpawn();
                _endPosObj?.transform.SetParent(null, true);
            }
            if (!_endPosObj.activeSelf) { _endPosObj.SetActive(true); }

            Vector3 point = hit.point;
            point.y += 0.1f;
            _endPosObj.transform.position = point;
            Quaternion rot = Quaternion.LookRotation(Vector3.up);
            _endPosObj.transform.rotation = rot;
            if (_isTrowing)
            {
                _isTrowing = false;
                GameObject go = FindThrowingItem();
                if (go != null)
                {
                    Debug.Log($"[{this.name}] : 너 들어오냐?");
                    Throwing_Obj throwingObj = null;
                    GUtill.TryGetCS(go, ref throwingObj);

                    if (_equipSlot == null || _equipSlot.GetItem is not SoundItemSO sound) { return; }

                    float range = sound.SoundRange;
                    throwingObj.SetTargetPos(point);
                    throwingObj.SetSoundRange(range);
                    throwingObj.OnThrowing();
                    OnSuccessThrowing?.Invoke();
                }
                OffThrowing();
            }
        }
    }

    public void OnTrowing()
    {
        _isTrowing = true;
    }
    public void OffThrowing()
    {
        if (!_isTrowing && (_endPosObj == null || !_endPosObj.activeSelf)) { return; }

        _endPosObj.SetActive(false);
    }
    #endregion
}
