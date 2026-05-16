using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_FootSound : MonoBehaviour
{
	public enum EGroundType
	{
		None,
		Road,
		Grass,
	}

	#region 인스펙터
	[SerializeField] private EGroundType _groundType;
    [SerializeField] private float _rayLength = 0.2f;
	[SerializeField] private string _roadTag = "Road";
	[SerializeField] private string _grassTag = "Grass";
	[SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Update()
    {
		bool ray = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _rayLength, _groundLayer);

        if (ray)
		{
			if (_groundType != EGroundType.Road && hit.collider.CompareTag(_roadTag))
			{
				_groundType = EGroundType.Road;
			}
			else if (_groundType != EGroundType.Grass && hit.collider.CompareTag(_grassTag))
			{
				_groundType = EGroundType.Grass;
			}
		}
    }
    #region 외부 호출 함수
	public void PlayFootStep(AudioSource source, ClipList list)
	{
		if (SoundManager.Instance == null) { return; }

		ClipData clip = null;

		switch(_groundType)
		{
			case EGroundType.Road:
				clip = list.GetClipData(EClipPlayType.FootStep_Road); break;
			case EGroundType.Grass:
				clip = list.GetClipData(EClipPlayType.FootStep_Grass); break;
        }

		if (clip != null)
        {
            SoundManager.Instance.SfxPlay(source, clip);
        }
	}

	public EGroundType GetGroundType => _groundType;
    #endregion
}
