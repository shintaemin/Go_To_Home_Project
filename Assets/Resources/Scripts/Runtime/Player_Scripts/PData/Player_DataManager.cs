using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 플레이어 데이터 매니저
/*
 ▶ 할일
  - 데이터 매니저
  - 생성 이유 : Data가 필요한 모든곳에서 참조하기보단 싱글톤을 통해 꺼내쓰는 방식이 편하고 참조를 한곳에서 진행해 유지보수하기 위함
*/
#endregion

public class Player_DataManager : MonoBehaviour
{
	public static Player_DataManager Instance { get; private set; }

	#region 인스펙터
	[SerializeField] private Player_DataSO _dataSO;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            enabled = false;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    #region 외부 호출 함수
    public Player_DataSO GetDataSO => _dataSO;
	#endregion
}
