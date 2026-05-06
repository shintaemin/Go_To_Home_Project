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
    [SerializeField] private string _weaponSoSavePath; // 무기SO 데이터들을 저장시킬 위치
    [SerializeField] private string _availableSOSavePath; // 사용가능 SO 데이터 들을 저장 시킬 위치

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
            string[] rows = csv.text.Split('\n');
            for (int i = 3; i < rows.Length; i++)
            {
                if (string.IsNullOrEmpty(rows[i])) { continue; }

                string[] cols = rows[i].Split(',');
                EItemType itemType = (EItemType)Enum.Parse(typeof(EItemType), cols[3].Trim());

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

        WeaponDataSO weaponSO = CreateSO<WeaponDataSO>(id, itemFileName, filePath);
        weaponSO.SetUp(id, name, type, isInteract, isStackable, maxStackCount, icon, info, weaponType, damage, speed, dur, cost, soundType, prefab);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(weaponSO);
#endif
    }

    // 사용 가능 아이템 분기
    private void ParseAvailable(string[] cols)
    {
        EAvailableType type = (EAvailableType)Enum.Parse(typeof(EAvailableType), cols[9].Trim());
        switch (type)
        {
            case EAvailableType.Healing: ParseHealingItem(cols); break;
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

    private T CreateSO<T>(int id, string itemFileName, string path) where T : ItemDataSO
    {
#if UNITY_EDITOR
        // 1. 파일 이름과 전체 경로 설정
        string fileName = $"{itemFileName}_{id}.asset";
        string fullPath = $"{path}/{fileName}";

        // 2. 이미 해당 경로에 에셋이 있는지 확인
        T so = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);

        if (so == null)
        {
            // 3. 파일이 없다면 메모리에 인스턴스 생성 후 에셋 파일로 물리적 저장
            so = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(so, fullPath);
        }

        return so;
#else
        return ScriptableObject.CreateInstance<T>();
#endif
    }

    #region 외부 호출 함수
    public void CSVParsing()
    {
        if (_reader == null) { return; }

        ParsingData();
    }
    #endregion
}
