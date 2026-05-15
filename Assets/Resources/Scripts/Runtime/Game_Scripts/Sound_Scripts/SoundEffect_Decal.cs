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
    [SerializeField] private GameObject _decalObj; // 데칼의 위치 이동 및 컴포넌트 캐싱을 위함
    [SerializeField] private DecalProjector _decal; // 쉐이더 그래프 데칼
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
    public Vector3 Pos // 외부에서 위치 지정 하도록
    {
        get { return  _pos; }
        set 
        { 
            _pos = value;
            if (_decalObj != null) { _decalObj.transform.position = _pos; }
        }
    }

    public float Range // 외부에서 범위 지정 하도록
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
        
        _aliveTime += Time.deltaTime; // 생존시간을 매프레임 ++
        
        float range = Range * 2f; // Range 는 반지름으로 최종사이즈 지정
        float t = 1.0f - Mathf.Exp(-_smooth * Time.deltaTime); // 지수 보간

        Vector3 start = _decal.size; // 현재 데칼사이즈
        Vector3 target = new Vector3(range, range, start.z); // 목표 데칼 사이즈
        Vector3 Lerp = Vector3.Lerp(start, target, t); // 보간
        _decal.size = Lerp; // 최종 지정

        float progress = Mathf.Clamp01(_aliveTime / _returnTime); // 데칼 진행도
        _decal.fadeFactor = 1.0f - progress; // 점차 사라지도록 Fade 처리

        float dis = (target - Lerp).sqrMagnitude; // 사이즈 비교를 위함
        if (dis <= 0.001f || _aliveTime >= _returnTime) // 목표 사이즈에 가깝거나, 돌아갈 시간이되면
        {
            if (SoundEffect_PoolManager.Instance != null) 
            {
                _decal.size = target; // 명시적으로 최종위치라 지정
                _isPlaying = false; // 다음 프레임에 실행되지않도록
                SoundEffect_PoolManager.Instance.ReturnToPool(this); // 풀로 복귀
            }
        }
    }

    #region 외부 호출 함수
    public void InitPlay() // 데칼 설정 완료시 호출함수
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
        _decal.size = new Vector3(0, 0, _decal.size.z); // 사이즈 초기화
        _decal.fadeFactor = 1.0f; // Fade 1설정
        _aliveTime = 0; // 생존시간 초기화
        _isPlaying = true ; // 데칼 플레이중
    }
    #endregion
}
