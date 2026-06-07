using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Ending_Col : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private string _endingColTag = "Player";
    #endregion

    #region 이벤트
    public event Action OnEndingSuccess;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || !other.CompareTag(_endingColTag)) { return; }

        OnEndingSuccess?.Invoke();
    }
}
