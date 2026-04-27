using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MoveScripts : MonoBehaviour
{
	#region 인스펙터
	[Header("캐릭터 컨트롤러")]
	[SerializeField] private CharacterController _controller;
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
    private const float _gravity = -9.81f;
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
    }

    private void Update()
    {
        #region 테스트용 인풋
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift);
        bool crouch = Input.GetKey(KeyCode.C);
        #endregion

        Vector3 dir = new Vector3(x, 0, z);

        SetRun(run);
        SetCrouch(crouch);

        MoveUpdata(dir);        // 이동
        SpeedParamUpdate(dir);  // 스피드 파라미터 업데이트
    }

    private void MoveUpdata(Vector3 dir)
    {
        if (_controller == null) { return; }

        Vector3 vertical = GravityUpdate(dir); // 중력 값 받아오기
        Vector3 moveDir = Vector3.ClampMagnitude(dir, 1); // 수평 이동 방향 (대각선이동 속도 제한)

        float speed = _moveSpeed * (_isRun ? _runMultiplier : _isCrouch ? _crouchMultiplier : 1) * Time.deltaTime;

        Vector3 current = moveDir + vertical;

        _controller.Move(current * speed);
    }

    private Vector3 GravityUpdate(Vector3 dir)
    {
        if (_controller.isGrounded && dir.y < 0.0f)
        {
            _verticalVec.y = -2.0f;
        }

        _verticalVec.y += _gravity * Time.deltaTime;

        return _verticalVec;
    }

    private void SpeedParamUpdate(Vector3 dir)
    {
        if (dir.magnitude < 0.001f)
        {
            _speedParam = 0;
            return;
        }

        float startParam = _speedParam;
        float endParam = _isRun ? 1.0f : _isCrouch ? 0.33f : 0.66f;
        _speedParam = Mathf.Lerp(startParam, endParam, _paramChangedSpeed * Time.deltaTime);
    }

    #region 외부 호출 함수
    public float GetSpeedParam => _speedParam;

    public void SetCrouch(bool use) => _isCrouch = use;
    public void SetRun(bool use) => _isRun = use;
    #endregion
}
