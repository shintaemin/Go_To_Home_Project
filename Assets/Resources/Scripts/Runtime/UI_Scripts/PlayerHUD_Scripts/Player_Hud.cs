using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

#region

#endregion


public class Player_Hud : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Slider _hp;
    [SerializeField] private Slider _stemina;
    #endregion

    #region 내부변수
    private Player_DataSO _data;
    #endregion

    private void Awake()
    {
        if (_hp == null || _stemina == null)
        {
            GUtill.Log($"[{this.name}] : 슬라이더 미연결");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (Player_DataManager.Instance != null)
        {
            _data = Player_DataManager.Instance.GetDataSO;
            if (_data != null)
            {
                _data.OnHPUpdate += OnHpUpdate;
                _data.OnSteminaUpdate += OnSteminaUpdate;
            }
        }
        _hp.gameObject.SetActive(false);
        _stemina.gameObject.SetActive(false);
    }

    private void OnHpUpdate(int value)
    {
        if (_hp == null || _data == null) { return; }

        int max = _data.GetMaxHP;
        int current = value;
        float view = (float)current / max;

        _hp.value = view;
    }

    private void OnSteminaUpdate(int value)
    {
        if (_stemina == null || _data == null) { return; }

        int max = _data.GetMaxStemina;
        int current = value;
        float view = (float)current / max;

        _stemina.value = view;
    }

    public void ActivePlayerHud(bool active)
    {
        if (_stemina != null && _hp != null)
        {
            _hp.gameObject.SetActive(active);
            _stemina.gameObject.SetActive(active);
        }
    }
}
