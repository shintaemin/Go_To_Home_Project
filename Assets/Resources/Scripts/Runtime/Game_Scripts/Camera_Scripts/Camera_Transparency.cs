using UnityEngine;

#region 투명화
/*
 ▶ 할일
  - 플레이어가 가려지면 투명화처리
*/
#endregion


public class Camera_Transparency : MonoBehaviour
{
    #region 인스펙터
    [Header("플레이어 트랜스폼")]
    [SerializeField] private Transform _playerTr;

    [Header("투명처리할 레이어")]
    [SerializeField] private LayerMask _transparencyLayer;
    #endregion

    #region 내부변수
    private Renderer _targetRender;
    #endregion

    private void LateUpdate()
    {
        if (_playerTr == null) { return; }

        Vector3 camPos = transform.position;
        Vector3 playerPos = _playerTr.position + Vector3.up;

        Vector3 dir = (playerPos - camPos).normalized;
        float dis = Vector3.Distance(camPos, playerPos);

        if (Physics.Raycast(camPos, dir, out RaycastHit hit, dis, _transparencyLayer))
        {
            Renderer render = hit.collider.GetComponent<Renderer>();

            if (render != null)
            {
                if (render != _targetRender)
                {
                    ResetRender();

                    _targetRender = render;
                    _targetRender.enabled = false;
                }
            }
        }
        else
        {
            ResetRender();
        }
    }

    private void ResetRender()
    {
        if (_targetRender == null) { return; }

        _targetRender.enabled = true;
        _targetRender = null;
    }
}
