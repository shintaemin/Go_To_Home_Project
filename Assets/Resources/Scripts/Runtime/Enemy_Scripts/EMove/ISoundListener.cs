#region 사운드 이벤트 리스너
/*
 ▶ 할일
  - 사운드 이벤트를 듣고 지정받을 오브젝트가 상속받을 리스너
*/
#endregion

using UnityEngine;

public interface ISoundListener
{
    void OnSoundListen(Vector3 soundPos);
}
