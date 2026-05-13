using System;
using System.Collections.Generic;
using UnityEngine;

#region 사운드 이펙터 풀 매니저
/*
 ▶ 할일
  - 사운드 이펙트를 미리 생성하고 OnOff 시킬 매니저
  - 외부에서 편하게 사용 및 풀로 복귀 시킬수 있도록 작업
*/
#endregion

public class SoundEffect_PoolManager : MonoBehaviour
{
    public static SoundEffect_PoolManager Instance { get; private set; }

	#region 인스펙터
	[SerializeField] private List<SoundEffect_Decal> _aliveList = new List<SoundEffect_Decal>();
	[SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _poolRoot;
    [SerializeField] private int _poolCount = 20;
	#endregion

	#region 내부 변수
	private Queue<SoundEffect_Decal> _pool = new Queue<SoundEffect_Decal>();
    #endregion

    #region 이벤트
    public event Action<Vector3, float> OnSoundEmited;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        InitPool();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void InitPool()
    {
        _aliveList.Clear();
        _pool.Clear();

        for (int i = 0; i < _poolCount; i++)
        {
            GameObject obj = Instantiate(_prefab);
            Transform root = _poolRoot;
            SoundEffect_Decal decal = null;
            GUtill.TryGetCS(obj, ref decal);

            if (decal != null)
            {
                _pool.Enqueue(decal);
                obj.transform.SetParent(root);
                obj.SetActive(false);
            }
        }
    }

    public SoundEffect_Decal SpawnEffect(Vector3 pos, float range)
    {
        SoundEffect_Decal decal = null;
        if (_pool.Count > 0)
        {
            decal = _pool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(_prefab);
            GUtill.TryGetCS(obj, ref decal);
        }

        if (decal != null)
        {
            decal.gameObject.SetActive(true);
            decal.transform.SetParent(null);
            decal.Pos = pos;
            decal.Range = range;
            decal.InitPlay();
            OnSoundEmited?.Invoke(pos, range);
            _aliveList.Add(decal);
        }
        
        return decal;
    }

    public void ReturnToPool(SoundEffect_Decal decal)
    {
        decal.transform.SetParent(_poolRoot);
        decal.gameObject.SetActive(false);
        _pool.Enqueue(decal);
        _aliveList.Remove(decal);
    }
}
