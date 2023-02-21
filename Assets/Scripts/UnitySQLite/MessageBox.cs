using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnitySQLite.Utilities;
using System.Collections;

public class MessageBox : GenericSingleton<MessageBox>
{
    public Button rightButton, leftButton;
    public TextMeshProUGUI mainText, label;
    private bool isRunning;

    public override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);

        Account.onAccountNotExists += () => {
            ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = false,
                useLeftButton= false,
                useRightButton= false,
                mainText = "Ο λογιαριασμός δεν υπάρχει."              
            }); ;
        };
    }

    public void ShowMessageBox(MessageBoxSettings settings)
    {
        rightButton.gameObject.SetActive(settings.useRightButton);
        if (settings.useRightButton) { rightButton.onClick.AddListener(() => settings.onRightButtonClick?.Invoke()); }

        leftButton.gameObject.SetActive(settings.useLeftButton);
        if (settings.useLeftButton) { leftButton.onClick.AddListener(() => settings.onLeftButtonClick?.Invoke()); }

        if (!settings.useRightButton & !settings.useLeftButton) { leftButton.transform.parent.gameObject.SetActive(false); }

        label.gameObject.SetActive(settings.showLabel);
        if (settings.showLabel) { label.text = settings.label; label.gameObject.SetActive(true); }

        mainText.text = settings.mainText;
        gameObject.SetActive(true);

        StartCoroutine(HideWithDelay(2f));
    }

    public void HideMessageBox()
    {
        gameObject.SetActive(false);
        if (isRunning) { StopCoroutine("HideWithDelay"); }
    }

    private IEnumerator HideWithDelay(float delay) 
    {
        if (isRunning) 
        {
            delay += delay;
        }
        isRunning = true;
        yield return new WaitForSeconds(delay);
        HideMessageBox();
        isRunning = false;
    }
}
