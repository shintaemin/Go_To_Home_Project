using System.Collections.Generic;
using UnityEngine;

#region 클립 리스트
/*
 ▶ 할일
  - 사운드클립을 클립별로 드는것이 아닌 타입을 나누어 리스트로 들게하여 랜덤재생하는 방식으로 사용하도록
*/
#endregion

public enum EClipPlayType
{
    None = 0,
    Attack,
    FootStep_Grass,
    FootStep_Road,
    UI_Button,

}

[System.Serializable]
public class ClipList
{
    [System.Serializable]
    public struct ClipPack
    {
        [SerializeField] private EClipPlayType _packType;
        [SerializeField] private List<ClipData> _clipList;

        public readonly EClipPlayType GetPackType => _packType;
        public readonly List<ClipData> GetClipList => _clipList;
    }

    #region 인스펙터
    [SerializeField] private List<ClipPack> _clipPackList = new List<ClipPack>();
    #endregion

    #region 내부변수
    private Dictionary<EClipPlayType, List<ClipData>> _clipDic = new Dictionary<EClipPlayType, List<ClipData>>();
    #endregion

    #region 외부 호출 함수
    public void InitClipList()
    {
        _clipDic.Clear();

        foreach (var packs in _clipPackList)
        {
            EClipPlayType type = packs.GetPackType;
            List<ClipData> list = packs.GetClipList;

            _clipDic.Add(type, list);
        }
    }

    public ClipData GetClipData(EClipPlayType type)
    {
        if (!_clipDic.TryGetValue(type, out List<ClipData> list)) { return null; }
        
        if (list.Count == 1) { return list[0]; }

        int random = Random.Range(0, list.Count);
        
        return list[random];
    }
    #endregion
}
