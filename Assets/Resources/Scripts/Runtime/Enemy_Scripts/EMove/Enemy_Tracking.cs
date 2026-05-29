using System;
using System.Collections;
using UnityEngine;

#region 적 추적
/*
 ▶ 할일
  - 적 추적을 관리
*/
#endregion


public class Enemy_Tracking : MonoBehaviour, ISoundListener
{
    #region 인스펙터
    [SerializeField] private float _minDistance = 3f;
    [SerializeField] private float _maxDistance = 15f;
    [SerializeField] private LayerMask _searchLayer;
    [SerializeField] private float _trackingWaitTime = 0.5f;
    #endregion

    #region 내부 변수
    private Transform _target;
    private Vector3 _soundPos;
    private bool _isSoundTracking;
    private Coroutine _trackingAnimCo;
    private bool _isTrackingAnim = false;
    #endregion

    #region 이벤트
    public event Action<Vector3> OnSoundTracking;
    #endregion

    private Transform TargetSearch()
    {
        Vector3 origin = transform.position;
        Collider[] targetAray = Physics.OverlapSphere(origin, _minDistance, _searchLayer);

        if (targetAray.Length == 0) { return null; }

        Transform target = targetAray[0].transform;
        float minDis = Vector3.Distance(transform.position, targetAray[0].transform.position);

        for (int i = 0; i < targetAray.Length; i++)
        {
            float dis = Vector3.Distance(transform.position, targetAray[i].transform.position);
            if (dis < minDis)
            {
                minDis = dis;
                target = targetAray[i].transform;
            }
        }
        
        // 여기서 적감지 빨간 ! 띄우기

        return target;
    }

    private IEnumerator CoTrackingAnim()
    {
        _isTrackingAnim = true;
        float time = 0;

        // 여기서 ! 노란색 UI띄우기
        GUtill.Log($"[{this.name}] : 사운드 감지!");

        while (time < _trackingWaitTime)
        {
            time += Time.deltaTime;

            Vector3 target = _target == null ? _soundPos : _target.position;
            Vector3 dir = target - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                dir.Normalize();
                Quaternion start = transform.rotation;
                Quaternion end = Quaternion.LookRotation(dir);
                float t = 1.0f - MathF.Exp(-10f * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(start, end, t);
            }

            yield return null;
        }

        // 여기서 ! 제거
        GUtill.Log($"[{this.name}] : ! UI 제거!");

        _isTrackingAnim = false;
        _trackingAnimCo = null;
    }

    #region 외부 호출 함수
    public void TrackingStart()
    {
        if (_trackingAnimCo == null)
        {
            _trackingAnimCo = StartCoroutine(CoTrackingAnim());
        }
    }
    public void UpdateTarget()
    {
        if (_target != null) { return; }
        if (_isTrackingAnim) { return; }

        Transform target = TargetSearch();
        if (target != null)
        {
            _target = target;
            _isSoundTracking = false;
            _soundPos = Vector3.zero;
        }
    }
    public bool IsTargetTracking()
    {
        if (_target != null)
        {
            float dis = Vector3.Distance(transform.position, _target.position);
            if (dis <= _maxDistance) { return true; }

            TargetClear();
            return false;
        }

        if (_isSoundTracking)
        {
            return true;
        }

        return false;
    }
    public Vector3 GetTargetPos()
    {
        if (_target != null) { return _target.position; }
        if (_isSoundTracking) { return _soundPos; }
        return Vector3.zero;
    }
    public Transform GetTarget()
    {
        return _target;
    }
    public void TargetClear()
    {
        if (_trackingAnimCo != null)
        {
            StopCoroutine(_trackingAnimCo);
            _trackingAnimCo = null;
            _isTrackingAnim = false;
        }

        _target = null;
        _isSoundTracking = false;
        _soundPos = Vector3.zero;
    }
    public void OnSoundListen(Vector3 soundPos)
    {
        _isSoundTracking = true;
        _soundPos = soundPos;
        OnSoundTracking?.Invoke(_soundPos);
    }
    public bool HasLiveTarget()
    {
        if (_target == null) { return false; }
        return true;
    }
    public bool IsTrackingAnimPlaying()
    {
        return _isTrackingAnim;
    }
    #endregion
}
