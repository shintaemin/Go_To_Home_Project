using UnityEngine;

#region 마우스포인터 바라보기
/*
 ▶ 할일
  - 마우스 포인터 위치 방향을 정면으로 한다.
  - 뉴인풋 매니저 캐싱하고 매프레임 없데이트 되는 값을 확인하기
  - 매프레임 Instance 하지않고 미리 캐싱
  - 카메라에서 마우스 위치를 레이로 발사하고 높이를 플레이어 높이로 고정하여 마우스위치를 뒤집어짐 없이 바라보게하기
*/
#endregion

public class Player_LoockMousePointer : MonoBehaviour
{
    #region 인스펙터
    [Header("")]
    [SerializeField] private Camera _cam;
    [SerializeField] private PlayerInputManager _im;
    #endregion

    private void Awake()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
        }
    }

    private void Start()
    {
        if (_im == null)
        {
            if (PlayerInputManager.Instance != null)
            {
                _im = PlayerInputManager.Instance;
            }
            else
            {
                Debug.LogWarning($"[Player_LoockMousePointer] : 플레이어 인풋 매니저가 없음");
            }
        }
    }

    private void Update()
    {
        LookMousePoint();
    }

    private void LookMousePoint()
    {
        Ray ray = _cam.ScreenPointToRay(_im.GetMousePos);

        // 충돌체가 있다면 무조건 바라보기 (추후 레이어가 추가될 수 있음)
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.collider != null)
            {
                Vector3 target = hit.point;
                target.y = transform.position.y;
                transform.LookAt(target);
            }
        }
    }
}
