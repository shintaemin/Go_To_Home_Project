using UnityEngine;

#region

#endregion


public class ESC_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _escObj;
    #endregion

    #region 내부변수
    private bool _isEnding;
    #endregion

    private void Awake()
    {
        if (_escObj == null)
        {
            _escObj = transform.GetChild(0).gameObject;
        }
        _isEnding = false;
    }

    #region 외부 호출 함수
    public void ActiveESCUI(bool active, bool ending = false)
    {
        if (_escObj == null || _isEnding) { return; }

        _isEnding = ending;
        if (_isEnding) { active = true; }

        _escObj.SetActive(active);

        Time.timeScale = active ? 0f : 1f;
    }

    public void ButtonEvent_GameEnd()
    {
        if (SceneLoadManager.Instance != null)
        {
            SceneLoadManager.Instance.GameEnd();
        }
    }

    public void ButtonEvent_Restart()
    {
        if (SceneLoadManager.Instance != null)
        {
            SceneLoadManager.Instance.RestartScene();
        }
    }

    public void ButtonEvent_GoToLobby()
    {
        if (SceneLoadManager.Instance != null)
        {
            SceneLoadManager.Instance.GoToLobby();
        }
    }

    public bool IsActiveESCUI => _escObj.activeSelf;
    #endregion
}
