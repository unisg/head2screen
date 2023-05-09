using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BindingInteger : MonoBehaviour
{
    [SerializeField]
    EAppSettingsNames settingName;
    [SerializeField]
    TMP_InputField TextField;

    private void OnEnable()
    {

        TextField.text = PlayerPrefs.GetInt(settingName.ToString()).ToString();
    }

   

    public void SaveValue()
    {
        if (!string.IsNullOrWhiteSpace(TextField.text))
        {
            string s = GetNumbers(TextField.text);
            PlayerPrefs.SetInt(settingName.ToString(),int.Parse(s));
        }

    }

    private static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
}
