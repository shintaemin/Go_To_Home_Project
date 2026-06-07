using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#region

#endregion


public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }

    #region 인스펙터
    [Header("페이드 세팅")]
    [SerializeField] private CanvasGroup _fadeGroup;
    [SerializeField] private float _defaultFadeTime = 1.0f;
    #endregion

    #region 내부변수
    private int _maxSceneIndex;
    private bool _isFading = false;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        Instance = this;

        if (_fadeGroup != null)
        {
            _fadeGroup.alpha = 1.0f;
            _fadeGroup.blocksRaycasts = false;
            StartCoroutine(FadeInRoutine(_defaultFadeTime));
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private IEnumerator FadeAndLoadSceneRoutine(int index, float fadeTime)
    {
        _isFading = true;
        _fadeGroup.blocksRaycasts = true;

        float t = 0f;
        float start = 0f;
        float end = 1f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            _fadeGroup.alpha = Mathf.Lerp(start, end, t / fadeTime);
            yield return null;
        }
        _fadeGroup.alpha = end;

        _maxSceneIndex = SceneManager.sceneCountInBuildSettings;
        if (index >= 0 && index < _maxSceneIndex)
        {
            SceneManager.LoadScene(index);
            yield return null;
        }
        else
        {
            GUtill.Log($"[{this.name}] : {index}에는 씬이 없음");
            _fadeGroup.alpha = start;
            _fadeGroup.blocksRaycasts = false;
            _isFading = false;
            yield break;
        }

        t = 0f;
        start = 1f;
        end = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            _fadeGroup.alpha = Mathf.Lerp(start, end, t / fadeTime);
            yield return null;
        }
        _fadeGroup.alpha = end;
        _fadeGroup.blocksRaycasts = false;
        _isFading = false;
    }

    private IEnumerator FadeInRoutine(float fadeTime)
    {
        float t = 0f;
        float start = 1f;
        float end = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            _fadeGroup.alpha = Mathf.Lerp(start, end, t / fadeTime);
            yield return null;
        }
        _fadeGroup.alpha = end;
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

    public void Restart(float fadeTime = -1f)
    {
        RestartScene(fadeTime);
    }

    public void GoToLobby(float fadeTime = -1f)
    {
        if (_isFading) return;

        float t = (fadeTime < 0f) ? _defaultFadeTime : fadeTime;

        Time.timeScale = 1.0f;
        StartCoroutine(FadeAndLoadSceneRoutine(0, t));

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnLobbyBgmPlay();
        }
    }

    public void MainGame(float fadeTime = -1f)
    {
        if (_isFading) return;

        float t = (fadeTime < 0f) ? _defaultFadeTime : fadeTime;

        Time.timeScale = 1.0f;
        StartCoroutine(FadeAndLoadSceneRoutine(1, t));

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnInGameBgmPlay();
        }
    }

    public void RestartScene(float fadeTime = -1f)
    {
        if (_isFading) return;

        float t = (fadeTime < 0f) ? _defaultFadeTime : fadeTime;

        Time.timeScale = 1.0f;
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(FadeAndLoadSceneRoutine(currentScene, t));
    }
    #endregion
}
