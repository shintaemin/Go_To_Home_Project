using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

#region 시네머신 시스템
/*
 ▶ 할일
  - 시네머신 가상 카메라들을 관리
  - 세부 설정을 코드로 제어하기보단 단순하게 보여질 카메라를 지정하는 방식으로 priority를 변경 하여 지정
  - 리스트와 Dictionary 로 열거형을 키로 보여줄 카메라를 찾아고 기본갑 10 이 메인카메라이며 먼저보여질 카메라는 20 아니면 0으로 priority 변경하여 카메라변경
*/
#endregion

[System.Serializable]
public class VirtualCamera
{
    [SerializeField] private CinemachineVirtualCamera _cam;
    [SerializeField] private EVirtualCamType _type;

    public CinemachineVirtualCamera Cam => _cam;
    public EVirtualCamType Type => _type;
}

public enum EVirtualCamType
{
    None = 0,
    Main,
    Inventory,
}

public class CineMashine_System : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private List<VirtualCamera> _virtualRegistry = new List<VirtualCamera>();
    [SerializeField] private EVirtualCamType _currentCam;

    [Header("옵션")]
    [SerializeField] private int _defaultPri = 10;
    [SerializeField] private int _viewPri = 20;
    [SerializeField] private int _hidePri = 0;
    #endregion

    #region 내부 변수
    private readonly Dictionary<EVirtualCamType, CinemachineVirtualCamera> _camDic = new Dictionary<EVirtualCamType, CinemachineVirtualCamera>();
    #endregion

    private void Awake()
    {
        if (_virtualRegistry.Count == 0) 
        {
            GUtill.Log($"[{this.name}] : 카메라 리스트 비어있음", EDebugType.Error);
            enabled = false;
            return; 
        }

        InitListToDic();
    }

    private void InitListToDic()
    {
        _camDic.Clear();

        foreach (var virtualCam in _virtualRegistry)
        {
            CinemachineVirtualCamera cam = virtualCam.Cam;
            EVirtualCamType type = virtualCam.Type;

            if (!_camDic.ContainsKey(type))
            {
                _camDic.Add(type, cam);
                _camDic[type].Priority = _hidePri;
            }
            else
            {
                GUtill.Log($"[{this.name}] : {type} 타입 으로 이미 등록된 카메라가 있음");
            }
        }

        if (_camDic.TryGetValue(EVirtualCamType.Main, out var mainCam))
        {
            mainCam.Priority = _defaultPri;
            _currentCam = EVirtualCamType.Main;
        }
        else
        {
            GUtill.Log($"[{this.name}] : 메인 캠 지정 안됨 MainCamera에 List 등록 확인");
        }
    }

    #region 외부 호출 함수
    public void SetVirtualCamViewer(EVirtualCamType type, bool view)
    {
        if (view)
        {
            if (_currentCam != type && _currentCam != EVirtualCamType.Main)
            {
                if (_camDic.TryGetValue(_currentCam, out var cam))
                {
                    cam.Priority = _hidePri;
                }
            }

            _camDic[type].Priority = _viewPri;
            _currentCam = type;
        }
        else
        {
            _camDic[type].Priority = _hidePri;
            _currentCam = EVirtualCamType.Main;
        }
    }
    #endregion
}
