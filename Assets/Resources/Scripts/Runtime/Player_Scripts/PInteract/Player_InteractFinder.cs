using UnityEngine;

#region 상호작용 오브젝트 검사
/*
 ▶ 할일
  - 오브젝트의 일정범위 안에 상호작용 가능한 오브젝트를 찾고 가장 가까운 오브젝트를 상호작용 대상 오브젝트로 등록
*/
#endregion


public class Player_InteractFinder : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private LayerMask _interactLayer;  // 검사할 레이어

    [Header("옵션")]
    [SerializeField] private float _range = 4.0f;       // 검사 범위
    [SerializeField] private float _distance = 1.0f;    // 후보로둘 최소값
    [SerializeField] private float _findInterval = 0.5f; // 지정한 시간에 한번씩 검사
    #endregion

    #region 내부 변수
    private Collider[] _reserver = new Collider[5];     // 충돌체를 담을 변수 - 검사마다 new 로 생성하기보다 미리 담아둘공간을 확보
    private float _nextFindTime = 0;    // 다음 검사 시간
    private Player_Interact _interact;
    #endregion

    private void Awake()
    {
        GUtill.TryGetCS(this, ref _interact);
    }

    private void Start()
    {
        _nextFindTime = Time.time + _findInterval; // 시작시 다음 검사시간을 현재 검사시간에서 interval 만큼 +
    }

    private void UpdateFinder()
    {
        if (Time.time >= _nextFindTime)
        {
            Finder();
        }
    }

    private void Finder()
    {
        IInteract target = null;
        float range = _distance;
        #region OverlapSphereNonAlloc
        /*
         ▶ NonAlloc
            메모리에 새로 할당하지 않겠다는 뜻
            
            OverlapSphere 를 사용하면 매 검사마다 새로운 Collider[] 을 메모리에 할당하는 방식이다 
            0.5초마다 검사할떄 이는 좋지않을거같아 NonAlloc 을 사용해 약간의 부하를 줄이기 위해 사용
        */
        #endregion
        // 오버랩 검사후 대상 의 갯수                  위치, 범위, 담을 Collider[], 레이어
        int search = Physics.OverlapSphereNonAlloc(transform.position, _range, _reserver, _interactLayer);

        for (int i = search - 1; i >= 0; i--)
        {
            if (_reserver[i] == null) { continue; }

            Collider col = _reserver[i];
            // 상호작용 인터페이스를 상속했다면
            if (col.TryGetComponent<IInteract>(out IInteract interact))
            {
                // 거리를 검사
                float targetDis = Distance(col.transform.position);
                // 지정된 최소 거리보다 작다면
                if (targetDis < range)
                {
                    // 최소거리를 현재거리로 할당 후 타겟으로 할당
                    range = targetDis;
                    target = interact;
                }
            }
        }

        // 최종 타겟 지정 + 다음 검사시간 업데이트
        _interact?.SetTarget(target);
        _nextFindTime = Time.time + _findInterval;
    }

    private float Distance(Vector3 target)
    {
        return (transform.position - target).sqrMagnitude;
    }

    #region 외부 호출 함수
    // 컨트롤러에서 업데이트 시킬 함수
    public void Find()
    {
        UpdateFinder();
    }
    #endregion
}
