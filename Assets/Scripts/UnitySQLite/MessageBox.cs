using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnitySQLite.Utilities;
using System.Collections;
using System;

public class MessageBox : GenericSingleton<MessageBox>
{
    public Button rightButton, leftButton;
    public TextMeshProUGUI mainText, label, rightButtonLabel, leftButtonLabel;
    private bool isRunning;
    private Action onShow, onHide;
    public Transform loadingIndicatorParent;

    public override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);

        Account.onAccountNotExists += () =>
        {
            ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = false,
                useLeftButton = false,
                useRightButton = false,
                mainText = "Ο λογιαριασμός δεν υπάρχει."
            }); ;
        };
    }

    public void ShowMessageBox(MessageBoxSettings settings, float delay = 2f)
    {
        onShow += settings.onShow;
        onShow?.Invoke();
        onShow = null;

        rightButton.gameObject.SetActive(settings.useRightButton);
        if (settings.useRightButton)
        {
            rightButton.onClick.AddListener(() => settings.onRightButtonClick?.Invoke());
            rightButtonLabel.text = settings.rightButtonLabel;
        }

        leftButton.gameObject.SetActive(settings.useLeftButton);
        if (settings.useLeftButton)
        {
            leftButton.onClick.AddListener(() => settings.onLeftButtonClick?.Invoke());
            leftButtonLabel.text = settings.leftButtonLabel;
        }

        if (settings.useRightButton | settings.useLeftButton) { leftButton.transform.parent.gameObject.SetActive(true); }

        label.gameObject.SetActive(settings.showLabel);
        if (settings.showLabel) { label.text = settings.label; label.gameObject.SetActive(true); }

        onHide += settings.onHide;

        mainText.text = settings.mainText;
        //ChangeUIItemsStatus(false);
        gameObject.SetActive(true);

        loadingIndicatorParent.gameObject.SetActive(settings.showLoadingIndicator);

        if (settings.showLoadingIndicator)
        {
            StartCoroutine(RotateLoadingIndicator());
        }


        if (delay > 0)
            StartCoroutine(HideWithDelay(delay));
    }

    public void HideMessageBox()
    {
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
        onHide?.Invoke();
        onHide = null;
        gameObject.SetActive(false);
        //ChangeUIItemsStatus(true);
        if (isRunning) { StopCoroutine("HideWithDelay"); }
        StopCoroutine(RotateLoadingIndicator());
        loadingIndicatorParent.transform.localRotation = Quaternion.identity;
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

    IEnumerator RotateLoadingIndicator()
    {
        while (true)
        {
            loadingIndicatorParent.Rotate(-Vector3.forward, Time.deltaTime * 300f);
            yield return null;
        }
    }

    void ChangeUIItemsStatus(bool status)
    {
        Button[] buttonsOnScreen = FindObjectsOfType<Button>();
        TMP_InputField[] inputFieldsOnScreen = FindObjectsOfType<TMP_InputField>();
        TMP_Dropdown[] dropdownsOnScreen = FindObjectsOfType<TMP_Dropdown>();

        for (int i = 0; i < buttonsOnScreen.Length; i++)
        {
            buttonsOnScreen[i].interactable = status;
        }

        for (int j = 0; j < inputFieldsOnScreen.Length; j++)
        {
            inputFieldsOnScreen[j].interactable = status;
        }

        for (int k = 0; k < dropdownsOnScreen.Length; k++)
        {
            dropdownsOnScreen[k].interactable = status;
        }

    }
}
