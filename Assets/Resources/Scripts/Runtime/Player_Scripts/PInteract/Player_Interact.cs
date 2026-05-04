using UnityEngine;

#region 플레이어 상호작용
/*
 ▶ 할일
  - 외부에서 지정해준 상호작용 가능 타겟의 Interact 를 호출
*/
#endregion


public class Player_Interact : MonoBehaviour
{
    #region 내부변수
    private IInteract _target;
    #endregion

    #region 외부 호출 함수
    public void TryInteract()
    {
        if (_target == null)
        {
            GUtill.Log($"[{this.name}] : 타겟이 없음", EDebugType.Warn);
            return;
        }

        _target.Interact();
    }

    public void SetTarget(IInteract target) => _target = target;
    #endregion
}
