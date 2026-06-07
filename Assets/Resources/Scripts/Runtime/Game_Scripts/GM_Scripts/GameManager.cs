using System.Collections;
using UnityEngine;

#region

#endregion


public class GameManager : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Ending_Col _endingCol;
    [SerializeField] private Player_Health _pHealthCS;
    [SerializeField] private Player_Controller _controllerCS;
    #endregion

    #region 내부 변수
    private Coroutine _endingCo;
    #endregion

    private void Awake()
    {
        if (_endingCol == null) { _endingCol = FindFirstObjectByType<Ending_Col>(); }
        if (_pHealthCS == null) { _pHealthCS = FindFirstObjectByType<Player_Health>(); }
        if (_controllerCS == null) { _controllerCS = FindFirstObjectByType<Player_Controller>(); }
    }

    private void Start()
    {
        if (_endingCol != null && _pHealthCS != null)
        {
            _endingCol.OnEndingSuccess += EndingSuccess;
            _pHealthCS.OnDead += EndingFail;
        }
        _endingCo = null;
    }

    private void OnDestroy()
    {
        if (_endingCol != null && _pHealthCS != null)
        {
            _endingCol.OnEndingSuccess -= EndingSuccess;
            _pHealthCS.OnDead -= EndingFail;
        }
        if (_endingCo != null)
        {
            _endingCo = null;
        }
    }

    private IEnumerator CoEndingWait()
    {
        float t = 0f;

        while (t < 3f)
        {
            t += Time.deltaTime;
            if (t >= 3f) { break; }
            yield return null;
        }

        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.EscInputActive(true);
        }

        _endingCo = null;
    }

    private void EndingSuccess()
    {
        // 자연스럽게 어두워지며 로비로 씬전환
        GUtill.Log($"[{this.name}] : 성공!!!");
        _controllerCS.MovementState = EMovementState.End;
        if (SceneLoadManager.Instance != null)
        {
            SceneLoadManager.Instance.GoToLobby(3f);
        }    
    }
    private void EndingFail()
    {
        if ( _endingCol != null )
        {
            _endingCo = null;
        }
        // 로비 or 게임종료 UI 띄우기
        GUtill.Log($"[{this.name}] : 실패!!!");
        _endingCo = StartCoroutine(CoEndingWait());
    }
}
