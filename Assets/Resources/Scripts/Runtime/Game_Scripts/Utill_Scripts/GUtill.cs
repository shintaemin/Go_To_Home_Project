using System.Diagnostics;
using UnityEngine;

#region 게임 유틸리티
/*
 ▶ 할일
  - 개발에 필요한 캐싱, 디버그 등 사용할 스크립트
  - 구현을 하려고한 이유
  - 각 스크립트에서 _log 변수를 활용하여 디버깅중인데 이를 하나로 모으고 빌드시 편하게 빼기위함
*/
#endregion
public enum EDebugType
{
    None = 0,
    Log,
    Warn,
    Error,
}

public static class GUtill
{
    public static bool IsLogView = true;

    [Conditional("UNITY_EDITOR")]
    public static void Log(string str, EDebugType type = EDebugType.Log)
    {
        if (!IsLogView) { return; }

        if (str == string.Empty || str == "")
        {
            UnityEngine.Debug.LogWarning("[GUtill] : 텍스트 입력 없음");
            return;
        }

        switch(type)
        {
            case EDebugType.Log:    UnityEngine.Debug.Log(str);         break;
            case EDebugType.Error:  UnityEngine.Debug.LogError(str);    break;
            case EDebugType.Warn:   UnityEngine.Debug.LogWarning(str);  break;
        }
    }

    public static void TryGetCS<T>(GameObject obj , ref T component, string str = "") where T : Component
    {
        if (obj == null) 
        {
            Log($"[GUtil] : 게임 오브젝트 없음", EDebugType.Warn);
            return; 
        }
        if (component != null)
        {
            Log($"[GUtill] : {component.name} 이미 있음", EDebugType.Warn);
            return;
        }

        if (obj.TryGetComponent<T>(out component))
        {
            if (str != "") { Log(str); }
        }
        else
        {
            Log($"[GUtill] : {obj.name} 에 컴포넌트가 없음", EDebugType.Error);
        }
    }

    public static void TryGetCS<T>(Component cs, ref T component, string str = "") where T : Component
    {
        TryGetCS(cs.gameObject, ref component, str);
    }

    public static void LogView(bool view)
    {
        IsLogView = view;
    }
}
