using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnitySQLite;

public class ShowHidePasswordInput : MonoBehaviour, IPointerExitHandler, IPointerDownHandler
{
    public Sprite show, hide;
    private Image img;
    private bool isVisible;

    private void Start()
    {
        img = GetComponent<Image>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.clickCount > 0)
        {
            ShowPassword();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //always hide on exit
        AccountManagement.Instance.passwordField.contentType = TMP_InputField.ContentType.Password;
        img.sprite = show;
        AccountManagement.Instance.passwordField.ForceLabelUpdate();
    }

    void ShowPassword()
    {
        TMP_InputField.ContentType current = AccountManagement.Instance.passwordField.contentType;
        if (current == TMP_InputField.ContentType.Password)
        {
            AccountManagement.Instance.passwordField.contentType = TMP_InputField.ContentType.Standard;
            img.sprite = hide;
        }
        else if (current == TMP_InputField.ContentType.Standard)
        {
            AccountManagement.Instance.passwordField.contentType = TMP_InputField.ContentType.Password;
            img.sprite = show;
        }

        AccountManagement.Instance.passwordField.ForceLabelUpdate();
    }
}
