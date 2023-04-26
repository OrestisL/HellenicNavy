using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnitySQLite;
using System.Collections.Generic;
using SPS;
using UnitySQLite.Utilities;
using System.Collections;
using System;
using System.IO;
using System.Linq;
using SimpleFileBrowser;

/// <summary>
/// Interface manager holds all necessary objects and functions for the interface.
/// </summary>
public class InterfaceManager : GenericSingleton<InterfaceManager>
{
    [SerializeField]
    public Service _currentService;
    public ServiceEntry _currentEntry;
    [SerializeField]
    CreateTables tables;
    public Dictionary<string, string> systemsDict = new Dictionary<string, string>();

    public GameObject canvas;

    [Header("Login panel")]
    public GameObject loginPanel;
    public Button loginButton;
    public Button updatePasswordButton;
    public Button quitButton;

    [Header("User panel")]
    public GameObject userPanel;
    public Button logoutButton;
    public TextMeshProUGUI username;

    [Header("Add Service Interface")]
    public GameObject addMachineryPanel;
    public Button addMachineryButton;
    public Button createMachineryButton;
    public Button addServiceEntryButton;
    public Button deleteSelectionButton;
    public Button closeAddMachineryPanel;
    public RectTransform serviceEntryParent;
    public TMP_InputField nameInput;
    public TMP_InputField descriptionInput;
    public TMP_InputField serialInput;
    public TMP_InputField idInput;
    public TMP_InputField dateInput;
    public TMP_Dropdown deptDropdown;
    public TMP_Dropdown systemDropdown;
    public TMP_InputField hoursInput;

    [Header("Display Machinery interface")]
    public GameObject displayMachineryPanel;
    public TextMeshProUGUI displayMachineryLabel;
    public Button updateMachineryButton;
    public Button displayAddServiceEntryButton;
    public Button displayDeleteSelectionButton;
    public Button displayPanelCloseButton;
    public Button displayEnableEditingButton;
    public Button displayDeleteMachineryButton;
    public Button displayCompleteServiceButton;
    public Button displayPostponeServiceButton;
    public RectTransform displayServiceEntryParent;
    public TMP_InputField displayNameInput;
    public TMP_InputField displayDescriptionInput;
    public TMP_InputField displaySerialInput;
    public TMP_InputField displayIdInput;
    public TMP_InputField displayDateInput;
    public TMP_Dropdown displayDeptDropdown;
    public TMP_Dropdown displaySystemDropdown;
    public TMP_InputField displayHoursInput;
    public TMP_InputField displayPreviousHours;
    public TMP_InputField displayNextHours;
    

    [Header("Menu")]
    public GameObject menuPanel;
    public Button menuButton;
    public Button createDeptButton;
    public Button addSystemButton;
    public Button changePWButton;
    public Button checkForServiceButton;
    public Button importExportMenuButton;

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
    private GameObject _currentRowSystems;

    [Header("Select Machinery Panel")]
    public GameObject selectMachineryPanel;
    private GameObject _currentRowMachinery;

    [Header("Select machinery with pending service")]
    public GameObject selectMachineryWithPendingServicePanel;
    private GameObject _currentRowPendingMachinery;

    [Header("Assignments Panel")]
    public GameObject assignmentsPanel;
    public GameObject assignmentsPanelScrollView;
    public Button addAsignmentButton;
    public Button markAssignmentCompleteButton;
    public Button closeAssignmentsPanelButton;

    [Header("Import/Export interface")]
    public GameObject importExportDBPanel;
    public Button importDBButton;
    public Button exportDBButton;
    public Button closeImportExportButton;

    [Header("Print day's report interface")]
    public Button printReportButton;

    [Header("Remarks interface")]
    public GameObject remarksPanel;
    public TMP_InputField remarksInputField;
    public Button submitRemarksButton;

    [Header("Prefabs")]
    public GameObject serviceEntryPrefab;
    public GameObject serviceTypePrefab;
    public GameObject selectDeptPrefab;
    public GameObject buttonRow;
    public GameObject assignmentPrefab;

    [Header("Text validators")]
    public TextValidator textValidator;
    public TextValidatorDateTime textValidatorDateTime;
    public TextValidatorNameInput textValidatorNameInput;

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
        ApplicationSetup();

        ResetUI();
        AccountManagement.onAfterLogin += (valid, acc) =>
        {
            if (valid)
            {
                CreateSystemDepartmentDictionary();
                username.text = acc.AccountName;
                loginPanel.SetActive(false);
                userPanel.SetActive(true);

                SetupInterface(acc.AccessLevel);
                //clear input fields
                AccountManagement.Instance.passwordField.text = "";
                AccountManagement.Instance.usernameField.text = "";

                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    useRightButton = false,
                    useLeftButton = false,
                    showLabel = false,
                    mainText = string.Format("Επιτυχής σύνδεση {0}.", acc.AccountName),
                }, 1.5f);

                //show info depending on account
                SetupInterface(acc.AccessLevel);
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

