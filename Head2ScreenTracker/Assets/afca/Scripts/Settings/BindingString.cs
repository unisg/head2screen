using System.Linq;
using TMPro;
using UnityEngine;

public class BindingString : MonoBehaviour
{

    [SerializeField]
    EAppSettingsNames settingName;
    [SerializeField]
    TMP_InputField TextField;

    private void OnEnable()
    {

        TextField.text = PlayerPrefs.GetString(settingName.ToString());
    }



    public void SaveValue()
    {
            PlayerPrefs.SetString(settingName.ToString(), TextField.text);

    }

}
