using UnityEngine;
using UnityEngine.SceneManagement;

#region

#endregion


public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }

    #region 인스펙터
    #endregion

    #region 내부변수
    private int _maxSceneIndex;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            enabled = false;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _maxSceneIndex = SceneManager.sceneCountInBuildSettings;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void SetSceneLoad(int index)
    {
        if (index >= 0 && index < _maxSceneIndex)
        {
            SceneManager.LoadScene(index);
        }
        else
        {
            GUtill.Log($"[{this.name}] : {index}에는 씬이 없음");
        }
    }

    #region 외부 호출 함수
    public void GameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Restart()
    {
        RestartScene();
    }

    public void GoToLobby()
    {
        Time.timeScale = 1.0f;
        SetSceneLoad(0);
    }

    public void MainGame()
    {
        Time.timeScale = 1.0f;
        SetSceneLoad(1);
    }

    public void RestartScene()
    {
        Time.timeScale = 1.0f;
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SetSceneLoad(currentScene);
    }
    #endregion
}
