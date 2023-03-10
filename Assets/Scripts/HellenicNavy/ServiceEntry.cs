using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SPS;
using System;
using System.Linq;
using UnityEngine.UI;
using UnitySQLite;

public class ServiceEntry : MonoBehaviour
{
    public TMP_InputField descriptionField;
    public TMP_InputField hoursField;
    public TMP_InputField daysField;
    public Toggle selectionToggle;
    public Image bgImg;
    public Color normalColor, selectedColor, postponedColor, highlightedColor;
    public RectTransform serviceTypesParentHours;
    public RectTransform serviceTypesParentDays;
    public List<ServiceAssignmentType> serviceAssignmentTypesHours;
    public List<ServiceAssignmentType> serviceAssignmentTypesDays;
    public ServiceStatus status;

    public int Hours
    {
        get
        {
            hoursField.text = hoursField.text.Trim();
            return int.Parse(hoursField.text);
        }
    }

    public int Days
    {
        get
        {
            daysField.text = daysField.text.Trim();
            return int.Parse(daysField.text);
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

    public bool IsSelected { get { return selectionToggle.isOn; } }

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

        selectionToggle.onValueChanged.AddListener((b) => bgImg.color = b ? selectedColor : normalColor);
    }

    public void DisplayFromData(string descr, int hours, int days, ServiceAssignmentType serviceTypesHours, ServiceAssignmentType serviceTypesDays, ServiceStatus status)
    {
        StartCoroutine(CreateInterfaceFromData(descr, hours, days, serviceTypesHours, serviceTypesDays, status));
    }

    public IEnumerator CreateInterfaceFromData(string descr, int hours, int days, ServiceAssignmentType serviceTypesHours, ServiceAssignmentType serviceTypesDays, ServiceStatus status)
    {
        bool accessible = AccountManagement.Instance.CurrentAccount.AccessLevel == UnitySQLite.Utilities.AccessLevel.admin;
        descriptionField.text = descr;
        descriptionField.interactable = accessible;

        hoursField.text += string.Format("{0}", hours);
        TMP_Dropdown dh = Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentHours).GetComponent<TMP_Dropdown>();
        dh.value = (int)serviceTypesHours;
        dh.interactable = accessible;
        hoursField.interactable = accessible;
        yield return new WaitForEndOfFrame();

        hoursField.text.TrimEnd();

        daysField.text += string.Format("{0}", days);
        TMP_Dropdown dd = Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentDays).GetComponent<TMP_Dropdown>();
        dd.value = (int)serviceTypesDays;
        dd.interactable = accessible;
        daysField.interactable = accessible;
        yield return new WaitForEndOfFrame();

        this.status = status;
        switch (status)
        {
            case ServiceStatus.pending:
            case ServiceStatus.completed:
                bgImg.color = normalColor;
                break;
            case ServiceStatus.postponed: //highlight previously postponed service entries
                bgImg.color = Color.yellow;
                break;
            default:
                break;
        }

        daysField.text.TrimEnd();

        //selectionToggle.interactable = accessible;
    }
   
    void ClearChildren(Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) { if (!child.name.Equals(parent.name)) Destroy(child.gameObject); }
    }
}
