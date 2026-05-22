using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 건물 층 트리거
/*
 ▶ 할일
  - 인스펙터에 지정된 오브젝트를 충돌감지시 키고 끄기
*/
#endregion


public class Building_FloorTrigger : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private string _colTag = "Player";
    [SerializeField] private GameObject[] _activeFloor = new GameObject[0];
    [SerializeField] private GameObject[] _deactiveFloor = new GameObject[0];
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_colTag)) { return; }

        for (int i = 0; i < _deactiveFloor.Length; i++)
        {
            if (_deactiveFloor[i] == null) { continue; }
            if (!_deactiveFloor[i].activeSelf) { continue; }

            _deactiveFloor[i].SetActive(false);
        }

        for (int i = 0; i < _activeFloor.Length; i++)
        {
            if (_activeFloor[i] == null) { continue; }
            if (_activeFloor[i].activeSelf) { continue; }

            _activeFloor[i].SetActive(true);
        }
    }
}
