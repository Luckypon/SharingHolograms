using TMPro;
using UnityEngine;

public class InformationText : SingletonBase<InformationText>
{
    public GameObject InfoTextGameObject;
    public TextMeshPro InfoText;


    private void Awake()
    {
        ShowInfoText();
    }

    public static void Show(string line = "")
    {
        if (Instance != null)
        {
            Instance.ShowInfoText(line);
        }
    }

    private void ShowInfoText(string text = "")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            InfoTextGameObject.SetActive(false);
        }
        else
        {
            InfoText.text = text;
            InfoTextGameObject.SetActive(true);
        }
    }
}

