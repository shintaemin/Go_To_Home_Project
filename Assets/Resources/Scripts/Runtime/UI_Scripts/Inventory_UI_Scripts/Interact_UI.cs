using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#region

#endregion


public class Interact_UI : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _interactPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _viewText;
    #endregion

    #region 내부변수
    private Transform _target;
    private RectTransform _panelRect;
    private Camera _cam;
    #endregion

    private void Awake()
    {
        if (_cam == null) { _cam = Camera.main; }
        if (_interactPanel == null) 
        { 
            GUtill.Log($"[{this.name}] : 상호작용 UI 오브젝트 없음"); 
            enabled = false; 
            return; 
        }
        if (_panelRect == null) { GUtill.TryGetCS(_interactPanel, ref _panelRect); }
    }

    private void LateUpdate()
    {
        if (_interactPanel == null || !_interactPanel.activeSelf || _target == null) { return; }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);

        _panelRect.position = screenPos;
    }
    private void SetInteractViewText(string name, string view)
    {
        if (_nameText == null || _viewText == null) { return; }

        _nameText.text = name;
        _viewText.text = view;
    }
    #region 외부 호출 함수
    public void SetActiveInteractView(bool active, string name = "", string view = "")
    {
        if (_interactPanel == null) { return; }

        _interactPanel.SetActive(active);

        if (!active) { return; }
        if (name == "" || view == "") { return; }

        SetInteractViewText(name, view);
    }
    public void SetTarget(Transform target)
    {
        if (target == null) 
        {
            TargetClear();
            return; 
        }

        _target = target; 
        Canvas.ForceUpdateCanvases();
    }
    public void TargetClear()
    {
        if (_interactPanel == null) { return; }

        _target = null;
        _interactPanel.SetActive(false);
    }
    #endregion
}
