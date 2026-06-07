using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Enemy_Sound : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private AudioSource _audio;
    [SerializeField] private ClipList _clipList;

    [Header("옵션")]
    [SerializeField] private float _idleSoundInterval = 6.0f;
    #endregion

    #region 내부변수
    private Enemy_Controller _controllerCS;
    private float _nextIdleSoundPlayTime;
    #endregion

    private void Awake()
    {
        if (_audio == null) { GUtill.TryGetCS(this, ref _audio); }
        if (_controllerCS == null) { GUtill.TryGetCS(this, ref _controllerCS); }
        _clipList.InitClipList();
    }

    private void Start()
    {
        _nextIdleSoundPlayTime = Time.time + _idleSoundInterval;
    }

    private void Update()
    {
        if (_controllerCS.EnemyMoveState != EEnemyMoveState.Patroll) { return; }
        if (Time.time < _nextIdleSoundPlayTime) { return; }

        SoundPlay(EClipPlayType.Z_Idle);
        _nextIdleSoundPlayTime = Time.time + _idleSoundInterval;
    }

    private void SoundPlay(EClipPlayType type)
    {
        if (_audio == null || SoundManager.Instance == null) { return; }
        ClipData clip = _clipList.GetClipData(type);
        if (clip != null)
        {
            SoundManager.Instance.SfxPlay(_audio, clip);
        }
    }

    #region 외부 호출 함수
    public void OnHitSoundPlay()
    {
        SoundPlay(EClipPlayType.Z_Hit);
    }
    
    public void OnAttackSoundPlay()
    {
        SoundPlay(EClipPlayType.Z_Attack);
    }

    public void OnDeathSoundPlay()
    {
        SoundPlay(EClipPlayType.Z_Death);
    }
    #endregion
}
