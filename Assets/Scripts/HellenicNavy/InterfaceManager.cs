using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnitySQLite;
using System.Collections.Generic;
using SPS;
using UnitySQLite.Utilities;

public class InterfaceManager : GenericSingleton<InterfaceManager>
{
    [SerializeField]
    Service _currentService;
    [SerializeField]
    CreateTables tables;

    public GameObject canvas;

    [Header("Login panel")]
    public GameObject loginPanel;
    public Button loginButton;
    public Button updatePasswordButton;

    [Header("User panel")]
    public GameObject userPanel;
    public Button logoutButton;
    public TextMeshProUGUI username;

    [Header("Service Interface")]
    public GameObject addMachineryPanel;
    public Button addMachineryButton;
    public Button createMachineryButton;
    public Button addServiceEntryButton;
    public RectTransform serviceEntryParent;
    public TMP_InputField nameInput;
    public TMP_InputField idInput;
    public TMP_Dropdown deptDropdown;
    public TMP_Dropdown systemDropdown;
    public TMP_InputField hoursInput;

    [Header("Menu")]
    public GameObject menuPanel;
    public Button menuButton;
    public Button addSystemButton;
    public Button changePWButton;

    [Header("Add System Interface")]
    public GameObject addSystemPanel;
    public Button createSystemEntryButton;
    public TMP_InputField addSystemName;
    public Button closeAddSystemPanel;

    [Header("Prefabs")]
    public GameObject serviceEntryPrefab;
    public GameObject serviceTypePrefab;

    [SerializeField]
    List<ServiceEntry> serviceEntries;

    public enum LoginInterfaceSetup
    {
        login,
        changePW,
    }

