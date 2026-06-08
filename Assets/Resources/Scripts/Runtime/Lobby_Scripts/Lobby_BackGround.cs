using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region

#endregion


public class Lobby_BackGround : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Material _mat;
    [SerializeField] private Texture2D[] _textures;

    [SerializeField] private float _changeInteval = 5.0f;
    [SerializeField] private string _texturePath = "Resource_Origin/Images/Lobby_BG";
    [SerializeField] private float _minZ = 5.0f;
    [SerializeField] private float _maxZ = 10.0f;
    [SerializeField] private float _moveSpeed = 0.3f;
    #endregion

    #region 내부변수
    private float _nextChangeTime;
    #endregion

    private void Awake()
    {
        if (_mat == null)
        {
            _mat = GetComponent<Renderer>().material;
        }
        if (_textures.Length == 0)
        {
            _textures = Resources.LoadAll<Texture2D>(_texturePath);
        }
    }
    private void Start()
    {
        _nextChangeTime = Time.time + _changeInteval;
    }

    private void Update()
    {
        if (Time.time < _nextChangeTime) 
        {
            MoveUpdate();
            return; 
        }

        RandomChange();
        transform.position = new Vector3(0, 0, 5);
        _nextChangeTime = Time.time + _changeInteval;
    }

    private void OnDestroy()
    {
        if (_mat != null)
        {
            _mat = null;
        }
    }

    private void MoveUpdate()
    {
        float wave = Mathf.Sin(Time.time * _moveSpeed);

        float normalizedWave = (wave + 1.0f) * 0.5f;

        float currentZ = Mathf.Lerp(_maxZ, _minZ, normalizedWave);

        Vector3 currentPos = transform.localPosition;
        currentPos.z = currentZ;
        transform.localPosition = currentPos;
    }

    private void RandomChange()
    {
        if (_mat == null || _textures.Length < 0) { return; }
        int max = _textures.Length;
        int rand = Random.Range(0, max);

        _mat.SetTexture("_BaseMap", _textures[rand]);
    }
}
