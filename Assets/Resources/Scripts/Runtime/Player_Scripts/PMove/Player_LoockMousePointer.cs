using UnityEngine;

#region 마우스포인터 바라보기
/*
 ▶ 할일
  - 마우스 포인터 위치 방향을 정면으로 한다.
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
