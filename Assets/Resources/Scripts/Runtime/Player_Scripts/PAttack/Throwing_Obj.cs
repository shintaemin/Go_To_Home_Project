using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Throwing_Obj : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Vector3 _target;
    [SerializeField] private float _decalRange;

    [SerializeField] private float _throwingSpeed = 10f;
    #endregion

    #region 내부변수

    #endregion

    private void Update()
    {
        
    }

    #region 외부 호출 함수
    public void SetTargetPos(Vector3 target)
    {
        if (_target == target) { return; }

        _target = target;
    }
    public void SetSoundRange(float range)
    {
        if (_decalRange == range) { return; }

        _decalRange = range;
    }
    #endregion
}