            //close connection?
            //DatabaseManager.Instance.CloseConnection();
        };

        logoutButton.onClick.AddListener(() => AccountManagement.onLogout?.Invoke());
    }

    private void Start()
    {
        SetupButtons();
        FileBrowser.AllFilesFilterText = "Αρχεία DB  (*.db)";
        FileBrowser.SetFilters(false, new string[] { ".db" });
        buttonsPerRow = SettingsHolder.Instance.settings.buttonsPerRow;
    }

    void SetupInterface(AccessLevel accessLevel)
    {
        //TODO hide UI elements according to access level
        switch (accessLevel)
        {
            case AccessLevel.user:
                //user should not be able to add system, dept and machinery
                addSystemButton.gameObject.SetActive(false);
                createDeptButton.gameObject.SetActive(false);
                addMachineryButton.gameObject.SetActive(false);
                displayEnableEditingButton.gameObject.SetActive(false);
                displayAddServiceEntryButton.gameObject.SetActive(false);
                displayDeleteSelectionButton.gameObject.SetActive(false);
                updateMachineryButton.gameObject.SetActive(false);
                displayDeleteMachineryButton.gameObject.SetActive(false);
                break;
            case AccessLevel.supervisor:

                break;
            case AccessLevel.admin:
                //admin should have access to all
                addSystemButton.gameObject.SetActive(true);
                createDeptButton.gameObject.SetActive(true);
                addMachineryButton.gameObject.SetActive(true);
                displayEnableEditingButton.gameObject.SetActive(true);
                displayAddServiceEntryButton.gameObject.SetActive(true);
                displayDeleteSelectionButton.gameObject.SetActive(true);
                updateMachineryButton.gameObject.SetActive(true);
                displayDeleteMachineryButton.gameObject.SetActive(true);
                break;
        }
        displayEnableEditingButton.onClick.RemoveAllListeners();
        displayEnableEditingButton.onClick.AddListener(() => { DisplayMachineryChangeButtonsStatus(accessLevel == AccessLevel.admin); EnableInputFieldsInDisplay(accessLevel); });
        //apply validator for dates
        if (textValidatorDateTime == null)
            textValidatorDateTime = ScriptableObject.CreateInstance<TextValidatorDateTime>();
        if (textValidator == null)
            textValidator = ScriptableObject.CreateInstance<TextValidator>();
        if (textValidatorNameInput == null)
            textValidatorNameInput = ScriptableObject.CreateInstance<TextValidatorNameInput>();

        dateInput.inputValidator = textValidatorDateTime;
        displayDateInput.inputValidator = textValidatorDateTime;
        nameInput.inputValidator = textValidatorNameInput;
        displayNameInput.inputValidator = textValidatorNameInput;
    }

    void SetupButtons()
    {
        #region login panel
        loginButton.onClick.AddListener(() =>
        {
            MessageBoxSettings settings = new MessageBoxSettings()
            {
                showLabel = false,
                useRightButton = false,
                useLeftButton = false,
                mainText = "Παρακαλώ περιμένετε...",
                showLoadingIndicator = true,

            };
            MessageBox.Instance.ShowMessageBox(settings);
            AccountManagement.Instance.Login();
        });
        quitButton.onClick.AddListener(QuitApplication);
        #endregion

        #region add machinery
        addMachineryButton.onClick.AddListener(() =>
        {
            if (systemsDict.Count == 0)
            {
                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings
                {
                    showLabel = false,
                    useLeftButton = false,
                    useRightButton = false,
                    mainText = "Δεν υπάρχουν συστήματα στη βάση δεδομένων, παρακαλώ χρησιμοποιήστε το αντίστοιχο κουμπί για να προσθέσετε συστήματα.",
                    showLoadingIndicator = false,
                }, 3);
                return;
            }
            ShowSystems(systemDropdown, deptDropdown);
            addMachineryPanel.SetActive(!addMachineryPanel.activeSelf);
        });
        addServiceEntryButton.onClick.AddListener(() => AddServiceEntry(serviceEntryParent));
        deleteSelectionButton.onClick.AddListener(() => DeleteSelectedServiceEntries(serviceEntryParent));
        createMachineryButton.onClick.AddListener(AddMachinery);
        closeAddMachineryPanel.onClick.AddListener(CloseAddManchineryPanel);
        #endregion

        #region display machinery

        displayDeleteSelectionButton.onClick.AddListener(() => DeleteSelectedServiceEntries(displayServiceEntryParent));
        displayAddServiceEntryButton.onClick.AddListener(() => AddServiceEntry(displayServiceEntryParent));
        updateMachineryButton.onClick.AddListener(UpdateMachineryInfo);

        displayPanelCloseButton.onClick.AddListener(() => CloseDisplayPanel());

        displayDeleteMachineryButton.onClick.AddListener(DeleteMachinery);

        displayCompleteServiceButton.onClick.AddListener(CompleteServiceEntries);
        displayPostponeServiceButton.onClick.AddListener(PostponeServiceEntries);
        #endregion

        #region system entry
        createSystemEntryButton.onClick.AddListener(() => AddSystem(addSystemName.text, addSystemDept.options[addSystemDept.value].text));
        addSystemButton.onClick.AddListener(() =>
        {
            ShowDepartments(addSystemDept);
            addSystemPanel.SetActive(!addSystemPanel.activeSelf);
        });
        closeAddSystemPanel.onClick.AddListener(() => addSystemPanel.SetActive(false));
        #endregion

        #region department panel
        createDeptEntryButton.onClick.AddListener(() => AddDepartment(addDeptName.text));
        createDeptButton.onClick.AddListener(() => addDeptPanel.SetActive(!addDeptPanel.activeSelf));
        closeDeptPanel.onClick.AddListener(() => addDeptPanel.SetActive(false));
        selectDeptButton.onClick.AddListener(() => { SetupDeptSelectionInterface(); });
        #endregion

        #region menu
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
        checkForServiceButton.onClick.AddListener(ServiceChecker.Instance.Check);

        importExportMenuButton.onClick.AddListener(() => importExportDBPanel.SetActive(!importExportDBPanel.activeSelf));

        #endregion

        #region date checker
        dateInput.onDeselect.AddListener((dt) => TextValidatorDateTime.CheckDateInput(dt));
        dateInput.onSubmit.AddListener((dt) => TextValidatorDateTime.CheckDateInput(dt));
        dateInput.onEndEdit.AddListener((dt) => TextValidatorDateTime.CheckDateInput(dt));
        displayDateInput.onDeselect.AddListener((dt) => TextValidatorDateTime.CheckDateInput(dt));
        displayDateInput.onSubmit.AddListener((dt) => TextValidatorDateTime.CheckDateInput(dt));
        displayDateInput.onEndEdit.AddListener((dt) => TextValidatorDateTime.CheckDateInput(dt));
        #endregion

        #region assignments
        addAsignmentButton.onClick.AddListener(AddAssignment);
        markAssignmentCompleteButton.onClick.AddListener(MarkAssignmentsComplete);
        closeAssignmentsPanelButton.onClick.AddListener(CloseAssignmentsPanel);
        #endregion

        #region import export db
        importDBButton.onClick.AddListener(() => ImportDatabase());
        exportDBButton.onClick.AddListener(() => ExportDatabase());
        #endregion

        #region report
        printReportButton.interactable = false;
        printReportButton.onClick.AddListener(() => remarksPanel.SetActive(!remarksPanel.activeSelf));
        submitRemarksButton.onClick.AddListener(() => SubmitRemarksAndPrint());
        #endregion
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

    void ApplicationSetup()
    {
        Application.targetFrameRate = SettingsHolder.Instance.settings.rate;
        QualitySettings.vSyncCount = 0;
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].DelayedEnableButton(0.5f);

            if (buttons[i].GetComponent<ReEnableButton>() == null)
                buttons[i].gameObject.AddComponent<ReEnableButton>();
        }
    }

    void ResetUI()
    {
        menuPanel.SetActive(false);
        userPanel.SetActive(false);
        addSystemPanel.SetActive(false);
        addMachineryPanel.SetActive(false);
        selectDeptPanel.SetActive(false);
        displayMachineryPanel.SetActive(false);
        addDeptPanel.SetActive(false);
        selectSystemPanel.SetActive(false);
        selectMachineryPanel.SetActive(false);
        selectMachineryWithPendingServicePanel.SetActive(false);
        importExportDBPanel.SetActive(false);
    }

    void CreateSystemDepartmentDictionary()
    {
        DatabaseManager.Instance.ReadData("SystemsList", SelectFromDatabaseMode.everything,
             (data) =>
             {
                 if (data == null | data.Count == 0)
                     return;

                 systemsDict = new Dictionary<string, string>();
                 for (int i = 0; i < data.Count; i++)
                 {
                     systemsDict.Add(data[i][0].StringValue, data[i][1].StringValue);
                 }

                 Debug.Log($"Remade dictionary, new size is {systemsDict.Count}.");
             });
    }

    void AddServiceEntry(Transform parent)
    {
        Instantiate(serviceEntryPrefab, parent);
    }

    void DeleteSelectedServiceEntries(Transform parent)
    {
        ServiceEntry[] currentEntries = parent.GetComponentsInChildren<ServiceEntry>();
        foreach (ServiceEntry entry in currentEntries)
        {
            if (entry.IsSelected) { Destroy(entry.gameObject); }
        }
    }

    public void ExportDatabase()
    {
        //first close connection to commit changes
        DatabaseManager.Instance.CloseConnection();
        //open dialogue for folder selection only
        string localDiskPath = @"C:\";
        FileBrowser.OnSuccess onSuccess = delegate (string[] paths)
        {
            bool exists = false;
            //paths will always have length 1 because multi selection will be disabled
            string location = DatabaseManager.Instance.GetDatabaseFullPath();
            string target = Path.Combine(paths[0], DatabaseManager.Instance.GetDatabaseName());
            try
            {
                File.Copy(location, target);
            }
            catch (Exception e)
            {
                exists = true;
                Utilities.LogException(e);
                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings
                {
                    showLabel = false,
                    useLeftButton = false,
                    useRightButton = false,
                    mainText = "Το αρχείο υπάρχει ήδη στην επιλεγμένη τοποθεσία.",
                    showLoadingIndicator = false,
                });
            }
            finally
            {
                if (!exists)
                {
                    //show message box
                    MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                    {
                        showLabel = false,
                        mainText = string.Format("Επιτυχής εξαγωγή βάσης δεδομένων σε {0}.", target),
                        useRightButton = false,
                        useLeftButton = false,
                        showLoadingIndicator = false,
                    });
                    Debug.Log(string.Format("Success when exporting database at {0}.", paths[0]));


                }
                //re open connection
                DatabaseManager.Instance.Initialize("Databases", "Machinery");
            }

        };
        FileBrowser.OnCancel onCancel = delegate
        {
            Debug.Log("Export folder selection was canceled by user.");
        };
        if (FileBrowser.ShowLoadDialog(onSuccess, onCancel, FileBrowser.PickMode.Folders, false, localDiskPath))
        {
            Debug.Log("Opened export dialogue");
            importExportDBPanel.SetActive(false);
        };
    }

    public void ImportDatabase()
    {
        //stop the connection with the database to avoid weird issues
        DatabaseManager.Instance.CloseConnection();
        //open dialogue for file selection only (starting on C:\)
        string localDiskPath = @"C:\";

        //on success, save the path _somewhere_
        FileBrowser.OnSuccess onSuccess = delegate (string[] paths)
        {
            string target = DatabaseManager.Instance.GetDatabaseFullPath();
            bool success = true;
            //copy the file from the path above to the path in database manager (overwrite)
            try
            {
                File.Copy(paths[0], target, true);
            }
            catch (Exception e)
            {
                success = false;
                Utilities.LogException(e);
            }
            finally
            {
                if (success)
                {
                    MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                    {
                        showLabel = false,
                        useLeftButton = false,
                        useRightButton = false,
                        mainText = "Επιτυχής εισαγωγή βάσης δεδομένων.",
                        showLoadingIndicator = false,

                    });
                    //re create the connection to the database
                    DatabaseManager.Instance.Initialize("Databases", "Machinery");
                }
            }

        };
        FileBrowser.OnCancel onCancel = delegate
        {
            Debug.Log("Import file was canceled by user.");
        };
        if (FileBrowser.ShowLoadDialog(onSuccess, onCancel, FileBrowser.PickMode.Files, false, localDiskPath))
        {
            Debug.Log("Opened import dialogue");
            importExportDBPanel.SetActive(false);
        };

    }

    void AddMachinery()
    {
        if (nameInput.text == "")
        {
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = "Αποθήκευση πληροφοριών μηχανήματος",
                useLeftButton = false,
                useRightButton = false,
                showLoadingIndicator = false,
                mainText = "Το πεδίο \"Όνομα Μηχανήματος\" δεν μπορεί να είναι κενό.",
            });
            return;
        }

        //ensure date is correct
        if (!TextValidatorDateTime.CheckDateInput(dateInput.text))
        {
            return;
        }

        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings
        {
            showLabel = false,
            mainText = string.Format("Αποθήκευση πληροφοριών μηχανήματος \"{0}\"...", nameInput.text),
            showLoadingIndicator = true,
            useLeftButton = false,
            useRightButton = false,
        }, -1);

        serviceEntries = new List<ServiceEntry>();

        for (int i = 0; i < serviceEntryParent.childCount; i++)
        {
            serviceEntries.Add(serviceEntryParent.GetChild(i).GetComponent<ServiceEntry>());
        }

        _currentService = new Service(
            nameInput.text, descriptionInput.text, idInput.text, hoursInput.text.Length > 0 ? int.Parse(hoursInput.text) : 0,
            0, 0, dateInput.text, systemDropdown.value,
            serviceEntries);
        //creating machinery should have 0 as starting hours
        Debug.Log(_currentService.ToJson());
        //write to database
        DatabaseManager.Instance.WriteOnce(() =>
        {
            TableRow row = new TableRow(new TableColumn[] { new TableColumn("Name", "TEXT", false, true), new TableColumn("ServiceDescr", "TEXT") });
            row.AddValues(new DataEntry[] { new DataEntry(nameInput.text), new DataEntry(string.Format("{0}", _currentService.ToJson())) });

            DatabaseManager.Instance.CreateTableOnDatabase(nameInput.text, row.GetColumns());
            TableRow rowList = tables.machineryList;
            DataEntry[] entries = new DataEntry[]
            {
                new DataEntry(nameInput.text),
                new DataEntry(descriptionInput.text),
                new DataEntry(serialInput.text),
                new DataEntry(idInput.text),
                new DataEntry(deptDropdown.captionText.text),
                new DataEntry(systemDropdown.options[systemDropdown.value].text),
                new DataEntry(int.Parse(hoursInput.text)),
                new DataEntry(dateInput.text),
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
                    mainText = "Τα δεδομένα υπάρχουν ήδη."
                }),
                () => MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    showLabel = false,
                    useRightButton = false,
                    useLeftButton = false,
                    mainText = "Επιτυχής εγγραφή δεδομένων."
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
                () =>
                {
                    MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                    {
                        useRightButton = false,
                        useLeftButton = false,
                        showLabel = false,
                        mainText = string.Format("To σύστημα \"{0}\" προστέθηκε στη βάση δεδομένων επιτυχώς.", name),
                    });
                    CreateSystemDepartmentDictionary();
                });
            //Debug.Log(string.Format("successfully added system {0} to the database", name));
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

    void ShowSystems(TMP_Dropdown systems, TMP_Dropdown depts, int defaultValue = 0)
    {
        List<string> systemsFromDict = systemsDict.Select(x => x.Key).ToList();
        List<string> deptsFromDict = systemsDict.Select(x => x.Value).ToList();

        if (systems != null)
        {
            systems.ClearOptions();
            systems.AddOptions(systemsFromDict);
            depts.ClearOptions();
            depts.AddOptions(deptsFromDict);
            systems.value = defaultValue;
            systems.onValueChanged.AddListener((i) =>
            {
                depts.value = i;
            });
            systems.onValueChanged.Invoke(defaultValue); //this does not work
            //depts.GetComponentInChildren<TextMeshProUGUI>().text = systemsDict[systemsFromDict[defaultValue]];
        }
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
                showLoadingIndicator = true,
            }, -1);
            DatabaseManager.Instance.ReadData("DepartmentsList", SelectFromDatabaseMode.everything,
                (data) =>
                {
                    selectDeptPanel.transform.ClearChildren();
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
                                        showLoadingIndicator = true,
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
            selectSystemPanel.transform.ClearChildren();
        }

    }

    IEnumerator SetupSystemsButtons(List<List<DataEntry>> data, string currentDept)
    {
        //setup interaface
        selectSystemPanel.transform.GetChild(1).ClearChildren();
        Transform labelParent = selectSystemPanel.transform.GetChild(0);
        labelParent.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("<b>{0}</b>\nΕπιλογή Συστήματος", currentDept);
        Button closeButton = selectSystemPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            selectSystemPanel.SetActive(false);
            selectSystemPanel.transform.GetChild(1).ClearChildren();
            SetupDeptSelectionInterface();
        });

        if (data.Count == 0 | data == null)
        {
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = string.Format("<b>Επιστασία {0}</b>", currentDept),
                useRightButton = false,
                useLeftButton = false,
                mainText = string.Format("Δεν υπάρχουν συστήματα στην επιστασία \"{0}\".", currentDept),
                onHide = () =>
                {
                    SetupDeptSelectionInterface();
                },
            }, 1f);

            yield break;
        }

        int buttonCount = 0;
        for (int i = 0; i < data.Count; i++)
        {
            if (buttonCount % buttonsPerRow == 0)
            {
                _currentRowSystems = Instantiate(buttonRow, selectSystemPanel.transform.GetChild(1));
            }
            string systemName = data[i][0].StringValue;
            //instantiate buttons here
            Button current = Instantiate(selectDeptButton, _currentRowSystems.transform);
            current.name = systemName;
            current.GetComponentInChildren<TextMeshProUGUI>().text = systemName;
            //each button should setup the machinery buttons for the system
            current.onClick.AddListener(() => SetupMachinerySelectionInterface(systemName));
            yield return new WaitForEndOfFrame();
            buttonCount++;
        }
        selectSystemPanel.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(selectSystemPanel.GetComponent<RectTransform>());
        MessageBox.Instance.HideMessageBox();

        Debug.Log($"read all systems for {currentDept}");
    }

    void SetupMachinerySelectionInterface(string currentSystem)
    {
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            showLabel = false,
            useRightButton = false,
            useLeftButton = false,
            mainText = string.Format("Ανάγνωση δεδομένων για \"{0}\", παρακαλώ περιμένετε...", currentSystem),
            showLoadingIndicator = true,
        }, -1); //message box should close when the data is read

        DatabaseManager.Instance.ReadData("MachineryList", SelectFromDatabaseMode.everything,
            (data) =>
            {
                StartCoroutine(SetupMachineryButtons(data, currentSystem));
            },
            SortResultsBy.none, null, "", 0, 0, string.Format("Where System = '{0}' AND isActive = 1", currentSystem));
    }

    void CloseAddManchineryPanel()
    {
        if (DatabaseManager.Instance.CheckIfTableExists(nameInput.text))
        {
            addMachineryPanel.SetActive(false);
            return;
        }
        //show message box
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            showLabel = true,
            label = "Αποθήκευση δεδομένων μηχανήματος",
            useRightButton = true,
            useLeftButton = true,
            showLoadingIndicator = false,
            mainText = "Θέλετε να αποθηκεύσετε τα δεδομένα;",
            rightButtonLabel = "NAI",
            leftButtonLabel = "OXI",
            onLeftButtonClick = () => { MessageBox.Instance.HideMessageBox(); addMachineryPanel.SetActive(false); },
            onRightButtonClick = () => { AddMachinery(); },
        }, -1);
    }

    void DisplayMachineryChangeButtonsStatus(bool status)
    {
        displayAddServiceEntryButton.interactable = status;
        displayDeleteSelectionButton.interactable = status;
        updateMachineryButton.interactable = status;
        displayDeleteMachineryButton.interactable = status;
        displayCompleteServiceButton.interactable = status;
        displayPostponeServiceButton.interactable = status;
    }

    void EnableInputFieldsInDisplay(AccessLevel level)
    {
        switch (level)
        {
            case AccessLevel.user:
                break;
            case AccessLevel.supervisor:
                displayHoursInput.interactable = true;
                break;
            case AccessLevel.admin:
                displayHoursInput.interactable = true;
                displayDateInput.interactable = true;
                displayDescriptionInput.interactable = true;
                displaySystemDropdown.interactable = true;
                displayIdInput.interactable = true;
                break;
        }
    }

    void CompleteServiceEntries()
    {
        ServiceEntry[] currentEntries = displayServiceEntryParent.GetComponentsInChildren<ServiceEntry>();
        _currentService.SetServiceStatusCompleted(currentEntries);
    }

    void PostponeServiceEntries()
    {
        ServiceEntry[] currentEntries = displayServiceEntryParent.GetComponentsInChildren<ServiceEntry>();
        _currentService.SetServiceStatusPostponed(currentEntries);
    }

    void CloseDisplayPanel()
    {
        //everyone should be able to update info on close
        //show message box in order to save the data
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            showLabel = true,
            label = "Ανανέωση δεδομένων μηχανήματος",
            useRightButton = true,
            useLeftButton = true,
            showLoadingIndicator = false,
            mainText = "Θέλετε να αποθηκεύσετε τις αλλαγές;",
            rightButtonLabel = "NAI",
            leftButtonLabel = "OXI",
            onLeftButtonClick = () => { MessageBox.Instance.HideMessageBox(); displayMachineryPanel.SetActive(false); },
            onRightButtonClick = () => { UpdateMachineryInfo(); },
        }, -1);
    }

    IEnumerator SetupMachineryButtons(List<List<DataEntry>> data, string currentSystem)
    {
        selectMachineryPanel.transform.GetChild(1).ClearChildren();
        selectSystemPanel.SetActive(false);
        Transform labelParent = selectMachineryPanel.transform.GetChild(0);
        labelParent.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("<b>{0}</b>\nΕπιλογή Μηχανήματος", currentSystem);
        Button closeButton = selectMachineryPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            selectMachineryPanel.SetActive(false);
            selectMachineryPanel.transform.GetChild(1).ClearChildren();
            selectSystemPanel.SetActive(true);
        });

        if (data.Count == 0 | data == null)
        {
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = string.Format("<b>Σύστημα {0}</b>", currentSystem),
                useRightButton = false,
                useLeftButton = false,
                mainText = string.Format("Δεν υπάρχουν μηχανήματα στο σύστημα \"{0}\".", currentSystem),
                onHide = () => { selectSystemPanel.SetActive(true); },
            }, 1.5f);

            yield break;
        }

        int buttonCount = 0;
        for (int i = 0; i < data.Count; i++)
        {
            if (buttonCount % buttonsPerRow == 0)
            {
                _currentRowMachinery = Instantiate(buttonRow, selectMachineryPanel.transform.GetChild(1));
            }
            string machineryName = data[i][0].StringValue;
            //instantiate buttons here
            Button current = Instantiate(selectDeptButton, _currentRowMachinery.transform);
            current.name = machineryName;
            current.GetComponentInChildren<TextMeshProUGUI>().text = machineryName;
            //each button should setup the machinery buttons for the system
            current.onClick.AddListener(() => ShowMachineryEntry(machineryName));
            yield return new WaitForEndOfFrame();
            buttonCount++;
        }
        yield return new WaitForEndOfFrame();
        selectMachineryPanel.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(selectMachineryPanel.GetComponent<RectTransform>());
        MessageBox.Instance.HideMessageBox();
    }

    public Button ShowMachineryWithPendingServiceButtons(string machineryName)
    {
        if (_currentRowPendingMachinery == null)
        {
            _currentRowPendingMachinery = Instantiate(buttonRow, selectMachineryWithPendingServicePanel.transform.GetChild(1));
        }
        else if (_currentRowPendingMachinery.transform.childCount > buttonsPerRow)
        {
            _currentRowPendingMachinery = Instantiate(buttonRow, selectMachineryWithPendingServicePanel.transform.GetChild(1));
        }
        Button current = Instantiate(selectDeptButton, _currentRowPendingMachinery.transform);
        current.name = machineryName;
        current.GetComponentInChildren<TextMeshProUGUI>().text = machineryName;
        //each button should setup the machinery buttons for the system
        current.onClick.AddListener(() => { ShowMachineryEntry(machineryName); });

        return current;
    }

    void ShowMachineryEntry(string machineryName)
    {
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            showLabel = false,
            mainText = string.Format("Ανάγνωση δεδομένων για {0}.", machineryName),
            showLoadingIndicator = true,
            useLeftButton = false,
            useRightButton = false,
        });
        DatabaseManager.Instance.ReadData(machineryName.Replace('.', ','), SelectFromDatabaseMode.everything,
            (data) =>
            {
                StartCoroutine(PopulateServiceEntriesForDisplayMachinery(data));
            });
    }

    IEnumerator PopulateServiceEntriesForDisplayMachinery(List<List<DataEntry>> data)
    {
        //clear previous entries
        displayServiceEntryParent.ClearChildren();
        //create new service from json
        Service serv = new Service(data[0][1].StringValue);
        _currentService = serv;
        _currentService.lastServiceHours = serv.lastServiceHours;
        _currentService.nextServiceHours = serv.nextServiceHours;
        ShowSystems(displaySystemDropdown, displayDeptDropdown, serv.systemName);
        //change label
        displayMachineryLabel.text = string.Format("<u>Επιστασία {0}, Σύστημα {1}, Πληροφορίες Μηχανήματος {2}</u>",
            displayDeptDropdown.options[serv.systemName].text, displaySystemDropdown.options[serv.systemName].text, serv.name);

        displayNameInput.text = serv.name;
        displayDescriptionInput.text = serv.descr;
        displayIdInput.text = serv.id;
        displayHoursInput.text = serv.CurrentHours.ToString();
        displayNextHours.text = serv.nextServiceHours.ToString();
        displayPreviousHours.text = serv.lastServiceHours.ToString();
        displayDateInput.text = serv.lastServiceDate.ToString("dd-MM-yy");

        for (int i = 0; i < serv.descriptions.Count; i++)
        {
            ServiceEntry currentEntry = Instantiate(serviceEntryPrefab, displayServiceEntryParent).GetComponent<ServiceEntry>();
            //currentEntry.DisplayFromData(serv.descriptions[i], serv.serviceHours[i], serv.serviceDays[i], serv.serviceTypesHours[i], serv.serviceTypesDays[i]);
            StartCoroutine(currentEntry.CreateInterfaceFromData(serv.descriptions[i], serv.serviceHours[i], serv.serviceDays[i], /*serv.serviceTypesHours[i], serv.serviceTypesDays[i],*/ serv.serviceStatuses[i], serv.serviceAssignments[i]));
            yield return new WaitForEndOfFrame();
        }

        //create service here
        serviceEntries = new List<ServiceEntry>();

        for (int i = 0; i < displayServiceEntryParent.childCount; i++)
        {
            serviceEntries.Add(displayServiceEntryParent.GetChild(i).GetComponent<ServiceEntry>());
            yield return new WaitForEndOfFrame();
        }

        _currentService = new Service(
            displayNameInput.text, displayDescriptionInput.text, displayIdInput.text, displayHoursInput.text.Length > 0 ? int.Parse(displayHoursInput.text) : 0,
            serv.lastServiceHours, serv.nextServiceHours, displayDateInput.text, displaySystemDropdown.value,
            serviceEntries);

        displayMachineryPanel.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(displayEnableEditingButton.transform.parent.parent.GetComponent<RectTransform>());
        MessageBox.Instance.HideMessageBox();
    }

    void SubmitRemarksAndPrint() 
    {
        if (remarksInputField.text.Length == 0) 
        {
            //no remarks
            Report.Remarks = "Δεν υπάρχουν παρατηρήσεις.";
        }

        remarksPanel.gameObject.SetActive(false);
        Report.ThreadedCreatePDF();

    }
    void UpdateMachineryInfo()
    {
        //show message box 
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            useLeftButton = false,
            useRightButton = false,
            showLabel = false,
            showLoadingIndicator = true,
            mainText = "Παρακαλώ περιμένετε, ανανέωση δεδομένων...",
        });
        //update some info
        if (!_currentService.ChangeHours(int.Parse(displayHoursInput.text)))
            return;
        serviceEntries = new List<ServiceEntry>();

        for (int i = 0; i < displayServiceEntryParent.childCount; i++)
        {
            serviceEntries.Add(displayServiceEntryParent.GetChild(i).GetComponent<ServiceEntry>());
        }
        _currentService.ChangeEntries(serviceEntries.ToArray());

        Debug.Log(_currentService.ToJson());
        //write to database
        DatabaseManager.Instance.WriteOnce(() =>
        {
            TableRow row = new TableRow(new TableColumn[] { new TableColumn("Name", "TEXT", false, true), new TableColumn("ServiceDescr", "TEXT") });
            row.AddValues(new DataEntry[] { new DataEntry(displayNameInput.text), new DataEntry(string.Format("{0}", _currentService.ToJson())) });

            TableRow rowList = tables.updateMachinery;
            DataEntry[] entries = new DataEntry[]
            {
                new DataEntry(displayDescriptionInput.text),
                new DataEntry(displaySerialInput.text),
                new DataEntry(displayIdInput.text),
                new DataEntry(displayDeptDropdown.captionText.text),
                new DataEntry(displaySystemDropdown.options[displaySystemDropdown.value].text),
                new DataEntry(int.Parse(displayHoursInput.text)),
                new DataEntry(displayDateInput.text),
                new DataEntry(1)
            };
            rowList.AddValues(entries);

            //update specific machinery table
            DatabaseManager.Instance.UpdateValuesOnTable(displayNameInput.text, row.GetColumnNames(), row.GetValues());
            //update machinery list
            DatabaseManager.Instance.UpdateValuesOnTable("MachineryList", rowList.GetColumnNames(), rowList.GetValues(), $"Name = '{displayNameInput.text}'");

            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    mainText = string.Format("Επιτυχής ανανέωση δεδομένων για μηχάνημα \"{0}\".", displayNameInput.text),
                    useRightButton = false,
                    useLeftButton = false,
                    showLabel = false,
                    showLoadingIndicator = false,
                    onHide = () => displayMachineryPanel.SetActive(false),
                }, 1f);
            });
        });
    }

    void AddAssignment()
    {
        Instantiate(assignmentPrefab, assignmentsPanel.transform.GetChild(1).GetChild(0).GetChild(0));
    }

    void CloseAssignmentsPanel()
    {
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            showLabel = true,
            label = "Αποθήκευση αλλαγών",
            mainText = "Είστε σίγουροι ότι θέλετε να αποθηκεύσετε τις αλλαγές;",
            useRightButton = true,
            rightButtonLabel = "NAI",
            onRightButtonClick = () =>
            {
                MessageBox.Instance.HideMessageBox(); List<Assignment> assignments = assignmentsPanelScrollView.GetComponentsInChildren<Assignment>().ToList();
                List<ServiceAssignment> serviceAssignments = new List<ServiceAssignment>(assignments.Count);
                List<ServiceStatus> serviceStatuses = new List<ServiceStatus>();
                for (int i = 0; i < assignments.Count; i++)
                {
                    serviceAssignments.Add(new ServiceAssignment(assignments[i].descrInput.text, assignments[i].isSelected.isOn));
                    serviceStatuses.Add(ServiceStatus.pending);
                }
                _currentEntry.assignments = serviceAssignments;
                _currentEntry.assignmentsStatuses = serviceStatuses;
                _currentEntry = null;
                assignmentsPanel.SetActive(false);
                assignmentsPanel.SetActive(false);
            },
            useLeftButton = true,
            leftButtonLabel = "OXI",
            onLeftButtonClick = () => { MessageBox.Instance.HideMessageBox(); assignmentsPanel.SetActive(false); },

        }, -1);

    }

    void MarkAssignmentsComplete()
    {
        Transform[] assignments = assignmentsPanel.transform.GetChild(1).GetChild(0).GetChild(0).GetComponentsInChildren<Transform>();
        if (assignments != null) { _currentService.lastServiceHours = int.Parse(hoursInput.text); _currentService.lastServiceDate = DateTime.Now; }
        foreach (Transform assignment in assignments)
        {
            Assignment assign = assignment.GetComponent<Assignment>();
            if (assign.isSelected.isOn)
            {
                assign.Complete();
            }
        }
    }

    void DeleteMachinery()
    {
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            showLabel = true,
            label = "Διαγραφή Μηχανήματος",
            mainText = string.Format("Είστε σίγουροι ότι θέλετε να διαγράψετε το μηχάνημα \"{0}\";", displayNameInput.text),
            useRightButton = true,
            rightButtonLabel = "ΝΑΙ",
            onRightButtonClick = () =>
            {
                displayMachineryPanel.SetActive(false);
                DatabaseManager.Instance.DeleteRowsOnTable("MachineryList", string.Format("Name = '{0}'", displayNameInput.text));
                DatabaseManager.Instance.DeleteTableFromDatabase(displayNameInput.text,
                    () =>
                    {
                        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                        {
                            showLabel = true,
                            label = "Διαγραφή Μηχανήματος",
                            mainText = string.Format("Επιτυχής διαγραφή μηχανήματος {0}.", displayNameInput.text),
                            useRightButton = false,
                            useLeftButton = false,
                            showLoadingIndicator = false,
                        });
                        selectMachineryPanel.SetActive(false);
                        selectSystemPanel.SetActive(false);
                        SetupDeptSelectionInterface();
                        //MessageBox.Instance.HideMessageBox();
                    });

            },
            useLeftButton = true,
            leftButtonLabel = "OXI",
            onLeftButtonClick = () =>
            {
                MessageBox.Instance.HideMessageBox();
            },
            showLoadingIndicator = false,
        }, -1);
    }

    void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();
#endif

    }
}