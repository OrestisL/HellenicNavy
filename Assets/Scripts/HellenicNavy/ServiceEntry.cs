using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SPS;
using System.Linq;
using UnityEngine.UI;
using UnitySQLite;


/// <summary>
/// Each service entry holds all the required information about a service (description, frequency in hours and days)
/// </summary>
public class ServiceEntry : MonoBehaviour
{
    public TMP_InputField descriptionField;
    public TMP_InputField hoursField;
    public TMP_InputField daysField;
    public Button displayAssignments;
    public Toggle selectionToggle;
    public Image bgImg;
    public Color normalColor, selectedColor, postponedColor, highlightedColor;
    public RectTransform serviceTypesParentHours;
    public RectTransform serviceTypesParentDays;
    public List<ServiceAssignmentType> serviceAssignmentTypesHours;
    public List<ServiceAssignmentType> serviceAssignmentTypesDays;
    public List<ServiceAssignment> assignments;
    public List<ServiceStatus> assignmentsStatuses;
    public string lastServiceDate;

    [SerializeField]
    private ServiceStatus status;
    public ServiceStatus Status 
    {
        get { return status; }
        set { status = value; bgImg.color = ChangeBGColor(); }
    }

    public int Hours
    {
        get
        {
            hoursField.text = hoursField.text.Trim();
            return int.Parse(hoursField.text); //for some reason this didnt work once, check
        }
    }

    public int Days
    {
        get
        {
            if (daysField.text.Length > 0)
            {
                daysField.text = daysField.text.Trim();

                return int.Parse(daysField.text);
            }
            return 0;
        }
    }

    public string Descr { get { return descriptionField.text; } }

    public ServiceAssignmentType TypesHours
    {
        get
        {
            return (ServiceAssignmentType)serviceTypesParentHours.GetChild(0).GetComponent<TMP_Dropdown>().value;
        }
    }

    public ServiceAssignmentType TypesDays
    {
        get
        {
            return (ServiceAssignmentType)serviceTypesParentDays.GetChild(0).GetComponent<TMP_Dropdown>().value;
        }
    }

    public bool IsSelected { get { return selectionToggle.isOn; } set { selectionToggle.isOn = value; } }

    private void Start()
    {
        //validator for input
        TextValidator validator = InterfaceManager.Instance.textValidator;
        //validator.name = "Input Validation for Hours and Days";
        hoursField.inputValidator = validator;
        daysField.inputValidator = validator;

        hoursField.onEndEdit.AddListener((s) =>
        {
            //clear whitespace
            hoursField.text = hoursField.text.Trim();
            if (s.Equals(string.Empty))
            {
                ClearChildren(serviceTypesParentHours);
            }
            else
            {
                int lineCount = hoursField.text.Count(x => x == '\n') + 1;
                int existing = serviceTypesParentHours.childCount;
                if (existing < lineCount)
                {
                    //need to add more 
                    for (int i = 0; i < lineCount - existing; i++)
                    {
                        Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentHours);
                    }
                }
                else if (existing > lineCount)
                {
                    //delete from bottom
                    for (int i = existing - 1; i >= lineCount; i--)
                    {
                        Destroy(serviceTypesParentHours.GetChild(i).gameObject);
                    }
                }

            }
        });

        daysField.onEndEdit.AddListener((s) =>
        {
            //clear whitespace
            daysField.text = daysField.text.Trim();
            if (s.Equals(string.Empty))
            {
                ClearChildren(serviceTypesParentDays);
            }
            else
            {
                int lineCount = daysField.text.Count(x => x == '\n') + 1;
                int existing = serviceTypesParentDays.childCount;
                if (existing < lineCount)
                {
                    //need to add more 
                    for (int i = 0; i < lineCount - existing; i++)
                    {
                        Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentDays);
                    }
                }
                else if (existing > lineCount)
                {
                    //delete from bottom
                    for (int i = existing - 1; i >= lineCount; i--)
                    {
                        Destroy(serviceTypesParentDays.GetChild(i).gameObject);
                    }
                }
            }
        });

        selectionToggle.onValueChanged.AddListener((b) => { bgImg.color = b ? selectedColor : ChangeBGColor();   });

        displayAssignments.onClick.AddListener(ShowAssignments);
    }

    public Color ChangeBGColor() 
    {
        switch (status)
        {
            case ServiceStatus.pending:
                return highlightedColor;
            case ServiceStatus.completed:
                return normalColor;
            case ServiceStatus.postponed:
                return postponedColor;
        }
        return normalColor;
    }

    public IEnumerator CreateInterfaceFromData(string descr, int hours, int days, /*ServiceAssignmentType serviceTypesHours, ServiceAssignmentType serviceTypesDays,*/ ServiceStatus status, List<ServiceAssignment> _assignments)
    {
        bool accessible = AccountManagement.Instance.CurrentAccount.AccessLevel == UnitySQLite.Utilities.AccessLevel.admin;
        descriptionField.text = descr;
        descriptionField.interactable = accessible;

        hoursField.text += string.Format("{0}", hours);
        //TMP_Dropdown dh = Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentHours).GetComponent<TMP_Dropdown>();
        //dh.value = (int)serviceTypesHours;
        //dh.interactable = accessible;
        hoursField.interactable = accessible;
        yield return new WaitForEndOfFrame();

        hoursField.text.TrimEnd();

        daysField.text += string.Format("{0}", days);
        //TMP_Dropdown dd = Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentDays).GetComponent<TMP_Dropdown>();
        //dd.value = (int)serviceTypesDays;
        //dd.interactable = accessible;
        daysField.interactable = accessible;
        yield return new WaitForEndOfFrame();

        Status = status;
        daysField.text.TrimEnd();

        assignments = new List<ServiceAssignment>();
        for (int i = 0; i < _assignments.Count; i++)
        {
            assignments.Add(new ServiceAssignment(_assignments[i].description, _assignments[i].isCompleted));
        }
        //selectionToggle.interactable = accessible;
    }

    void ShowAssignments()
    {
        //clear previous entries in assignment panel
        InterfaceManager.Instance.assignmentsPanelScrollView.transform.ClearChildren();
        InterfaceManager.Instance._currentEntry = this;
        for (int i = 0; i < assignments.Count; i++)
        {
           Instantiate(InterfaceManager.Instance.assignmentPrefab, InterfaceManager.Instance.assignmentsPanelScrollView.transform).GetComponent<Assignment>().Set(assignments[i].description);
        }
        InterfaceManager.Instance.assignmentsPanel.SetActive(true);
    }

    void ClearChildren(Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) { if (!child.name.Equals(parent.name)) Destroy(child.gameObject); }
    }
}
