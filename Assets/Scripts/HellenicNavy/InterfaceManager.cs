using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnitySQLite;
using System.Collections.Generic;
using SPS;
using UnitySQLite.Utilities;
using System.Collections;

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
    public TMP_InputField serialInput;
    public TMP_InputField idInput;
    public TMP_Dropdown deptDropdown;
    public TMP_Dropdown systemDropdown;
    public TMP_InputField hoursInput;

    [Header("Menu")]
    public GameObject menuPanel;
    public Button menuButton;
    public Button createDeptButton;
    public Button addSystemButton;
    public Button changePWButton;

    [Header("Add System Interface")]
    public GameObject addSystemPanel;
    public Button createSystemEntryButton;
    public TMP_InputField addSystemName;
    public TMP_Dropdown addSystemDept;
    public Button closeAddSystemPanel;

    [Header("Add Department Interface")]
    public GameObject addDeptPanel;
    public Button createDeptEntryButton;
    public TMP_InputField addDeptName;
    public Button closeDeptPanel;

    [Header("Select department interface")]
    public Button selectDeptButton;
    public GameObject selectDeptPanel;

    [Header("Select System Interface")]
    public GameObject selectSystemPanel;
    public int buttonsPerRow;
    private GameObject _currentRow;

    [Header("Prefabs")]
    public GameObject serviceEntryPrefab;
    public GameObject serviceTypePrefab;
    public GameObject selectDeptPrefab;
    public GameObject buttonRow;

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
                    useLeftButton = false,
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

        createSystemEntryButton.onClick.AddListener(() => AddSystem(addSystemName.text, addSystemDept.options[addSystemDept.value].text));
        addSystemButton.onClick.AddListener(() =>
        {
            ShowDepartments(addSystemDept);
            addSystemPanel.SetActive(!addSystemPanel.activeSelf);
        });
        closeAddSystemPanel.onClick.AddListener(() => addSystemPanel.SetActive(false));

        createDeptEntryButton.onClick.AddListener(() => AddDepartment(addDeptName.text));
        createDeptButton.onClick.AddListener(() => addDeptPanel.SetActive(!addDeptPanel.activeSelf));
        closeDeptPanel.onClick.AddListener(() => addDeptPanel.SetActive(false));

        selectDeptButton.onClick.AddListener(() => SetupDeptSelectionInterface());

        menuButton.onClick.AddListener(() =>
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            menuButton.transform.GetChild(1).rotation *= Quaternion.Euler(0, 0, 180);
        });

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
        selectDeptPanel.SetActive(false);
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
        //write to database
        DatabaseManager.Instance.WriteOnce(() =>
        {
            TableRow row = new TableRow(new TableColumn[] { new TableColumn("Name", "TEXT", false, true), new TableColumn("ServiceDescr", "TEXT") });
            row.AddValues(new DataEntry[] { new DataEntry(nameInput.text), new DataEntry(string.Format("\"{0}\"", _currentService.ToJson())) });

            DatabaseManager.Instance.CreateTableOnDatabase(nameInput.text, row.GetColumns());
            TableRow rowList = tables.machineryList;
            DataEntry[] entries = new DataEntry[]
            {
                new DataEntry(nameInput.text),
                new DataEntry(serialInput.text),
                new DataEntry(idInput.text),
                new DataEntry(deptDropdown.captionText.text),
                new DataEntry(systemDropdown.options[systemDropdown.value].text),
                new DataEntry(int.Parse(hoursInput.text)),
                new DataEntry(5),
                new DataEntry(1)
            };
            rowList.AddValues(entries);
            DatabaseManager.Instance.ThreadedWriteToDatabase("MachineryList", rowList, true);

            DatabaseManager.Instance.ThreadedWriteToDatabase(nameInput.text, row, true,
                () => MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    showLabel = false,
                    useRightButton = false,
                    useLeftButton = false,
                    mainText = "Τα δεδομένα υπάρχουν ήδη"
                }),
                () => MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    showLabel = false,
                    useRightButton = false,
                    useLeftButton = false,
                    mainText = "Επιτυχής εγγραφή δεδομένων"
                }));
        });
    }


    void AddDepartment(string name)
    {
        if (name.Equals(string.Empty))
        {
            return;
        }

        DatabaseManager.Instance.WriteOnce(() =>
        {
            List<DataEntry> entries = new List<DataEntry>() { new DataEntry(name) };
            List<TableColumn> columns = tables.departmentsList.columns;
            TableRow row = new TableRow(columns, entries);
            DatabaseManager.Instance.ThreadedWriteToDatabase("DepartmentsList", row, true,
                () => MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    useRightButton = false,
                    useLeftButton = false,
                    showLabel = false,
                    mainText = string.Format("Η επιστασία \"{0}\" υπάρχει ήδη στη βάση δεδομένων.", name),
                }
                ),
                () => MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    useRightButton = false,
                    useLeftButton = false,
                    showLabel = false,
                    mainText = string.Format("Η επιστασία \"{0}\" προστέθηκε στη βάση δεδομένων επιτυχώς.", name),
                }));
            //Debug.Log(string.Format("successfully added system {0} to the database", name));
        });
    }

    /// <summary>
    /// When adding a system, it should be written to the systems list table.
    /// </summary>
    /// <param name="name">System name.</param>
    void AddSystem(string name, string dept)
    {
        if (name.Equals(string.Empty))
        {
            return;
        }

        DatabaseManager.Instance.WriteOnce(() =>
        {
            List<DataEntry> entries = new List<DataEntry>() { new DataEntry(name), new DataEntry(dept) };
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
    /// When adding/editing machinery info, a dropdown that contains all departments & systems should appear.
    /// </summary>
    /// <param name="departments">Dropdown containing all departments.</param>
    /// <param name="systems">Dropdown containing all systems.</param>
    void ShowDepartmentsAndSystems(TMP_Dropdown departments, TMP_Dropdown systems)
    {
        List<string> deptNames = new List<string>();
        List<string> systemNames = new List<string>();
        //read departments table
        DatabaseManager.Instance.ReadData("SystemsDepartmentsList", SelectFromDatabaseMode.everything,
            (data) =>
            {

                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i][1].IntegerValue == 0)
                        deptNames.Add(data[i][0].StringValue);
                    else if (data[i][1].IntegerValue == 1)
                        systemNames.Add(data[i][0].StringValue);
                }
                if (departments != null) { departments.ClearOptions(); departments.AddOptions(deptNames); }

                if (systems != null) { systems.ClearOptions(); systems.AddOptions(systemNames); }
            });
    }

    void ShowDepartments(TMP_Dropdown departments)
    {
        List<string> deptNames = new List<string>();
        DatabaseManager.Instance.ReadData("DepartmentsList", SelectFromDatabaseMode.everything,
           (data) =>
           {
               for (int i = 0; i < data.Count; i++)
               {
                   deptNames.Add(data[i][0].StringValue);
               }
               if (departments != null) { departments.ClearOptions(); departments.AddOptions(deptNames); }
           });
    }

    void ShowSystems(TMP_Dropdown systems)
    {
        List<string> systemNames = new List<string>();
        DatabaseManager.Instance.ReadData("SystemsList", SelectFromDatabaseMode.everything,
           (data) =>
           {
               for (int i = 0; i < data.Count; i++)
               {
                   systemNames.Add(data[i][0].StringValue);
               }

               if (systems != null)
               {
                   systems.ClearOptions();
                   systems.AddOptions(systemNames);
                   systems.onValueChanged.AddListener((i) => deptDropdown.GetComponentInChildren<TextMeshProUGUI>().text = data[i][1].StringValue);
                   systems.onValueChanged?.Invoke(0);
               }
           });
    }

    void SetupDeptSelectionInterface()
    {
        if (!selectDeptPanel.activeSelf)
        {
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = false,
                useRightButton = false,
                useLeftButton = false,
                mainText = "Παρακαλώ περιμένετε, ανάγνωση δεδομένων...",
            });
            DatabaseManager.Instance.ReadData("DepartmentsList", SelectFromDatabaseMode.everything,
                (data) =>
                {
                    MessageBox.Instance.HideMessageBox();
                    for (int i = 0; i < data.Count; i++)
                    {
                        Button b = Instantiate(selectDeptPrefab, selectDeptPanel.transform).GetComponent<Button>();
                        string currentDept = data[i][0].StringValue;
                        b.name = currentDept;
                        b.GetComponentInChildren<TextMeshProUGUI>().text = b.name;
                        b.onClick.AddListener(() =>
                                         MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                                         {
                                             showLabel = true,
                                             label = "Επιλογή Επιστασίας",
                                             useRightButton = true,
                                             rightButtonLabel = "ΝΑΙ",
                                             onRightButtonClick = () =>
                                             {
                                                 /* read all machinery for specific dept from database (should open message box), then close*/
                                                 Debug.Log("Reading data");
                                                 selectDeptPanel.SetActive(false);
                                                 //MessageBox.Instance.HideMessageBox();
                                                 MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                                                 {
                                                     showLabel = false,
                                                     useRightButton = false,
                                                     useLeftButton = false,
                                                     mainText = string.Format("Ανάγνωση δεδομένων για \"{0}\", παρακαλώ περιμένετε...", currentDept),
                                                 }, -1); //message box should close when the data is read
                                                 DatabaseManager.Instance.ReadData("SystemsList", SelectFromDatabaseMode.everything,
                                                   (data) => StartCoroutine(SetupSystemsButtons(data, currentDept)), SortResultsBy.none, null, "", 0, 0, $"Where Department = '{currentDept}'");
                                             },
                                             useLeftButton = true,
                                             leftButtonLabel = "ΟΧΙ",
                                             onLeftButtonClick = () => MessageBox.Instance.HideMessageBox(),
                                             mainText = string.Format("Είστε σίγουροι ότι θέλετε να επιλέξετε {0};", currentDept),
                                         }, -1));
                    }
                    selectDeptPanel.SetActive(true);
                    MessageBox.Instance.HideMessageBox();
                });

        }
        else
        {
            selectDeptPanel.SetActive(false);
            Transform[] children = selectDeptPanel?.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name.Equals("Label") | children[i] == selectDeptPanel.transform)
                    continue;

                Destroy(children[i].gameObject);
            }
        }


    }

    IEnumerator SetupSystemsButtons(List<List<DataEntry>> data, string currentDept)
    {
        //setup interaface
        selectSystemPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("<b>{0}</b>\nΕπιλογή Συστήματος", currentDept);

        if (data.Count == 0 | data == null)
        {
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = string.Format("<b>Επιστασία {0}</b>", currentDept),
                useRightButton = false,
                useLeftButton = false,
                mainText = string.Format("Δεν υπάρχουν συστήματα στην επιστασία \"{0}\".", currentDept)
            }, 1.5f);
            yield break;
        }

        int buttonCount = 0;
        for (int i = 0; i < data.Count; i++)
        {
            if (buttonCount % buttonsPerRow == 0)
            {
                _currentRow = Instantiate(buttonRow, selectSystemPanel.transform.GetChild(1));
            }
            string systemName = data[i][0].StringValue;
            //instantiate buttons here
            Button current = Instantiate(selectDeptButton, _currentRow.transform);
            current.name = systemName;
            current.GetComponentInChildren<TextMeshProUGUI>().text = systemName;
            yield return new WaitForEndOfFrame();
            buttonCount++;
        }
        selectSystemPanel.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(selectSystemPanel.GetComponent<RectTransform>());
        MessageBox.Instance.HideMessageBox();

        Debug.Log($"read all for {currentDept}");
    }
}



//List<string> deptNames = new List<string>();
