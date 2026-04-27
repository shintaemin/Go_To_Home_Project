using UnityEngine;

#region 플레이어 이동
/*
 ▶ 할일
  - 입력에따른 플레이어 이동 로직
  - 캐릭터 컨트롤러를 활용한 간단한 중력 구현
  - 웅크리기, 달리기등 에따른 속도 변화를 주기 위한 Multiplier 변수를 두고 속도를 제어
  - Data가 없는 경우나 인풋매니저가 없는 경우에 따라 의존성을 낮게 유지하려 노력하기

 ※ 현재 구조로 생각한 이유 ※
  Rigidbody 를 통한 물리 연산 및 예외처리를 직접 하기 보다 효율적이라 판단 했음

  01. 땅에 닿았는지 확인 유무 - 별도로 RayCast 로직을 구현하는 대신 캐릭터컨트롤러의 IsGrounde 변수 하나로 해결
  02. 물리 충돌 - 별도로 땅인지 오브젝트인지 검사하지않고 캐릭터 컨트롤러 자체에 물리충돌을 사용
  03. 예외 처리 - 물리연산에 경우 대부분의 상황에서 직접 연산하고 예외처리를 해야하지만 현재 물리학에 역량에 있어 구현이 오래걸릴것으로 판단
*/
#endregion


public class Player_Move : MonoBehaviour
{
    #region 인스펙터
    [Header("캐릭터 컨트롤러")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerInputManager _im;
    [SerializeField] private float _runMultiplier = 1.5f;
    [SerializeField] private float _crouchMultiplier = 0.3f;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _speedParam;

    [Header("옵션")]
    [SerializeField] private bool _isCrouch = false;
    [SerializeField] private bool _isRun = false;
    [SerializeField] private float _paramChangedSpeed = 1.0f;
    [SerializeField] private float _defaultSpeed = 5.0f; // 기본 속도값 Data를 불러오지 못했을경우를 대비 의존성 ↓↓
    #endregion

    #region 내부변수
    private const float _gravity = -9.81f; // 중력 대표값
    private Vector3 _verticalVec; // 중력 적용 벡터 변수
    #endregion

    private void Awake()
    {
        if (_controller == null)
        {
            if (!TryGetComponent<CharacterController>(out _controller))
            {
                Debug.LogWarning($"[Player_MoveScripts] : 캐릭터 컨트롤러 캐싱 실패 <인스펙터 확인>");
                enabled = false;
                return;
            }
        }
    }

    private void Start()
    {
        if (Player_DataManager.Instance != null)
        {
            float speed = Player_DataManager.Instance.GetDataSO.GetSpeed;
            _moveSpeed = speed;
        }
        else
        {
            _moveSpeed = _defaultSpeed;
            Debug.LogWarning($"[Player_MoveScripts] : Player_DataManager 를 찾지 못함 기본값 사용 속도 : {_moveSpeed}");
        }

        if (_im == null)
        {
            if (PlayerInputManager.Instance != null)
            {
                _im = PlayerInputManager.Instance;
            }
            else
            {
                Debug.LogWarning($"[Player_MoveScript] : 플레이어 인풋 매니저가 없음");
            }
        }
    }

    private void Update()
    {
        // 각 입력 확인 - 의존성을 낮추기 위한 3항연산자 사용
        Vector2 move = _im != null ? _im.GetMoveInput : new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool run = _im != null ? _im.GetRunInput : Input.GetKey(KeyCode.LeftShift);
        bool crouch = _im != null ? _im.GetCrouchInput : Input.GetKey(KeyCode.Space);

        // 입력 방향 Vector3
        Vector3 dir = new Vector3(move.x, 0, move.y);

        SetRun(run);            // 달리기 입력 전달
        SetCrouch(crouch);      // 웅크리기 입력 전달

        MoveUpdata(dir);        // 이동
        SpeedParamUpdate(dir);  // 스피드 파라미터 업데이트 - 부드러운 애니메이션전환을 위해 상태전달이아닌 Update로 값변경방식
    }

    // 이동 업데이트
    private void MoveUpdata(Vector3 dir)
    {
        if (_controller == null) { return; }

        Vector3 vertical = GravityUpdate(dir); // 중력 값 받아오기
        Vector3 moveDir = Vector3.ClampMagnitude(dir, 1); // 수평 이동 방향 (대각선이동 속도 제한)

        // 속도 지정
        float speed = _moveSpeed * (_isRun ? _runMultiplier : _isCrouch ? _crouchMultiplier : 1) * Time.deltaTime;

        // 좌우 * 속도 + 가져온 중력값
        Vector3 current = (moveDir * speed) + vertical;

        _controller.Move(current); // 이동
    }

    // 중력 업데이트
    private Vector3 GravityUpdate(Vector3 dir)
    {
        // 땅에 닿아있고 방향 y 가 0 보다 작다면 -2정도로 지정
        if (_controller.isGrounded && dir.y < 0.0f)
        {
            _verticalVec.y = -2.0f;
        }

        // 매프레임 중력값(-9.81) 만큼 업데이트
        _verticalVec.y += _gravity * Time.deltaTime;

        return _verticalVec;
    }

    // 애니메이션에서 읽을 _speedParam 업데이트
    private void SpeedParamUpdate(Vector3 dir)
    {
        if (dir.magnitude < 0.001f)
        {
            _speedParam = 0; // 이동방향이 없다면 0 고정 - idle 상태 유지를 위함
            return;
        }

        float startParam = _speedParam; // 시작속도
        float endParam = _isRun ? 1.0f : _isCrouch ? 0.33f : 0.66f; // 목표속도 (달리기, 웅크리기 에 해당하는 목표값 지정)
        _speedParam = Mathf.Lerp(startParam, endParam, _paramChangedSpeed * Time.deltaTime); // 보간처리
    }

    #region 외부 호출 함수
    public float GetSpeedParam => _speedParam;

    public void SetCrouch(bool use) => _isCrouch = use;
    public void SetRun(bool use) => _isRun = use;
    #endregion
}
