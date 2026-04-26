using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MoveScripts : MonoBehaviour
{
	#region 인스펙터
	[Header("캐릭터 컨트롤러")]
	[SerializeField] private CharacterController _controller;
    [SerializeField] private float _moveSpeed;

    [Header("옵션")]
    [SerializeField] private float _defaultSpeed = 5.0f; // 기본 속도값 Data를 불러오지 못했을경우를 대비 의존성 ↓↓
    #endregion

    #region 내부변수

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
        #endregion

        if (x != 0 || z != 0)
        {
            MoveUpdata(x, z);
        }
    }

    private void MoveUpdata(float x, float z)
    {
        if (_controller == null) { return; }

        float t = 1.0f - Mathf.Exp(-_moveSpeed * Time.deltaTime);
        Vector3 dir = new Vector3(x, 0, z) * t;

        _controller.Move(dir);
    }

    #region 외부 호출 함수
    public Vector3 GetVelocity => _controller.velocity;
    #endregion
}
