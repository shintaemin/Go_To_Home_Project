using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region

#endregion


public class Ending_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _endingRoot;

    [SerializeField] private Image _labelImage;
    [SerializeField] private TextMeshProUGUI _endingText;
    [SerializeField] private float _originAlpha = 0f;
    [SerializeField] private float _endAlpha = 0.8f;
    [SerializeField] private float _uiOpeningTimer = 2.0f;
    [SerializeField] private Color _successColor = Color.green;
    [SerializeField] private Color _failColor = Color.red;
    [SerializeField] private string _succsssText = "성 공";
    [SerializeField] private string _failText = "사 망";
    #endregion

    #region 내부변수
    private Coroutine _endingCo;
    #endregion

    private void Awake()
    {
        if (_endingRoot == null) { _endingRoot = transform.GetChild(0).gameObject; }
        if (_endingText == null) { _endingText = GetComponentInChildren<TextMeshProUGUI>(); }
        if (_labelImage == null) { GUtill.Log($"[{this.name}] : 라벨 없음"); return; }

        _endingRoot.SetActive(false);
    }

    private IEnumerator Co_EndingUI(bool isSuccess)
    {
        if (_endingRoot == null || _labelImage == null || _endingText == null) { yield break; }

        _endingRoot.SetActive(true);
        _labelImage.enabled = !isSuccess;

        Color c = _labelImage.color;
        Color endingTextColor = isSuccess ? _successColor : _failColor;
        string endText = isSuccess ? _succsssText : _failText;

        c.a = _originAlpha;
        endingTextColor.a = _originAlpha;
        _labelImage.color = c;
        _endingText.color = endingTextColor;
        _endingText.text = endText;

        float t = 0;
        while (t < _uiOpeningTimer)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / _uiOpeningTimer);

            c.a = Mathf.Lerp(_originAlpha, _endAlpha, ratio);
            endingTextColor.a = c.a;

            _labelImage.color = c;
            _endingText.color = endingTextColor;

            yield return null;
        }
        c.a = _endAlpha;
        endingTextColor.a = c.a;
        _labelImage.color = c;
        _endingText.color = endingTextColor;

        _endingCo = null;
    }

    #region 외부 호출 함수
    public void EndingUIActive(bool isSuccess)
    {
        if (_endingCo != null)
        {
            _endingCo = null;
        }

        _endingCo = StartCoroutine(Co_EndingUI(isSuccess));
    }
    #endregion
}
