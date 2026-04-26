using System.Collections;
using System.Collections.Generic;
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

    #endregion

    private void Awake()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
        }
    }

    private void Update()
    {
        LookMousePoint();
    }

    private void LookMousePoint()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

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
