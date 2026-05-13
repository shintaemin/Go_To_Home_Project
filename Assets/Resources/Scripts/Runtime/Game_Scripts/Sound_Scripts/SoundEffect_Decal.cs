using UnityEngine;
using UnityEngine.Rendering.Universal;

#region 사운드 이펙트 데칼
/*
 ▶ 할일
  - 사운드 범위 데칼을 재생시킬 스크립트
  - 위치랑 범위를 지정받게하고 SoundEmition 함수에서 해당 위치에서 범위만큼 사운드 데칼 재생
*/
#endregion

public class SoundEffect_Decal : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _decalObj;
    [SerializeField] private DecalProjector _decal;
    [SerializeField] private float _smooth = 3f;

    [Header("시간 옵션")]
    [SerializeField] private float _aliveTime;
    [SerializeField] private float _returnTime = 2.0f;
    #endregion
    #region 내부 변수
    private Vector3 _pos;
    private float _range;
    private bool _isPlaying;
    #endregion
    #region 프로퍼티
    public Vector3 Pos
    {
        get { return  _pos; }
        set 
        { 
            _pos = value;
            if (_decalObj != null) { _decalObj.transform.position = _pos; }
        }
    }

    public float Range
    {
        get { return _range; }
        set { _range = value; }
    }
    #endregion

    private void Awake()
    {
        if (_decal == null && _decalObj != null)
        {
            GUtill.TryGetCS(_decalObj, ref _decal);

            if (_decal == null)
            {
                GUtill.Log($"[{this.name}] : 데칼 캐싱 실패", EDebugType.Warn);
                enabled = false;
                return;
            }
        }
    }

    private void Update()
    {
        if (_decal == null || !_isPlaying) { return; }
        
        _aliveTime += Time.deltaTime;

        float range = Range * 2f;
        float t = 1.0f - Mathf.Exp(-_smooth * Time.deltaTime); // 지수 보간

        Vector3 start = _decal.size;
        Vector3 end = new Vector3(range, range, start.z);
        Vector3 Lerp = Vector3.Lerp(start, end, t);
        _decal.size = Lerp;

        float dis = (end - Lerp).sqrMagnitude;
        if (dis <= 0.001f || _aliveTime >= _returnTime)
        {
            if (SoundEffect_PoolManager.Instance != null)
            {
                _decal.size = end;
                _isPlaying = false;
                SoundEffect_PoolManager.Instance.ReturnToPool(this);
            }
        }
    }

    #region 외부 호출 함수
    public void InitPlay()
    {
        if (_decal == null && _decalObj != null)
        {
            GUtill.TryGetCS(_decalObj, ref _decal);

            if (_decal == null)
            {
                GUtill.Log($"[{this.name}] : 데칼 캐싱 실패", EDebugType.Warn);
                enabled = false;
                return;
            }
        }

        _decalObj.transform.position = _pos; // Pos 에서 작업하지만 혹시몰라 한번더
        _decal.size = new Vector3(0, 0, _decal.size.z);
        _aliveTime = 0;
        _isPlaying = true ;
    }
    #endregion
}
