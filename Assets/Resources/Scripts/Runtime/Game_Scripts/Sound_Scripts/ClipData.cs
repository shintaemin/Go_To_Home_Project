using UnityEngine;

#region 클립 데이터
/*
 ▶ 할일
  - 각 클립이 갖고있을 데이터
*/
#endregion

[System.Serializable]
public class ClipData
{
    [SerializeField] private AudioClip _clip;
    [SerializeField] private bool _randomPitch = false;
    [SerializeField] private bool _sound3D = true;

    #region 생성자
    public ClipData(AudioClip clip, bool randomPitch = false)
    {
        _clip = clip;
        _randomPitch = randomPitch;
    }
    #endregion
    #region 외부 호출 함수
    public AudioClip GetClip => _clip;
    public float GetPitch => _randomPitch ? Random.Range(0.9f, 1.1f) : 1;
    public float GetSpatialBlend => _sound3D ? 1f : 0;
    #endregion
}
