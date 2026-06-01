using TMPro;
using UnityEngine;

#region

#endregion


public class FontFinder : MonoBehaviour
{
    #region ¿ŒΩ∫∆Â≈Õ
    [SerializeField] private string _fontFilePath = "Fonts/CookieRun Black SDF.asset";
    [SerializeField] private GameObject _canvas;
    [SerializeField] private bool _OnChingFont;
    #endregion

    private void OnValidate()
    {
        if (_OnChingFont)
        {
            SirchTextObj();
            _OnChingFont = false;
        }
    }

    private void SirchTextObj()
    {
        if (_canvas == null)
        {
            _canvas = FindFirstObjectByType<Canvas>().gameObject;
        }

        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(_fontFilePath);
        TextMeshProUGUI[] tmps = _canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        if (font != null && tmps.Length > 0)
        { 
            for (int i = 0; i < tmps.Length; i++)
            {
                tmps[i].font = font;
            }
        }
    }
}
