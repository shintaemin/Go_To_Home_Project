using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Player_Throwing : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Transform _handPos;
    [SerializeField] private LayerMask _groundLayer;

    [Header("생성 / 캐싱 확인용")]
    [SerializeField] private Throwing_Obj _throwingCS;
    [SerializeField] private GameObject _endPosPrefab;
    #endregion

    #region 내부변수
    private Camera _cam;
    private GameObject _endPosObj;
    private bool _isTrowing = false;
    #endregion

    private void OnDisable()
    {
        _throwingCS = null;
    }

    private GameObject EndPosSpawn()
    {
        if (_endPosPrefab == null) { return null; }
        GameObject go = Instantiate(_endPosPrefab);
        return go;
    }

    private GameObject FindThrowingItem()
    {
        if (_handPos == null) { return null; }

        GameObject go = _handPos.GetChild(0).gameObject;
        GUtill.TryGetCS(go, ref _throwingCS);
        if (_throwingCS == null)
        {
            GUtill.Log($"[{this.name}] : {go.name} 에 {_throwingCS.name} 이 없음");
            return null;
        }
        return go;
    }

    #region 외부 호출 함수
    public void TrowingPosUpdate(Vector3 mouseInput)
    {
        if (_cam == null) { _cam = Camera.main; }

        Ray ray = _cam.ScreenPointToRay(mouseInput);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            if (_endPosObj == null)
            {
                _endPosObj = EndPosSpawn();
                _endPosObj?.transform.SetParent(null, true);
            }
            Vector3 point = hit.point;
            point.y = 0.1f;
            _endPosObj.transform.position = point;
            _endPosObj.transform.LookAt(_endPosObj.transform, Vector3.up);
            if (_isTrowing)
            {
                GameObject go = FindThrowingItem();
                if (go != null && _throwingCS != null)
                {
                    _throwingCS.SetTargetPos(point);
                }
                _isTrowing = false;
            }
        }
    }

    public void OnTrowing()
    {
        _isTrowing = true;
    }
    #endregion
}
