using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnitySQLite;
using System.Collections.Generic;
using SPS;

public class InterfaceManager : GenericSingleton<InterfaceManager>
{
    [SerializeField]
    Service _currentService;

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
    public TMP_InputField hoursInput;

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

        _currentService = new Service(nameInput.text, idInput.text, int.Parse(hoursInput.text), serviceEntries);
    }
}
