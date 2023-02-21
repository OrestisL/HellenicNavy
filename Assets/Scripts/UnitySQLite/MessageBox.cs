using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnitySQLite.Utilities;

public class MessageBox : GenericSingleton<MessageBox>
{

    public Button rightButton, leftButton;
    public TextMeshProUGUI mainText, label;

    public override void Awake()
    {
        base.Awake();
    }

    public void ShowMessageBox(MessageBoxSettings settings)
    {
        rightButton.gameObject.SetActive(settings.useRightButton);
        if (settings.useRightButton) { rightButton.onClick.AddListener(() => settings.onRightButtonClick?.Invoke()); }

        leftButton.gameObject.SetActive(settings.useLeftButton);
        if (settings.useLeftButton) { leftButton.onClick.AddListener(() => settings.onLeftButtonClick?.Invoke()); }

        label.gameObject.SetActive(settings.showLabel);
        if (settings.showLabel) { label.text = settings.label; label.gameObject.SetActive(true); }

        mainText.text = settings.mainText;
    }

    public void HideMessageBox()
    {
        gameObject.SetActive(false);
    }
}
