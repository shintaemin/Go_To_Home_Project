using System;
using UnityEngine;

#region CSV 파서
/*
 ▶ 할일
  - 읽어온 CSV 를 파싱하고 Registry 에 등록시킬 스크립트
*/
#endregion

public class ItemCSVParser : MonoBehaviour
{
    #region 인스펙터
    [Header("옵션")]
    [SerializeField] private string _weaponSoSavePath = "Assets/Resources/SO/ItemSO/WeaponSO"; // 무기SO 데이터들을 저장시킬 위치
    [SerializeField] private string _availableSOSavePath = "Assets/Resources/SO/ItemSO/AvailableSO"; // 사용가능 SO 데이터 들을 저장 시킬 위치

    [Header("파싱 시작 토글")]
    [SerializeField] private bool _parsingUpdate = false;
    #endregion

    #region 내부변수
    private ItemCSVReader _reader; // 읽어올 TextAsset 이 담겨있는 클래스
    #endregion

    private void OnValidate()
    {
        if (!_parsingUpdate) { return; }

        if (_reader == null)
        {
            _reader = FindFirstObjectByType<ItemCSVReader>();
        }

        CSVParsing();
        _parsingUpdate = false;
    }

    private void ParsingData()
    {
        TextAsset[] csvs = _reader.GetCsvArray;
        
        foreach(var csv in csvs)
        {
            string[] rows = csv.text.Split('\n'); // \n 을 기준으로 배열에 담기 - 한줄씩 배열에 등록
            for (int i = 3; i < rows.Length; i++)
            {
                if (string.IsNullOrEmpty(rows[i])) { continue; } // 해당 줄 의 문자가 비어있으면 다음반복 수행

                string[] cols = rows[i].Split(',');  // , 를 기준으로 배열에 담기 - 한칸 씩 배열에 담기

                // item 의 타입을 확인해서 분기를 위해 - 열거형도 Enum.Parse 를 통해 String 을 enum 으로 형변환이 가능
                EItemType itemType = (EItemType)Enum.Parse(typeof(EItemType), cols[3].Trim()); 

                // 분기
                switch(itemType)
                {
                    case EItemType.Weapon:  ParseWeapon(cols);
                        break;
                    case EItemType.Available:   ParseAvailable(cols); 
                        break;
                }
            }
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.SaveAssets();      // 모든 SO 파일 하드디스크에 저장
        UnityEditor.AssetDatabase.Refresh();         // 프로젝트 창 갱신
        GUtill.Log("전체 아이템 파싱 및 영구 저장 완료!", EDebugType.Warn);
#endif
    }

    // 무기 아이템 파싱
    private void ParseWeapon(string[] cols)
    {
        string itemFileName = cols[0];
        int id = int.Parse(cols[1]);
        string name = cols[2];
        EItemType type = (EItemType)Enum.Parse(typeof(EItemType), cols[3].Trim());
        int isInteract = int.Parse(cols[4]);
        int isStackable = int.Parse(cols[5]);
        int maxStackCount = int.Parse(cols[6]);
        Sprite icon = Resources.Load<Sprite>(cols[7]);
        string info = cols[8];
        EWeaponType weaponType = (EWeaponType)Enum.Parse(typeof(EWeaponType), cols[9].Trim());
        int damage = int.Parse(cols[10]);
        float speed = float.Parse(cols[11]);
        float dur = float.Parse(cols[12]);
        float cost = float.Parse(cols[13]);
        EAttackSoundType soundType = (EAttackSoundType)Enum.Parse(typeof(EAttackSoundType), cols[14].Trim());
        GameObject prefab = Resources.Load<GameObject>(cols[15].Trim());
        string filePath = _weaponSoSavePath;

        // 실제 SO 생성 후 SetUp을 호출
        WeaponDataSO weaponSO = CreateSO<WeaponDataSO>(id, itemFileName, filePath);
        weaponSO.SetUp(id, name, type, isInteract, isStackable, maxStackCount, icon, info, weaponType, damage, speed, dur, cost, soundType, prefab);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(weaponSO); // 저장을 위함 에디터에 SO 정보가 바뀌었음을 저장한다.
#endif
    }

    // 사용 가능 아이템 분기
    private void ParseAvailable(string[] cols)
    {
        EAvailableType type = (EAvailableType)Enum.Parse(typeof(EAvailableType), cols[9].Trim());

        // 사용가능아이템은 물건던지기 등의 다양한 아이템이 들어 올 수 있으므로 분기가 가능하도록
        switch (type)
        {
            case EAvailableType.Healing: ParseHealingItem(cols); break;
                // 타입이 추가되면 case 추가
        }

    }

    // 회복 아이템 파싱
    private void ParseHealingItem(string[] cols)
    {
        string itemFileName = cols[0];
        int id = int.Parse(cols[1]);
        string name = cols[2];
        EItemType type = (EItemType)Enum.Parse(typeof(EItemType), cols[3].Trim());
        int isInteract = int.Parse(cols[4]);
        int isStackable = int.Parse(cols[5]);
        int maxStackCount = int.Parse(cols[6]);
        Sprite icon = Resources.Load<Sprite>(cols[7]);
        string info = cols[8];
        EAvailableType availableType = (EAvailableType)Enum.Parse(typeof(EAvailableType), cols[9].Trim());
        EHealingType healType = (EHealingType)Enum.Parse(typeof (EHealingType), cols[10].Trim());
        int value = int.Parse(cols[11]);
        float dur = float.Parse(cols[12]);
        float cooldown = float.Parse(cols[13]);
        string filePath = _availableSOSavePath;
        GameObject prefab = Resources.Load<GameObject>(cols[14]);

        HealItemSO healItemSO = CreateSO<HealItemSO>(id, itemFileName, filePath);
        healItemSO.SetUp(id, name, type, isInteract, isStackable, maxStackCount, icon, info, availableType, healType, value, dur, cooldown, prefab);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(healItemSO);
#endif
    }

    // SO를 생성하고 반환시킬 함수
    private T CreateSO<T>(int id, string itemFileName, string path) where T : ItemDataSO
    {
#if UNITY_EDITOR
        string fileName = $"{itemFileName}_{id}.asset"; // 저장할 이름
        string fullPath = $"{path}/{fileName}"; // 저장 위치 + 이름

        T so = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath); // 해당경로의 이름으로 있는지를 확인

        if (so == null)
        {
            so = ScriptableObject.CreateInstance<T>(); // 없다면 ItemDataSo 를 지정한 T(무기, 사용가능아이템)등 으로 생성
            UnityEditor.AssetDatabase.CreateAsset(so, fullPath); // 에셋파일로 생성
        }

        return so; // 찾은 or 만든 SO 를 반환
#else
        return ScriptableObject.CreateInstance<T>();
#endif
    }

    #region 외부 호출 함수
    public void CSVParsing() // 실제 파싱을 수행할 진입점
    {
        if (_reader == null) { return; }

        ParsingData();
    }
    #endregion
}
