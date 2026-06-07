using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class GameManager : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Ending_Col _endingCol;
    [SerializeField] private Player_Health _pHealthCS;
    #endregion

    #region

    #endregion

    private void Awake()
    {
        if (_endingCol == null) { _endingCol = FindFirstObjectByType<Ending_Col>(); }
        if (_pHealthCS == null) { _pHealthCS = FindFirstObjectByType<Player_Health>(); }
    }

    private void Start()
    {
        if (_endingCol != null && _pHealthCS != null)
        {
            _endingCol.OnEndingSuccess += EndingSuccess;
            _pHealthCS.OnDead += EndingFail;
        }
    }

    private void OnDestroy()
    {
        if (_endingCol != null && _pHealthCS != null)
        {
            _endingCol.OnEndingSuccess -= EndingSuccess;
            _pHealthCS.OnDead -= EndingFail;
        }
    }

    private void EndingSuccess()
    {
        // 자연스럽게 어두워지며 로비로 씬전환
        GUtill.Log($"[{this.name}] : 성공!!!");
    }
    private void EndingFail()
    {
        // 로비 or 게임종료 UI 띄우기
        GUtill.Log($"[{this.name}] : 실패!!!");
    }
}
