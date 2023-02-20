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

    [Header("Login panel")]
    public GameObject loginPanel;

    [Header("User panel")]
    public GameObject userPanel;
    public Button logoutButton;
    public TextMeshProUGUI username;

    [Header("Service Interface")]
    public Button addMachineryButton;
    public Button addServiceEntryButton;
    public RectTransform serviceEntryParent;
    public TMP_InputField nameInput;
    public TMP_InputField idInput;
    public TMP_Dropdown deptDropdown;
    public TMP_Dropdown systemDropdown;
    public TMP_InputField hoursInput;

    [Header("Menu")]
    public GameObject manuPanel;
    public Button menuButton;
    public Button addSystemButton;

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
                AccountManagement.Instance.password.text = "";
                AccountManagement.Instance.username.text = "";

                //show info depending on account
            }
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

    private void Start()
    {
        addServiceEntryButton.onClick.AddListener(AddServiceEntry);
        addMachineryButton.onClick.AddListener(AddMachinery);
        createSystemEntryButton.onClick.AddListener(() => AddSystem(addSystemName.text));
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
        
        _currentService = new Service(nameInput.text, idInput.text, hoursInput.text.Length > 0? int.Parse(hoursInput.text) : 0, serviceEntries);
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
            Debug.Log("empty name");
            return;
        }

        DatabaseManager.Instance.WriteOnce(() =>
        {
            List<DataEntry> entries = new List<DataEntry>() { new DataEntry(name) };
            List<TableColumn> columns = tables.systemsList.columns;
            TableRow row = new TableRow(columns, entries);
            DatabaseManager.Instance.ThreadedWriteToDatabase("Systems List", row);
            Debug.Log("test");
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
        DatabaseManager.Instance.ReadData("Systems List", SelectFromDatabaseMode.everything, 
            (data) => 
            {
                for (int i = 0; i < data.Count; i++)
                {
                    names.Add(data[i][0].StringValue);
                    Debug.Log(data[i][0].StringValue);
                }
            }
            );

        dropdown.AddOptions(names);
    }
}
