using UnityEngine;
using UnityEngine.Rendering.Universal; // DecalProjector 사용을 위함

#region 플레이어 사운드 데칼
/*
 ▶ 할일
  - 데칼을 Sound 를 통해 지정된 범위를 목표로 size 를 보간처리
*/
#endregion


public class Player_SoundDecal : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private GameObject _decalObj;
    [SerializeField] private DecalProjector _decal;
    [SerializeField] private float _smooth = 3f; // 부드러운 보간값을 위해
    #endregion

    #region 내부 변수
    private float _targetSize;
    #endregion

    private void Awake()
    {
        if (_decalObj != null && _decal == null)
        {
            if (!_decalObj.TryGetComponent<DecalProjector>(out _decal))
            {
                Debug.LogWarning($"[{this.name}] : 데칼 캐싱 실패");
                _decalObj.SetActive(false);
                enabled = false;
                return;
            }
        }
    }

    private void Update()
    {
        float target = _targetSize * 2f; // 지정된 사이즈의 * 2 (지름으로 사용하기 위함)
        float t = 1.0f - Mathf.Exp(-_smooth * Time.deltaTime); // 지수 보간

        Vector3 start = _decal.size; // 시작 사이즈
        Vector3 end = new Vector3(target, target, start.z); // 목표 사이즈 z 축은 깊이로 기존 사이즈.z 값 사용

        Vector3 current = Vector3.Lerp(start, end, t); // 최종 Lerp 보간

        _decal.size = current; // 사이즈 Update
    }

    #region 외부 호출 함수
    // 목표 범위 지정
    public void SetTargetRange(float range) 
    {
        if (_targetSize == range) { return; }

        _targetSize = range;
    }
    #endregion
}
