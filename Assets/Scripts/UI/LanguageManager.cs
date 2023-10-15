using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;

public class LanguageManager : MonoBehaviour
{

    private bool active = false;
    
    private void Start() 
    {
        int id_LangKey = PlayerPrefs.GetInt("LangKey", 0);
    }
    public void ChangeLang(int index)
    {
        if (active)
            return;
        StartCoroutine(SetLang(index));
    }

    IEnumerator SetLang(int _localID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localID];
        PlayerPrefs.SetInt("LangKey", _localID);
        active = false;
    }
}
