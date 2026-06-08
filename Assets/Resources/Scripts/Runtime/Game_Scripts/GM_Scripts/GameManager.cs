using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

#region

#endregion


public class GameManager : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Ending_Col _endingCol;
    [SerializeField] private Player_Health _pHealthCS;
    [SerializeField] private Player_Controller _controllerCS;
    [SerializeField] private CineMashine_System _camSystem;

    [SerializeField] private float _endingWaitTime = 5.0f;
    #endregion

    #region 내부 변수
    private Coroutine _endingCo;
    private WaitForSeconds _endingWait;
    private bool _isEnding = false;
    #endregion

    private void Awake()
    {
        if (_endingCol == null) { _endingCol = FindFirstObjectByType<Ending_Col>(); }
        if (_pHealthCS == null) { _pHealthCS = FindFirstObjectByType<Player_Health>(); }
        if (_controllerCS == null) { _controllerCS = FindFirstObjectByType<Player_Controller>(); }
        if (_camSystem == null && Camera.main != null) { _camSystem = Camera.main.GetComponent<CineMashine_System>();
        }
        _endingWait = new WaitForSeconds(_endingWaitTime);
        _isEnding = false;
    }

    private void Start()
    {
        if (_endingCol != null) { _endingCol.OnEndingSuccess += EndingSuccess; }
        if (_pHealthCS != null) { _pHealthCS.OnDead += EndingFail; }
        _endingCo = null;
    }

    private void OnDestroy()
    {
        if (_endingCol != null) { _endingCol.OnEndingSuccess -= EndingSuccess; }
        if (_pHealthCS != null) { _pHealthCS.OnDead -= EndingFail; }
        if (_endingCo != null) { _endingCo = null; }
    }

    private IEnumerator CoEndingWait(bool isSuccess)
    {
        if (UI_Manager.Instance == null || _isEnding) { yield break; }

        _isEnding = true;
        if (!isSuccess && _camSystem != null)
        {
            _camSystem.SetVirtualCamViewer(EVirtualCamType.Player_Death, true);
        }
        UI_Manager.Instance.EndingUISetActive(isSuccess);

        yield return _endingWait;

        if (isSuccess)
        {
            if (SceneLoadManager.Instance != null)
            {
                _endingCo = null;
                SceneLoadManager.Instance.GoToLobby(3f);
                _isEnding = false;
                yield break;
            }
        }

        UI_Manager.Instance.EscInputActive(true);
        _isEnding = false;
        _endingCo = null;
    }

    private void EndingSuccess()
    {
        if (_endingCol != null)
        {
            _endingCo = null;
        }

        // 자연스럽게 어두워지며 로비로 씬전환
        GUtill.Log($"[{this.name}] : 성공!!!");
        _controllerCS.MovementState = EMovementState.End;
        _endingCo = StartCoroutine(CoEndingWait(true));
    }
    private void EndingFail()
    {
        if ( _endingCol != null )
        {
            _endingCo = null;
        }
        // 로비 or 게임종료 UI 띄우기
        GUtill.Log($"[{this.name}] : 실패!!!");
        _endingCo = StartCoroutine(CoEndingWait(false));
    }
}
