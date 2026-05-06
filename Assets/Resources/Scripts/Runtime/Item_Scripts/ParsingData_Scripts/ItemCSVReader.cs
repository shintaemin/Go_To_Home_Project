using UnityEngine;

#region CSV 리더
/*
 ▶ 할일 
  - 만들어진 구글시트를 읽어오는 스크립트
*/
#endregion


public class ItemCSVReader : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private TextAsset[] _itemCsvArray;

    [Header("시트 위치")]
    [SerializeField] private string _sheetPilePath;

    [Header("실행 여부")]
    [SerializeField] private bool _updateCsvArray = false;
    #endregion

    #region

    #endregion

    private void OnValidate()
    {
        if (!_updateCsvArray) { return; }

        if (!string.IsNullOrEmpty(_sheetPilePath))
        {
            TextAsset[] csvs = FindCSV();

            if (csvs.Length > 0)
            {
                _itemCsvArray = csvs;
            }
            _updateCsvArray = false;
            GUtill.Log($"[{this.name}] : CSV 배열 갱신 완료 GetCsvArray 로 사용 가능", EDebugType.Warn);
        }
    }

    #region 외부 호출 함수
    public TextAsset[] GetCsvArray => _itemCsvArray;

    public TextAsset[] FindCSV()
    {
        return Resources.LoadAll<TextAsset>(_sheetPilePath);
    }
    #endregion
}
