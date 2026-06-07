using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Lobby_Menu : MonoBehaviour
{
    #region

    #endregion

    #region

    #endregion

    #region 외부 호출 함수
    public void ButtonEvent_GameEnd()
    {
        if (SceneLoadManager.Instance != null)
        {
            SceneLoadManager.Instance.GameEnd();
        }
    }

    public void ButtonEvent_MainGame()
    {
        if (SceneLoadManager.Instance != null)
        {
            SceneLoadManager.Instance.MainGame();
        }
    }
    #endregion
}

