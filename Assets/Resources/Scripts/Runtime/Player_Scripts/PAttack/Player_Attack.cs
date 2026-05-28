using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 플레이어 공격
/*
 ▶ 할일
  - 플레이어 공격 처리
  - 인풋 공격 이벤트 구독하고 TryAttack 를 수행 _anim 에 Attack Tregger 활성화
  - EndAttack 를 만들어서 애니메이션 마지막 프레임에 호출되도록 작업

   ※ 현재구조로 선택한 이유 ※
    01. 코루틴을 활용한 시간기반 작업으로 구현했으나 타이밍이 오류나거나 트랜지션전이가 안되거나 애니메이션의 Speed 값이 변경되면 오류가생겼음
    02. 간단한 구조이지만 오류가 적고 변경에있어 작업이 유연해보임
*/
#endregion


public class Player_Attack : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _attackWidth = 2.0f; // 공격 범위의 좌우 총 가로폭 (미터)
    [SerializeField] private float _attackDepth = 3.0f; // 공격 범위의 앞방향 총 깊이 (미터)
    #endregion

    #region 내부 변수
    private Player_Anim _anim;
    private Player_Controller _controllCS;
    private Player_DataSO _dataSO;
    #endregion

    private void Awake()
    {
        if (_controllCS == null)
        {
            _controllCS = FindFirstObjectByType<Player_Controller>();
        }
        GUtill.TryGetCS(this, ref _anim);
    }

    private void Start()
    {
        if (_dataSO == null)
        {
            if (Player_DataManager.Instance != null)
            {
                _dataSO = Player_DataManager.Instance.GetDataSO;
            }
        }
    }
    private List<Transform> GetTargetsInFront()
    {
        List<Transform> validTargets = new List<Transform>();

        // 확실하게 캐릭터 최상위 본체(Root)의 위치와 회전값을 기준으로 잡습니다.
        Transform rootTransform = transform.root;

        // 1. 박스 판정의 중심점(Center)을 구합니다. 
        // 플레이어 정중앙이 아니라, 플레이어 위치에서 정면(forward)으로 깊이의 절반만큼 앞으로 밀어주어야 '내 앞의 범위'가 됩니다.
        Vector3 boxCenter = rootTransform.position + (rootTransform.forward * (_attackDepth * 0.5f));
        boxCenter.y += 1.0f; // 높이 차이 오차 방지를 위해 캐릭터 배꼽 높이 정도로 보정

        // 2. OverlapBox에 들어갈 Extents(반지름 크기)를 계산합니다. 
        // 유니티의 박스 크기는 Half-Extents(반값) 기준이므로 인스펙터 입력값의 절반을 넣습니다.
        Vector3 boxHalfSize = new Vector3(_attackWidth * 0.5f, 1.5f, _attackDepth * 0.5f);

        // 3. 내 정면 각도(Rotation)를 그대로 반영하여 정면 직사각형 영역의 컬라이더들을 긁어옵니다.
        Collider[] targetsInBox = Physics.OverlapBox(boxCenter, boxHalfSize, rootTransform.rotation, _targetLayer);

        if (targetsInBox.Length == 0) return validTargets;

        for (int i = 0; i < targetsInBox.Length; i++)
        {
            Transform targetRoot = targetsInBox[i].transform.root;

            // 중복 적재 방지 (콜라이더가 몸통, 머리 여러 개 붙어있는 적 예외 처리)
            if (!validTargets.Contains(targetRoot))
            {
                validTargets.Add(targetRoot);
            }
        }

        return validTargets;
    }
    #region 외부 호출 함수
    public void TryAttack()
    {
        if (_dataSO == null && Player_DataManager.Instance != null)
        {
            _dataSO = Player_DataManager.Instance.GetDataSO;
        }

        if (_dataSO.Stemina < _dataSO.GetSteminaAttackCost)
        {
            GUtill.Log($"[{this.name}] : 스테미너 부족 공격 불가");
            return;
        }

        _controllCS.MovementState = EMovementState.Attack;
        _anim.SetTreggerAnim(_controllCS.MovementState);

        
        GUtill.Log($"[{this.name}] : 공격 시작!");
    }
    public void AnimEvent_HitCheck()
    {
        List<Transform> targets = GetTargetsInFront();

        GUtill.Log($"[{this.name}] : 내 정면 범위 수색 완료! 포착된 적의 수: {targets.Count}명");

        for (int i = 0; i < targets.Count; i++)
        {
            Transform tr = targets[i];
            if (tr.TryGetComponent<Enemy_Health>(out Enemy_Health enemy))
            {
                enemy.TakeDamage(100);
            }
        }
    }

    /// <summary>
    /// 애니메이션 마지막 프레임에 호출할 함수
    /// </summary>
    public void EndAttack()
    {
        _controllCS.MovementState = EMovementState.Idle;

        GUtill.Log($"[{this.name}] : 공격 종료");
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Transform rootTransform = transform.root;
        Vector3 boxCenter = rootTransform.position + (rootTransform.forward * (_attackDepth * 0.5f));
        boxCenter.y += 1.0f;

        Vector3 boxSize = new Vector3(_attackWidth, 3.0f, _attackDepth);

        // 플레이어가 회전한 각도에 맞춰서 박스 기즈모도 같이 회전되도록 매트릭스 고정
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rootTransform.rotation, Vector3.one);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, boxSize); // 회전 매트릭스가 적용되어 중심은 zero로 대입

        Gizmos.matrix = originalMatrix;
    }
}
