using TMPro;
using UnityEngine;
using System.Collections.Generic;
using SPS;
using System;
using UnityEngine.Diagnostics;
using System.Linq;

public class ServiceEntry : MonoBehaviour
{
    public TMP_InputField descriptionField;
    public TMP_InputField hoursField;
    public TMP_InputField daysField;
    public RectTransform serviceTypesParentHours;
    public RectTransform serviceTypesParentDays;
    public List<ServiceAssignmentType> serviceAssignmentTypesHours;
    public List<ServiceAssignmentType> serviceAssignmentTypesDays;

    public List<int> Hours
    {
        get
        {
            hoursField.text = hoursField.text.Trim();
            List<int> result = new List<int>();
            if (hoursField.text.Length == 0)
            {
                return result;
            }
            string[] nlSeparated = hoursField.text.Split('\n');
            for (int i = 0; i < nlSeparated.Length; i++)
            {
                result.Add(int.Parse(nlSeparated[i]));
            }
            return result;
        }
    }
    public List<int> Days
    {
        get
        {
            daysField.text = daysField.text.Trim();
            List<int> result = new List<int>();
            if (daysField.text.Length == 0)
            {
                return result;
            }
            string[] nlSeparated = daysField.text.Split('\n');
            for (int i = 0; i < nlSeparated.Length; i++)
            {
                result.Add(int.Parse(nlSeparated[i]));
            }
            return result;
        }
    }

    public string Descr { get { return descriptionField.text; } }

    public List<ServiceAssignmentType> TypesHours
    {
        get
        {
            serviceAssignmentTypesHours = new List<ServiceAssignmentType>();
            //get all assignment types
            for (int i = 0; i < serviceTypesParentHours.childCount; i++)
            {
                serviceAssignmentTypesHours.Add((ServiceAssignmentType)serviceTypesParentHours.GetChild(i).GetComponent<TMP_Dropdown>().value);
            }

            return serviceAssignmentTypesHours;
        }
    }

    public List<ServiceAssignmentType> TypesDays
    {
        get
        {
            serviceAssignmentTypesDays = new List<ServiceAssignmentType>();
            for (int i = 0; i < serviceTypesParentDays.childCount; i++)
            {
                serviceAssignmentTypesDays.Add((ServiceAssignmentType)serviceTypesParentDays.GetChild(i).GetComponent<TMP_Dropdown>().value);
            }

            return serviceAssignmentTypesDays;
        }
    }

    private void Start()
    {
        //validator for input
        TMP_InputValidator validator = new TextValidator();
        validator.name = "Input Validation for Hours and Days";
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
    }
    void ClearChildren(Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) { if (!child.name.Equals(parent.name)) Destroy(child.gameObject); }
    }
}
