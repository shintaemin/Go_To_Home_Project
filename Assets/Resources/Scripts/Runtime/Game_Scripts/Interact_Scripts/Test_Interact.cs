using UnityEngine;

#region 상호작용 테스트
/*
 ▶ 할일
  - 상속 받고 함수만들고 연결 테스트용 스크립트
*/
#endregion

public class Test_Interact : MonoBehaviour, IInteract
{
    public void Interact()
    {
        GUtill.Log($"[{this.name}] : 상호작용 성공");
    }
}
