using UnityEngine;

#region

#endregion


public class Throwing_Obj : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Vector3 _target;
    [SerializeField] private float _decalRange;
    [SerializeField] private float _throwForce = 5f;
    [SerializeField] private float _upwardAngle = 45f;
    #endregion

    #region 내부변수
    private Rigidbody _rb;
    private bool _isThrowing;
    #endregion

    private void Awake()
    {
        GUtill.TryGetCS(this, ref _rb);

        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }
        _isThrowing = false;
    }

    private void Update()
    {
        if (!_isThrowing) { return; }

        // [실시간 거리 체크] 내 현재 3D 위치가 마우스 목적지 좌표와 충분히 가까워졌는가?
        if (Vector3.Distance(transform.position, _target) <= 0.2f)
        {
            OnArrival();
        }
    }

    private float CalculateForceForDistance(float horizontalDistance, float heightDifference)
    {
        float angleRad = _upwardAngle * Mathf.Deg2Rad;
        float g = Mathf.Abs(Physics.gravity.y);

        float top = g * horizontalDistance * horizontalDistance;
        float bottom = 2 * Mathf.Pow(Mathf.Cos(angleRad), 2) * (horizontalDistance * Mathf.Tan(angleRad) - heightDifference);

        if (bottom <= 0)
        {
            return 10f;
        }

        float requiredSpeed = Mathf.Sqrt(top / bottom);

        return requiredSpeed * _rb.mass;
    }

    // 목적지 근접 감지 시 실행될 착지 함수
    private void OnArrival()
    {
        _isThrowing = false;

        GUtill.Log($"[{this.name}] : 목적지 근접 안착 완료! 사운드 범위 반경 {_decalRange} 발동");
        if(SoundEffect_PoolManager.Instance != null)
        {
            Vector3 pos = transform.position;
            pos.y += 0.1f;
            SoundEffect_PoolManager.Instance.SpawnEffect(pos, _decalRange);
        }
        Destroy(gameObject);
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
    public void OnThrowing()
    {
        if (_rb == null) 
        {
            GUtill.TryGetCS(this, ref _rb);
        }

        _rb.isKinematic = false;
        _rb.useGravity = true;
        _isThrowing = true;

        Vector3 directionXZ = (_target - transform.position);
        directionXZ.y = 0;
        if (directionXZ != Vector3.zero)
        {
            transform.forward = directionXZ.normalized;
        }

        transform.Rotate(Vector3.left, _upwardAngle);

        float requiredForce = CalculateForceForDistance(directionXZ.magnitude, _target.y - transform.position.y);
        _throwForce = requiredForce; // 인스펙터 실시간 확인용 동기화

        _rb.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
    }
    #endregion
}
