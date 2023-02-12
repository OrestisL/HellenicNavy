using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnitySQLite;

public class InterfaceManager : GenericSingleton<InterfaceManager>
{
    [Header("Login panel")]
    public GameObject loginPanel;

    [Header("User panel")]
    public GameObject userPanel;
    public Button logoutButton;
    public TextMeshProUGUI username;

    public override void Awake()
    {
        base.Awake();

        AccountManagement.onSuccessfulLogin += (acc) =>
        {
            username.text = acc.AccountName;
            loginPanel.SetActive(false);
            userPanel.SetActive(true);

            //clear input fields
            AccountManagement.Instance.password.text = "";
            AccountManagement.Instance.username.text = "";

            //show info depending on account 
        };

        AccountManagement.onLogout += () =>
        {
            //hide all info first
            AccountManagement.Instance.ClearCurrentAccount();
            userPanel.SetActive(false);
            loginPanel.SetActive(true);
        };

        logoutButton.onClick.AddListener(() => AccountManagement.onLogout?.Invoke());
    }
}
