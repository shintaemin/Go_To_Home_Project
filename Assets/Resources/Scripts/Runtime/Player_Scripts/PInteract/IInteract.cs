
#region 상호작용 인터페이스
/*
 ▶ 할일
  - 플레이어와 상호작용할 컴포넌트에 상속받을 인터페이스
*/
#endregion

using UnityEngine;

public interface IInteract
{
    void Interact();
    // 무언가 추가 되거나 매개변수가 바뀔 수 있음
    string NameText();
    string ViewText();
}
