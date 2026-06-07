using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region

#endregion


public class BloodScreen : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Image _image;
    [SerializeField] private float _hideTime = 3f;
    [SerializeField] private float _startAlpha = 1.0f;
    [SerializeField] private float _endAlpha = 0;
    #endregion

    #region 내부변수
    private Coroutine _bloodCo;
    private Player_Health _pHealthCS;
    #endregion

    private void Awake()
    {
        if (_image == null)
        {
            GUtill.TryGetCS(this, ref _image);
        }
        if (_pHealthCS == null)
        {
            _pHealthCS = FindFirstObjectByType<Player_Health>();
        }

        _bloodCo = null;
        Color c = _image.color;
        c.a = _endAlpha;
        _image.color = c;
    }

    private void Start()
    {
        if (_pHealthCS != null)
        {
            _pHealthCS.OnHit += ActiveBloodScreen;
        }
    }

    private void OnDestroy()
    {
        if (_bloodCo != null)
        {
            StopCoroutine(_bloodCo);
            _bloodCo = null;
        }
        if (_pHealthCS != null)
        {
            _pHealthCS.OnHit -= ActiveBloodScreen;
        }
    }

    private IEnumerator CoBloodScreenAnim()
    {
        if (_image == null) 
        {
            _bloodCo = null;
            yield break; 
        }

        float t = 0;
        Color c = _image.color;
        c.a = _startAlpha;
        float start = c.a;
        float end = _endAlpha;

        while (t < _hideTime)
        {
            t += Time.deltaTime;

            float progress = t / _hideTime;
            float current = Mathf.Lerp(start, end, progress);
            c.a = current;
            _image.color = c;
            yield return null;
        }

        c.a = end;
        _image.color = c;

        _bloodCo = null;
    }
    #region 외부 호출 함수
    public void ActiveBloodScreen()
    {
        if (_bloodCo != null)
        {
            StopCoroutine(_bloodCo);
            _bloodCo = null;
        }

        _bloodCo = StartCoroutine(CoBloodScreenAnim());
    }
    #endregion
}