    public override void Awake()
    {
        base.Awake();

        AccountManagement.onAfterLogin += (valid, acc) =>
        {
            if (valid)
            {
                username.text = acc.AccountName;
                loginPanel.SetActive(false);
                userPanel.SetActive(true);

                //clear input fields
                AccountManagement.Instance.passwordField.text = "";
                AccountManagement.Instance.usernameField.text = "";

                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    useRightButton = false,
                    useLeftButton= false,
                    showLabel = false,
                    mainText = string.Format("Επιτυχής σύνδεση {0}.", acc.AccountName),
                });

                //show info depending on account
                SetupInterface(AccountManagement.Instance.CurrentAccount.AccessLevel);
            }
        };

        AccountManagement.onLogout += () =>
        {
            //clear input fields
            AccountManagement.Instance.passwordField.text = "";
            AccountManagement.Instance.usernameField.text = "";

            //hide all info first
            AccountManagement.Instance.ClearCurrentAccount();
            userPanel.SetActive(false);
            SetupLoginInterface(LoginInterfaceSetup.login);
        };

        logoutButton.onClick.AddListener(() => AccountManagement.onLogout?.Invoke());
    }

    private void Start()
    {
        SetupButtons();
    }

    void SetupInterface(AccessLevel accessLevel)
    {
        //TODO hide UI elements according to access level
    }

    void SetupButtons()
    {
        loginButton.onClick.AddListener(() =>
        {
            MessageBoxSettings settings = new MessageBoxSettings()
            {
                showLabel = false,
                useRightButton = false,
                useLeftButton = false,
                mainText = "Παρακαλώ περιμένετε...",

            };
            MessageBox.Instance.ShowMessageBox(settings);
            AccountManagement.Instance.Login();
        });

        addMachineryButton.onClick.AddListener(() =>
        {
            ShowSystems(systemDropdown);
            addMachineryPanel.SetActive(!addMachineryPanel.activeSelf);
        });
        addServiceEntryButton.onClick.AddListener(AddServiceEntry);
        createMachineryButton.onClick.AddListener(AddMachinery);

        createSystemEntryButton.onClick.AddListener(() => AddSystem(addSystemName.text));
        addSystemButton.onClick.AddListener(() => addSystemPanel.SetActive(!addSystemPanel.activeSelf));
        closeAddSystemPanel.onClick.AddListener(() => addSystemPanel.SetActive(false));

        menuButton.onClick.AddListener(() => menuPanel.SetActive(!menuPanel.activeSelf));

        changePWButton.onClick.AddListener(() =>
        {
            SetupLoginInterface(LoginInterfaceSetup.changePW);
            loginPanel.SetActive(!loginPanel.activeSelf);
        });
        updatePasswordButton.onClick.AddListener(() =>
        {
            AccountManagement.Instance.CurrentAccount.ChangePassword(AccountManagement.Instance.passwordField.text);
            loginPanel.SetActive(false);
            //show message
        });
    }

    void SetupLoginInterface(LoginInterfaceSetup setup)
    {
        AccountManagement.Instance.ChangeInterfaceLayout(setup);
        switch (setup)
        {
            case LoginInterfaceSetup.login:
                updatePasswordButton.transform.parent.gameObject.SetActive(false);
                loginButton.transform.parent.gameObject.SetActive(true);
                ResetUI();
                loginPanel.SetActive(true);
                break;
            case LoginInterfaceSetup.changePW:
                updatePasswordButton.transform.parent.gameObject.SetActive(true);
                loginButton.transform.parent.gameObject.SetActive(false);
                break;
        }

    }

    void ResetUI()
    {
        menuPanel.SetActive(false);
        userPanel.SetActive(false);
        addSystemPanel.SetActive(false);
        addMachineryPanel.SetActive(false);
    }

    void AddServiceEntry()
    {
        Instantiate(serviceEntryPrefab, serviceEntryParent);
    }

    void AddMachinery()
    {
        serviceEntries = new List<ServiceEntry>();

        for (int i = 0; i < serviceEntryParent.childCount; i++)
        {
            serviceEntries.Add(serviceEntryParent.GetChild(i).GetComponent<ServiceEntry>());
        }

        _currentService = new Service(nameInput.text, idInput.text, hoursInput.text.Length > 0 ? int.Parse(hoursInput.text) : 0, serviceEntries);
        Debug.Log(_currentService.ToJson());
    }

    /// <summary>
    /// When adding a system, it should be written to the systems list table.
    /// </summary>
    /// <param name="name">System name.</param>
    void AddSystem(string name)
    {
        if (name.Equals(string.Empty))
        {
            return;
        }

        DatabaseManager.Instance.WriteOnce(() =>
        {
            List<DataEntry> entries = new List<DataEntry>() { new DataEntry(name) };
            List<TableColumn> columns = tables.systemsList.columns;
            TableRow row = new TableRow(columns, entries);
            DatabaseManager.Instance.ThreadedWriteToDatabase("SystemsList", row, true,
                () => MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    useRightButton = false,
                    useLeftButton = false,
                    showLabel = false,
                    mainText = string.Format("To σύστημα \"{0}\" υπάρχει ήδη στη βάση δεδομένων.", name),
                }
                ),
                () => MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    useRightButton = false,
                    useLeftButton = false,
                    showLabel = false,
                    mainText = string.Format("To σύστημα \"{0}\" προστέθηκε στη βάση δεδομένων επιτυχώς.", name),
                }));
            //Debug.Log(string.Format("successfully added system {0} to the database", name));
        });
    }

    /// <summary>
    /// When adding/editing machinery info, a dropdown that contains all systems should appear.
    /// </summary>
    /// <param name="dropdown">Dropdown containing all systems.</param>
    void ShowSystems(TMP_Dropdown dropdown)
    {
        List<string> names = new List<string>();
        //read systems list table
        DatabaseManager.Instance.ReadData("SystemsList", SelectFromDatabaseMode.everything,
            (data) =>
            {
                for (int i = 0; i < data.Count; i++)
                {
                    names.Add(data[i][0].StringValue);
                    Debug.Log(data[i][0].StringValue);
                }
                dropdown.ClearOptions();
                dropdown.AddOptions(names);
            });
    }
}
