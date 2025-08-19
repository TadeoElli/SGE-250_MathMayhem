using TMPro;
using UnityEngine;

public class TextLocalizer : MonoBehaviour
{
    public string localizationKey;
    private TextMeshProUGUI textComponent;
    private bool useTTS = false;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        UpdateText();
        LocalizationNotifier.OnLanguageChanged += UpdateText;
    }

    void OnDestroy()
    {
        LocalizationNotifier.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (textComponent == null) return;

        if (useTTS)
        {
            textComponent.text = DialogueLoader.GetTTS(localizationKey);
        }
        else
        {
            textComponent.text = DialogueLoader.GetText(localizationKey);
        }
    }

    public void SetTTSMode(bool isTTS)
    {
        useTTS = isTTS;
        UpdateText();
    }
}