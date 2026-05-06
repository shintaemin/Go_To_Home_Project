using UnityEngine;

#region CSV 리더
/*
 ▶ 할일 
  - 만들어진 구글시트를 읽어오는 스크립트
  - Parser 가 사용할 수 있도록 인스펙터에 지정된 경로에서 CSV 를 찾고 배열에 담는 스크립트
*/
#endregion


public class ItemCSVReader : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private TextAsset[] _itemCsvArray; // CSV 파일을 저장할 배열

    [Header("시트 위치")]
    [SerializeField] private string _sheetPilePath = "SO/ItemSO/CSV"; // 인스펙터상 지정할 CSV 파일 위치

    [Header("실행 여부")]
    [SerializeField] private bool _updateCsvArray = false; // Validate 타이밍을 완벽하게 제어하기 위함
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
